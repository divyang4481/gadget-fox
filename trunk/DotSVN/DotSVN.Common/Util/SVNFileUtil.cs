#region Copyright
/*
* ====================================================================
* Copyright (c) 2007 www.dotsvn.net.  All rights reserved.
*
* This software is licensed as described in the file LICENSE, which
* you should have received as part of this distribution.  
* ====================================================================
*/
#endregion //Copyright

using System;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace DotSVN.Common.Util
{
    public class SVNFileUtil
    {

        internal class DummyStream : Stream
        {
            public override void WriteByte(byte b)
            {
                throw new IOException("Not supported");
            }

            public override void Flush()
            {
                throw new IOException("Not supported");
            }

            public override Int64 Seek(Int64 offset, SeekOrigin origin)
            {
                throw new IOException("Not supported");
            }

            public override void SetLength(Int64 value)
            {
            }

            public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
            {
                return 0;
            }

            public override void Write(Byte[] buffer, Int32 offset, Int32 count)
            {
            }

            public override Boolean CanRead
            {
                get { return false; }
            }

            public override Boolean CanSeek
            {
                get { return false; }
            }

            public override Boolean CanWrite
            {
                get { return false; }
            }

            public override Int64 Length
            {
                get { return 0; }
            }

            public override Int64 Position
            {
                get { return 0; }

                set { }
            }
        }

        /// <summary>
        /// Creates an empty file.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/> for the file to be created.</param>
        /// <returns><c>true</c> if a new file was created, <c>fasle</c> is the file already exists or if file creation fails.</returns>
        public static bool createEmptyFile(FileInfo file)
        {
            if (file.Exists)
                return false;

            bool created;
            try
            {
                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }

                using (FileStream createdFile = file.Create())
                {
                    createdFile.Close();
                    created = true;
                }
            }
            catch
            {
                created = false;
            }
            if (!created)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.IO_ERROR, "Cannot create new file '{0}'", file);
                SVNErrorManager.error(err);
            }
            return created;
        }


        /// <summary>
        /// Creates the unique file.
        /// </summary>
        /// <param name="parent">The parent path.</param>
        /// <param name="name">The file name.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns>A <see cref="FileInfo" of the unique file/></returns>
        public static FileInfo createUniqueFile(FileInfo parent, String name, String suffix)
        {
            string path = parent.FullName;
            new FileIOPermission(FileIOPermissionAccess.Write, path).Demand();
            FileInfo tempFile = null;
            if (string.Compare(suffix.ToLower(), ".tmp") == 0)
            {
                StringBuilder tmpFileName = new StringBuilder(260);
                if (NativeMethods.GetTempFileName(path, name, 0, tmpFileName) == 0)
                {
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.IO_UNIQUE_NAMES_EXHAUSTED, "Unable to make name for '{0}'",
                                               new FileInfo(parent.FullName + "\\" + name));
                    SVNErrorManager.error(err);
                    return null;
                }
                tempFile = new FileInfo(tmpFileName.ToString());
            }
            else
            {
                FileInfo file = new FileInfo(Path.Combine(parent.FullName, name + suffix));
                for (int i = 1; i < 99999; i++)
                {
                    if (!file.Exists)
                    {
                        tempFile = file;
                        break;
                    }
                    file = new FileInfo(Path.Combine(parent.FullName, name + "." + i + suffix));
                }
            }
            return tempFile;
        }
       
        public static void deleteFile(FileInfo file)
        {
            if (file == null)
            {
                return;
            }

            bool isDirectory = false;
            bool isFile = file.Exists;
            if (!isFile)
            {
                isDirectory = Directory.Exists(file.FullName);
            }

            if (!(isFile || isDirectory))
                return;

            // Try to delete the file up to 10 times with a delay of 100ms in between
            for (int count = 0; count < 10; count++)
            {
                try
                {
                    if (isFile)
                    {
                        file.Delete();
                    }
                    else
                    {
                        Directory.Delete(file.FullName);
                    }
                }
                catch
                {
                    Thread.Sleep(100);
                    continue;
                }
                return;
            }
            SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.IO_ERROR, "Cannot delete file '{0}'", file);
            SVNErrorManager.error(err);
        }

        public static Stream openFileForWriting(FileInfo file)
        {
            return openFileForWriting(file, false);
        }

        public static Stream openFileForWriting(FileInfo file, bool append)
        {
            if (file == null)
            {
                return null;
            }
            try
            {
                if (file.Exists)
                {
                    if (append)
                    {
                        // Reset reasonly attribute if it is set
                        file.Attributes = file.Attributes | ~FileAttributes.ReadOnly;
                    }
                }
                else
                {
                    // Ensure that the parent DIr exists
                    if (!file.Directory.Exists)
                        file.Directory.Create();
                }
                FileMode mode = append ? FileMode.Append : FileMode.Create;
                return file.Open(mode, FileAccess.ReadWrite);
            }
            catch (Exception e)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.IO_ERROR, "Cannot write to ''{0}'': {1}",
                                           new Object[] {file, e.Message});
                SVNErrorManager.error(err, e);
            }
            return null;
        }

        public static Stream openFileForReading(FileInfo file)
        {
            if (file == null)
            {
                return null;
            }
            if (Directory.Exists(file.FullName))
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.IO_ERROR,
                                           string.Format("Cannot read from '{0}': path refers to directory or read access is denied",file) );
                SVNErrorManager.error(err);
            }
            if (!file.Exists)
            {
                return null;
            }
            try
            {
                return new BufferedStream(new FileStream(file.FullName, FileMode.Open, FileAccess.Read));
            }
            catch (FileNotFoundException e)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.IO_ERROR, "Cannot read from to '{0}': {1}",
                                           new Object[] { file, e.Message });
                SVNErrorManager.error(err, e);
            }
            return null;
        }
   
        /// <summary>
        /// Returns an array of abstract pathnames representing the files and directories of the specified path.
        /// </summary>
        /// <param name="path">The abstract pathname to list it childs.</param>
        /// <returns>An array of abstract pathnames childs of the path specified or null if the path is not a directory</returns>
        public static FileInfo[] GetFiles(FileInfo path)
        {
            if ((path.Attributes & FileAttributes.Directory) > 0)
            {
                String[] fullpathnames = Directory.GetFileSystemEntries(path.FullName);
                FileInfo[] result = new FileInfo[fullpathnames.Length];
                for (int i = 0; i < result.Length; i++)
                    result[i] = new FileInfo(fullpathnames[i]);
                return result;
            }
            else return null;
        }


        /// <summary>
        /// Closes the <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to close.</param>
        public static void closeFile(Stream stream)
        {
            if (stream == null)
            {
                return;
            }
            try
            {
                stream.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gets the <see cref="SVNNodeKind"/> for the given <see cref="FileInfo"/>.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <returns>The curresponding <see cref="SVNNodeKind"/></returns>
        public static SVNNodeKind GetNodeKind(FileInfo file)
        {
            if (file == null)
            {
                return SVNNodeKind.unknown;
            }
            else if (file.Exists)
            {
                return SVNNodeKind.file;
            }
            else if (Directory.Exists(file.FullName))
            {
                return SVNNodeKind.dir;
            }
            else
            {
                return SVNNodeKind.none;
            }
        }
    }
}