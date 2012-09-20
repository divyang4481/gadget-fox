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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DotSVN.Common;
using DotSVN.Common.Entities;
using DotSVN.Common.Exceptions;
using DotSVN.Common.Util;
using DotSVN.Server.RepositoryAccess.File;

namespace DotSVN.Server.FS
{
    /// <summary>
    /// Encapsulates low level access to the FSFS file system.
    /// </summary>
    public class FSFS
    {
        #region Constants

        public const String CHILDREN_LOCK_KEY = "children";
        public const String COMMENT_LOCK_KEY = "comment";
        public const String CREATION_DATE_LOCK_KEY = "creation_date";
        private const int DB_FORMAT_VERSION_NUMBER = 2;
        private const String DB_TYPE = "fsfs";
        public const int DIGEST_SUBDIR_LEN = 3;
        public const String EXPIRATION_DATE_LOCK_KEY = "expiration_date";
        public const String IS_DAV_COMMENT_LOCK_KEY = "is_dav_comment";
        public const String OWNER_LOCK_KEY = "owner";
        public const String PATH_LOCK_KEY = "path";
        public const String PATH_PREFIX_NODE = "node.";
        private const int REPOS_FORMAT_VERSION_NUMBER = 5;
        public const String SVN_OPAQUE_LOCK_TOKEN = "opaquelocktoken:";
        public const String SVN_REPOS_DB_DIR = "db";
        public const String TOKEN_LOCK_KEY = "token";
        public const String TXN_PATH_EXT = ".txn";
        public const String TXN_PATH_EXT_CHILDREN = ".children";
        public const String TXN_PATH_EXT_PROPS = ".props";

        public const String TXN_PATH_REV = "rev";

        #endregion

        private FileInfo currentFile;
        private int dbFormatVersion;
        private readonly FileInfo dbRootDir;
        private bool isOpen = false;
        private readonly FileInfo locksRootFile;
        private int repositoryFormatVersion;

        private readonly FileInfo repositoryRootDir;
        private String repositoryUUID;
        private readonly FileInfo revisionPropsRootDir;
        private readonly FileInfo revisionsRootDir;
        private readonly FileInfo transactionsRootDir;
        private readonly FileInfo writeLockFile;


        /// <summary>
        /// Gets the DB format version number.
        /// </summary>
        /// <value>The DB format.</value>
        public int DBFormatVersion
        {
            get
            {
                if (!isOpen)
                    SVNErrorManager.error(
                        SVNErrorMessage.create(SVNErrorCode.UNKNOWN, "Repository should be opened before calling this."));
                return dbFormatVersion;
            }
        }

        /// <summary>
        /// Gets the Repository format version number.
        /// </summary>
        /// <value>The repository format.</value>
        public int RepositoryFormatVersion
        {
            get
            {
                if (!isOpen)
                    SVNErrorManager.error(
                        SVNErrorMessage.create(SVNErrorCode.UNKNOWN, "Repository should be opened before calling this."));
                return repositoryFormatVersion;
            }
        }

        /// <summary>
        /// Gets or sets the Repository UUID.
        /// </summary>
        /// <value>The Repository UUID.</value>
        public string UUID
        {
            get
            {
                if (repositoryUUID == null)
                {
                    // Read the uuid file
                    FSFile uuidFile = new FSFile(new FileInfo(Path.Combine(dbRootDir.FullName, "uuid")));
                    try
                    {
                        repositoryUUID = uuidFile.ReadLine(38);
                    }
                    finally
                    {
                        uuidFile.Close();
                    }
                }

                return repositoryUUID;
            }
        }

        /// <summary>
        /// Gets the write lock file.
        /// </summary>
        /// <value>The write lock file.</value>
        public FileInfo WriteLockFile
        {
            get { return writeLockFile; }
        }

        /// <summary>
        /// Gets the DB root file.
        /// </summary>
        /// <value>The DB root file.</value>
        public FileInfo DBRootDir
        {
            get { return dbRootDir; }
        }

        /// <summary>
        /// Gives the Next Node ID and Next Copy ID as a string array
        ///  index 0: Node ID
        ///  index 1: Copy ID
        /// </summary>
        public string[] NextRevisionIDs
        {
            get
            {
                // Construct a string array of length 2
                string[] ids = new String[2];

                // Extract the next node id and next copy id from the 'current' file
                // The format of the "current" file is a single line of the form
                //  "<youngest-revision> <next-node-id> <next-copy-id>\n" giving the
                //  youngest revision, the next unique node-ID, and the next unique
                //  copy-ID for the repository.
                FSFile fsCurrentFile = new FSFile(CurrentFile);
                bool fileCorrupt = true;

                // Read the current file
                string currentFileLine;
                try
                {
                    currentFileLine = fsCurrentFile.ReadLine(80);
                }
                finally
                {
                    fsCurrentFile.Close();
                }

                // Extract the Next Node ID and Next Copy ID from the current line
                if (!string.IsNullOrEmpty(currentFileLine))
                {
                    int spaceIndex = currentFileLine.IndexOf(' ');
                    if (spaceIndex != -1)
                    {
                        currentFileLine = currentFileLine.Substring(spaceIndex + 1);
                        spaceIndex = currentFileLine.IndexOf(' ');
                        if (spaceIndex != -1)
                        {
                            ids[0] = currentFileLine.Substring(0, (spaceIndex) - (0));
                            ids[1] = currentFileLine.Substring(spaceIndex + 1);
                            fileCorrupt = false;
                        }
                    }
                }
                if (fileCorrupt)
                {
                    SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Corrupt current file");
                    SVNErrorManager.error(err);
                }

                return ids;
            }
        }

        public FileInfo TransactionsParentDir
        {
            get { return transactionsRootDir; }
        }

        protected internal FileInfo CurrentFile
        {
            get
            {
                if (currentFile == null)
                {
                    currentFile = new FileInfo(Path.Combine(dbRootDir.FullName, "current"));
                }
                return currentFile;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FSFS"/> class.
        /// </summary>
        /// <param name="repositoryRootDir">The <see cref="FileInfo"/> that points to the root of the FSFS repository.</param>
        public FSFS(FileInfo repositoryRootDir)
        {
            this.repositoryRootDir = repositoryRootDir;
            dbRootDir = new FileInfo(Path.Combine(this.repositoryRootDir.FullName, "db"));
            revisionsRootDir = new FileInfo(Path.Combine(dbRootDir.FullName, "revs"));
            revisionPropsRootDir = new FileInfo(Path.Combine(dbRootDir.FullName, "revprops"));
            transactionsRootDir = new FileInfo(Path.Combine(dbRootDir.FullName, "transactions"));
            writeLockFile = new FileInfo(Path.Combine(dbRootDir.FullName, "write-lock"));
            locksRootFile = new FileInfo(Path.Combine(dbRootDir.FullName, "locks"));
        }

        /// <summary>
        /// Gets the youngest revision.
        /// </summary>
        /// <value>The youngest revision.</value>
        public long GetYoungestRevision()
        {
            long youngestRevision = -1;
            FSFile fsCurrentFile = new FSFile(CurrentFile);
            try
            {
                String line = fsCurrentFile.ReadLine(180);
                int spaceIndex = line.IndexOf(' ');
                if (spaceIndex > 0)
                {
                    bool parseSuccess = Int64.TryParse(line.Substring(0, spaceIndex), out youngestRevision);
                    if (!parseSuccess)
                    {
                        SVNErrorMessage err =
                            SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT,
                                                   "Can't parse revision number in file '{0}'", fsCurrentFile);
                        SVNErrorManager.error(err);
                    }
                }
            }
            catch(Exception)
            {
            }
            finally
            {
                fsCurrentFile.Close();
            }

            return youngestRevision;
        }

        /// <summary>
        /// Opens the repository, variour format files and verifies that there are no unsupported formats.
        /// </summary>
        public void Open()
        {
            // Read the repository format from : /root/format
            FSFile formatFile = new FSFile(new FileInfo(Path.Combine(repositoryRootDir.FullName, "format")));
            int formatVersion = -1;
            try
            {
                formatVersion = formatFile.ReadInt();
            }
            finally
            {
                formatFile.Close();
            }
            if (formatVersion > REPOS_FORMAT_VERSION_NUMBER)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.REPOS_UNSUPPORTED_VERSION,
                                           string.Format("Expected Repository format '{0}'; found format '{1}'",
                                                         REPOS_FORMAT_VERSION_NUMBER, formatVersion));
                SVNErrorManager.error(err);
            }
            repositoryFormatVersion = formatVersion;

            // Read the fs format from :/root/db/format
            formatFile = new FSFile(new FileInfo(Path.Combine(dbRootDir.FullName, "format")));
            try
            {
                formatVersion = formatFile.ReadInt();
            }
            catch (SVNException svne)
            {
                if (svne.InnerException is FileNotFoundException)
                {
                    formatVersion = DB_FORMAT_VERSION_NUMBER;
                }
            }
            finally
            {
                formatFile.Close();
            }
            if (formatVersion > DB_FORMAT_VERSION_NUMBER)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_UNSUPPORTED_FORMAT,
                                           string.Format("Expected FS format '{0}'; found format '{1}'",
                                                         DB_FORMAT_VERSION_NUMBER, formatVersion));
                SVNErrorManager.error(err);
            }
            dbFormatVersion = formatVersion;

            // Read the fs type from: /root/db/fs-type
            formatFile = new FSFile(new FileInfo(Path.Combine(dbRootDir.FullName, "fs-type")));
            String fsType;
            try
            {
                fsType = formatFile.ReadLine(128);
            }
            finally
            {
                formatFile.Close();
            }
            if (!DB_TYPE.Equals(fsType))
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_UNKNOWN_FS_TYPE,
                                           string.Format("Unsupported fs type '{0}'", fsType));
                SVNErrorManager.error(err);
            }

            bool exists, canRead = false;
            FileInfo dbCurrentFile = CurrentFile;
            if (File.Exists(dbCurrentFile.FullName))
                exists = true;
            else
                exists = Directory.Exists(dbCurrentFile.FullName);

            if (exists)
            {
                try
                {
                    using (FileStream fs = dbCurrentFile.OpenRead())
                    {
                        fs.Close();
                        canRead = true;
                    }
                }
                catch
                {
                    canRead = false;
                }
            }

            if (!(exists && canRead))
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.IO_ERROR, string.Format("Can't open file '{0}'", dbCurrentFile));
                SVNErrorManager.error(err);
            }

            isOpen = true;
        }

        public IDictionary<string, string> GetRevisionProperties(long revision)
        {
            FSFile file = new FSFile(getRevisionPropertiesFile(revision));
            try
            {
                return file.ReadProperties(false);
            }
            finally
            {
                file.Close();
            }
        }

        public FSRevisionRoot createRevisionRoot(long revision)
        {
            return new FSRevisionRoot(this, revision);
        }

        public FSRevisionNode getRevisionNode(FSID id)
        {
            FSFile revisionFile;

            if (id.IsTransaction)
            {
                FileInfo file =
                    new FileInfo(getTransactionDir(id.TransactionID).FullName + "\\" + PATH_PREFIX_NODE + id.NodeID + "." +
                                 id.CopyID);
                revisionFile = new FSFile(file);
            }
            else
            {
                revisionFile = getRevisionFile(id.Revision);
                revisionFile.Seek(id.Offset);
            }

            IDictionary<string, string> headers;
            try
            {
                headers = revisionFile.ReadHeader();
            }
            finally
            {
                revisionFile.Close();
            }

            return FSRevisionNode.fromMap(headers);
        }

        public IDictionary<string, FSEntry> getDirContents(FSRevisionNode revNode)
        {
            if (revNode.TextRepresentation != null && revNode.TextRepresentation.IsTransaction)
            {
                FSFile childrenFile = getTransactionRevisionNodeChildrenFile(revNode.ID);
                IDictionary<string, FSEntry> entries;
                try
                {
                    IDictionary<string, string> rawEntries = childrenFile.ReadProperties(true);

                    // get all the keys whose value is null and remove them
                    List<string> keylist = new List<string>();
                    foreach (string key in rawEntries.Keys)
                    {
                        if (key == null)
                            keylist.Add(key);
                    }

                    foreach (string keyToBeRemoved in keylist)
                    {
                        rawEntries.Remove(keyToBeRemoved);
                    }

                    entries = parsePlainRepresentation(rawEntries, true);
                }
                finally
                {
                    childrenFile.Close();
                }
                return entries;
            }
            else if (revNode.TextRepresentation != null)
            {
                FSRepresentation textRep = revNode.TextRepresentation;
                FSFile revisionFile = null;

                try
                {
                    revisionFile = openAndSeekRepresentation(textRep);
                    String repHeader = revisionFile.ReadLine(160);

                    if (!"PLAIN".Equals(repHeader))
                    {
                        SVNErrorMessage err =
                            SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed representation header");
                        SVNErrorManager.error(err);
                    }

                    String checksum;
                    IDictionary<string, string> rawEntries = revisionFile.ReadProperties(false, out checksum);

                    if (!checksum.Equals(textRep.HexDigest))
                    {
                        SVNErrorMessage err =
                            SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT,
                                                   "Checksum mismatch while reading representation:\n\t Expected: {0}\n\t Actual:{1}",
                                                   new Object[] { checksum, textRep.HexDigest });
                        SVNErrorManager.error(err);
                    }

                    return parsePlainRepresentation(rawEntries, false);
                }
                finally
                {
                    if (revisionFile != null)
                    {
                        revisionFile.Close();
                    }
                }
            }
            return (new Dictionary<string, FSEntry>()); // returns an empty map, must not be null!!
        }

        public IDictionary<string, string> GetProperties(FSRevisionNode revNode)
        {
            if (revNode.PropsRepresentation != null && revNode.PropsRepresentation.IsTransaction)
            {
                FSFile propsFile = null;
                try
                {
                    propsFile = getTransactionRevisionNodePropertiesFile(revNode.ID);
                    return propsFile.ReadProperties(false);
                }
                finally
                {
                    if (propsFile != null)
                    {
                        propsFile.Close();
                    }
                }
            }
            else if (revNode.PropsRepresentation != null)
            {
                FSRepresentation propsRep = revNode.PropsRepresentation;
                FSFile revisionFile = null;

                try
                {
                    revisionFile = openAndSeekRepresentation(propsRep);
                    String repHeader = revisionFile.ReadLine(160);

                    if (!"PLAIN".Equals(repHeader))
                    {
                        SVNErrorMessage err =
                            SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed representation header");
                        SVNErrorManager.error(err);
                    }

                    String checksum;
                    IDictionary<string, string> props = revisionFile.ReadProperties(false, out checksum);

                    if (!checksum.Equals(propsRep.HexDigest))
                    {
                        SVNErrorMessage err =
                            SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT,
                                                   "Checksum mismatch while reading representation:\n   expected:  {0}\n     actual:  {1}",
                                                   new Object[] { checksum, propsRep.HexDigest });
                        SVNErrorManager.error(err);
                    }
                    return props;
                }
                finally
                {
                    if (revisionFile != null)
                    {
                        revisionFile.Close();
                    }
                }
            }
            return (new Dictionary<string, string>()); // no properties? return an empty map
        }

        public IDictionary listTransactions()
        {
            IDictionary result = new Hashtable();
            FileInfo txnsDir = TransactionsParentDir;

            FileInfo[] entries = SVNFileUtil.GetFiles(txnsDir);
            for (int i = 0; i < entries.Length; i++)
            {
                FileInfo entry = entries[i];
                if (entry.Name.Length <= TXN_PATH_EXT.Length || !entry.Name.EndsWith(TXN_PATH_EXT))
                {
                    continue;
                }
                String txnName = entry.Name.Substring(0, (entry.Name.LastIndexOf(TXN_PATH_EXT)) - (0));
                result[txnName] = entry;
            }
            return result;
        }

        public FileInfo getNewRevisionFile(long revision)
        {
            FileInfo revFile = new FileInfo(Path.Combine(revisionsRootDir.FullName, Convert.ToString(revision)));
            if (File.Exists(revFile.FullName))
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.FS_CONFLICT, "Revision already exists");
                SVNErrorManager.error(err);
            }
            return revFile;
        }

        public FileInfo getNewRevisionPropertiesFile(long revision)
        {
            FileInfo revPropsFile =
                new FileInfo(Path.Combine(revisionPropsRootDir.FullName, Convert.ToString(revision)));
            if (File.Exists(revPropsFile.FullName))
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.FS_CONFLICT, "Revision already exists");
                SVNErrorManager.error(err);
            }
            return revPropsFile;
        }

        public FileInfo getTransactionDir(String txnID)
        {
            return new FileInfo(TransactionsParentDir.FullName + Path.PathSeparator + txnID + TXN_PATH_EXT);
        }

        public FileInfo getRevisionPropertiesFile(long revision)
        {
            FileInfo file = new FileInfo(Path.Combine(revisionPropsRootDir.FullName, Convert.ToString(revision)));
            if (!File.Exists(file.FullName))
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_NO_SUCH_REVISION, string.Format("No such revision {0}", revision));
                SVNErrorManager.error(err);
            }
            return file;
        }

        public FileInfo getRepositoryRoot()
        {
            return repositoryRootDir;
        }

        public FSFile openAndSeekRepresentation(FSRepresentation rep)
        {
            if (!rep.IsTransaction)
            {
                return openAndSeekRevision(rep.Revision, rep.Offset);
            }
            return openAndSeekTransaction(rep);
        }

        public FileInfo getNextIDsFile(String txnID)
        {
            return new FileInfo(Path.Combine(getTransactionDir(txnID).FullName, "next-ids"));
        }

        public IDictionary<string, string> getTransactionProperties(String txnID)
        {
            FSFile txnPropsFile = new FSFile(getTransactionPropertiesFile(txnID));
            try
            {
                return txnPropsFile.ReadProperties(false);
            }
            finally
            {
                txnPropsFile.Close();
            }
        }

        public FileInfo getTransactionPropertiesFile(String txnID)
        {
            return new FileInfo(Path.Combine(getTransactionDir(txnID).FullName, "props"));
        }

        public FileInfo getTransactionRevNodeFile(FSID id)
        {
            return
                new FileInfo(getTransactionDir(id.TransactionID).FullName + "\\" + PATH_PREFIX_NODE + id.NodeID + "." +
                             id.CopyID);
        }

        public SVNLock getLock(String repositoryPath, bool haveWriteLock)
        {
            SVNLock svnLock = fetchLockFromDigestFile(null, repositoryPath, null);

            if (svnLock == null)
            {
                SVNErrorManager.error(FSErrors.ErrorNoSuchLock(repositoryPath, this));
            }
            else
            {
                DateTime current = DateTime.Now;
                if (svnLock.ExpirationDate != null && current.CompareTo(svnLock.ExpirationDate) > 0)
                {
                    if (haveWriteLock)
                    {
                        deleteLock(svnLock);
                    }
                    SVNErrorManager.error(FSErrors.ErrorLockExpired(svnLock.ID, this));
                }
            }
            return svnLock;
        }

        public void deleteLock(SVNLock svnLock)
        {
            String reposPath = svnLock.Path;
            String childToKill = null;
            IList children = new ArrayList();
            while (true)
            {
                fetchLockFromDigestFile(null, reposPath, children);
                if (childToKill != null)
                {
                    children.Remove(childToKill);
                }

                if (children.Count == 0)
                {
                    childToKill = getDigestFromRepositoryPath(reposPath);
                    FileInfo digestFile = getDigestFileFromRepositoryPath(reposPath);
                    try
                    {
                        digestFile.Delete();
                    }
                    catch
                    {
                        SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.IO_ERROR, string.Format("Cannot delete file '{0}'", digestFile));
                        SVNErrorManager.error(err);
                    }
                }
                else
                {
                    writeDigestLockFile(null, children, reposPath);
                    childToKill = null;
                }

                if ("/".Equals(reposPath))
                {
                    break;
                }

                reposPath = SVNPathUtil.removeTail(reposPath);

                if ("".Equals(reposPath))
                {
                    reposPath = "/";
                }
                children.Clear();
            }
        }

        public SVNLock getLockHelper(String repositoryPath, bool haveWriteLock)
        {
            SVNLock svnLock;
            try
            {
                svnLock = getLock(repositoryPath, haveWriteLock);
            }
            catch (SVNException svnException)
            {
                if (svnException.ErrorMessage.ErrorCode == SVNErrorCode.FS_NO_SUCH_LOCK ||
                    svnException.ErrorMessage.ErrorCode == SVNErrorCode.FS_LOCK_EXPIRED)
                {
                    return null;
                }
                throw;
            }
            return svnLock;
        }

        public SVNLock fetchLockFromDigestFile(FileInfo digestFile, String repositoryPath, IList children)
        {
            FileInfo digestLockFile = (digestFile == null)
                                          ? getDigestFileFromRepositoryPath(repositoryPath)
                                          : digestFile;
            IDictionary<string, string> lockProps = new Dictionary<string, string>();

            if (File.Exists(digestLockFile.FullName) || Directory.Exists(digestLockFile.FullName))
            {
                FSFile fsDigestLockFile = new FSFile(digestLockFile);
                try
                {
                    lockProps = fsDigestLockFile.ReadProperties(false);
                }
                catch (SVNException svne)
                {
                    SVNErrorMessage err =
                        svne.ErrorMessage.wrap("Can't parse lock/entries hashfile ''{0}''", digestLockFile);
                    SVNErrorManager.error(err);
                }
                finally
                {
                    fsDigestLockFile.Close();
                }
            }

            SVNLock svnLock = null;
            string lockPath = lockProps[PATH_LOCK_KEY];
            if (lockPath != null)
            {
                string lockToken = lockProps[TOKEN_LOCK_KEY];
                if (lockToken == null)
                {
                    SVNErrorManager.error(FSErrors.ErrorCorruptLockFile(lockPath, this));
                }
                string lockOwner = lockProps[OWNER_LOCK_KEY];
                if (lockOwner == null)
                {
                    SVNErrorManager.error(FSErrors.ErrorCorruptLockFile(lockPath, this));
                }
                string davComment = lockProps[IS_DAV_COMMENT_LOCK_KEY];
                if (davComment == null)
                {
                    SVNErrorManager.error(FSErrors.ErrorCorruptLockFile(lockPath, this));
                }
                string creationTime = lockProps[CREATION_DATE_LOCK_KEY];
                if (creationTime == null)
                {
                    SVNErrorManager.error(FSErrors.ErrorCorruptLockFile(lockPath, this));
                }
                DateTime creationDate = SVNTimeUtil.parseDate(creationTime);
                string expirationTime = lockProps[EXPIRATION_DATE_LOCK_KEY];
                DateTime expirationDate = new DateTime(0);
                if (expirationTime != null)
                {
                    expirationDate = SVNTimeUtil.parseDate(expirationTime);
                }
                string comment = lockProps[COMMENT_LOCK_KEY];
                svnLock = new SVNLock(lockPath, lockToken, lockOwner, comment, creationDate, expirationDate);
            }

            String childEntries = lockProps[CHILDREN_LOCK_KEY];
            if (children != null && childEntries != null)
            {
                String[] digests = childEntries.Split(new char[] { '\n' } );
                for (int i = 0; i < digests.Length; i++)
                {
                    
                    children.Add( digests[i] );
                }
            }
            return svnLock;
        }

        public FileInfo getDigestFileFromRepositoryPath(String repositoryPath)
        {
            String digest = getDigestFromRepositoryPath(repositoryPath);
            FileInfo parent =
                new FileInfo(locksRootFile.FullName + "\\" + digest.Substring(0, (DIGEST_SUBDIR_LEN) - (0)));
            return new FileInfo(Path.Combine(parent.FullName, digest));
        }

        public String getDigestFromRepositoryPath(String repositoryPath)
        {
            String digestData = string.Empty;;
            try
            {
                // Create a new instance of the MD5CryptoServiceProvider object.
                MD5 md5Hasher = MD5.Create();
                byte[] encodedBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("UTF-8").GetBytes(repositoryPath));

                StringBuilder sBuilder = new StringBuilder();

                for (int index = 0; index < encodedBytes.Length; index++)
                {
                    sBuilder.Append(encodedBytes[index].ToString("x2"));
                }

                digestData = sBuilder.ToString();

            }
            catch (IOException ioException)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.IO_ERROR, ioException.Message);
                SVNErrorManager.error(err, ioException);
            }
            catch (Exception exception)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.IO_ERROR, "MD5 implementation not found: {0}", exception.Message);
                SVNErrorManager.error(err, exception);
            }
            return digestData;
        }

        public void unlockPath(String path, String token, String username, bool breakLock, bool enableHooks)
        {
            String[] paths = new String[] { path };

            if (!breakLock && username == null)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_NO_USER,
                                           "Cannot unlock path ''{0}'', no authenticated username available", path);
                SVNErrorManager.error(err);
            }

            if (enableHooks)
            {
                FSHooks.runPreUnlockHook(repositoryRootDir, path, username);
            }

            FSWriteLock writeLock = FSWriteLock.GetWriteLock(this);
            lock (writeLock)
            {
                try
                {
                    writeLock.Lock();
                    unlock(path, token, username, breakLock);
                }
                finally
                {
                    writeLock.Unlock();
                    FSWriteLock.Release(writeLock);
                }
            }

            if (enableHooks)
            {
                try
                {
                    FSHooks.runPostUnlockHook(repositoryRootDir, paths, username);
                }
                catch (SVNException svne)
                {
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.REPOS_POST_UNLOCK_HOOK_FAILED,
                                               "Unlock succeeded, but post-unlock hook failed");
                    err.ChildErrorMessage = svne.ErrorMessage;
                    SVNErrorManager.error(err, svne);
                }
            }
        }

        public SVNLock lockPath(String path, String token, String username, String comment,
                                        ref DateTime expirationDate, long currentRevision, bool stealLock)
        {
            String[] paths = new String[] { path };

            if (username == null)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_NO_USER,
                                           "Cannot lock path ''{0}'', no authenticated username available.", path);
                SVNErrorManager.error(err);
            }

            FSHooks.runPreLockHook(repositoryRootDir, path, username);
            SVNLock svnLock;

            FSWriteLock writeLock = FSWriteLock.GetWriteLock(this);

            lock (writeLock)
            {
                try
                {
                    writeLock.Lock();
                    svnLock = Lock(path, token, username, comment, ref expirationDate, currentRevision, stealLock);
                }
                finally
                {
                    writeLock.Unlock();
                    FSWriteLock.Release(writeLock);
                }
            }

            try
            {
                FSHooks.runPostLockHook(repositoryRootDir, paths, username);
            }
            catch (SVNException svne)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.REPOS_POST_LOCK_HOOK_FAILED,
                                           "Lock succeeded, but post-lock hook failed");
                err.ChildErrorMessage = svne.ErrorMessage;
                SVNErrorManager.error(err, svne);
            }
            return svnLock;
        }

        public IDictionary<string, string> compoundMetaProperties(long revision)
        {
            Dictionary<string, string> metaProps = new Dictionary<string, string>();
            IDictionary<string, string> revProps = GetRevisionProperties(revision);
            String author = revProps[SVNRevisionProperty.AUTHOR];
            String date = revProps[SVNRevisionProperty.DATE];
            String uuid = UUID;
            String rev = Convert.ToString(revision);

            metaProps[SVNProperty.LAST_AUTHOR] = author;
            metaProps[SVNProperty.COMMITTED_DATE] = date;
            metaProps[SVNProperty.COMMITTED_REVISION] = rev;
            metaProps[SVNProperty.UUID] = uuid;
            return metaProps;
        }

        public static FileInfo findRepositoryRoot(FileInfo path)
        {
            if (path == null)
            {
                path = new FileInfo("");
            }
            FileInfo rootPath = path;
            while (!isRepositoryRoot(rootPath))
            {
                rootPath = new FileInfo(rootPath.DirectoryName);
            }
            return rootPath;
        }

        public static String findRepositoryRoot(String host, String path)
        {
            if (path == null)
            {
                path = string.Empty;
            }

            String testPath = host != null ? Path.Combine(@"\\" + host, path) : path;
            FileInfo rootPath = new FileInfo(testPath);
            while (!isRepositoryRoot(rootPath))
            {
                if (new FileInfo(rootPath.DirectoryName) == null)
                {
                    return null;
                }
                String name = rootPath.Name;
                path = path.Substring(0, (path.Length - name.Length) - (0));
                while (path.EndsWith("/") || path.EndsWith("\\"))
                {
                    path = path.Substring(0, (path.Length - 1) - (0));
                }
                if ("".Equals(path))
                {
                    return null;
                }
                testPath = host != null ? Path.Combine(@"\\" + host, path) : path;
                rootPath = new FileInfo(new FileInfo(testPath).FullName);
            }
            while (path.EndsWith("/"))
            {
                path = path.Substring(0, (path.Length - 1) - (0));
            }
            while (path.EndsWith("\\"))
            {
                path = path.Substring(0, (path.Length - 1) - (0));
            }
            return path;
        }

        protected internal FSFile getTransactionRevisionPrototypeFile(String txnID)
        {
            FileInfo revFile = new FileInfo(Path.Combine(getTransactionDir(txnID).FullName, TXN_PATH_REV));
            return new FSFile(revFile);
        }

        protected internal FSFile getTransactionChangesFile(String txnID)
        {
            FileInfo file = new FileInfo(Path.Combine(getTransactionDir(txnID).FullName, "changes"));
            return new FSFile(file);
        }

        protected internal FSFile getTransactionRevisionNodeChildrenFile(FSID txnID)
        {
            FileInfo childrenFile =
                new FileInfo(getTransactionDir(txnID.TransactionID).FullName + "\\" + PATH_PREFIX_NODE + txnID.NodeID + "." +
                             txnID.CopyID + TXN_PATH_EXT_CHILDREN);
            return new FSFile(childrenFile);
        }

        protected internal FSFile getRevisionFile(long revision)
        {
            FileInfo revisionFile = new FileInfo(Path.Combine(revisionsRootDir.FullName, Convert.ToString(revision)));
            if (!File.Exists(revisionFile.FullName))
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_NO_SUCH_REVISION, string.Format("No such revision {0}", revision));
                SVNErrorManager.error(err);
            }
            return new FSFile(revisionFile);
        }

        protected internal FSFile getTransactionRevisionNodePropertiesFile(FSID id)
        {
            FileInfo revNodePropsFile =
                new FileInfo(getTransactionDir(id.TransactionID).FullName + "\\" + PATH_PREFIX_NODE + id.NodeID + "." +
                             id.CopyID + TXN_PATH_EXT_PROPS);
            return new FSFile(revNodePropsFile);
        }

        private void unlock(String path, String token, String username, bool breakLock)
        {
            SVNLock svnLock = getLock(path, true);
            if (!breakLock)
            {
                if (token == null || !token.Equals(svnLock.ID))
                {
                    SVNErrorManager.error(FSErrors.ErrorNoSuchLock(svnLock.Path, this));
                }
                if (string.IsNullOrEmpty(username))
                {
                    SVNErrorManager.error(FSErrors.ErrorNoUser(this));
                }
                if (!username.Equals(svnLock.Owner))
                {
                    SVNErrorManager.error(FSErrors.ErrorLockOwnerMismatch(username, svnLock.Owner, this));
                }
            }
            deleteLock(svnLock);
        }

        private SVNLock Lock(String path, String token, String username, String comment,
                                     ref DateTime expirationDate, long currentRevision, bool stealLock)
        {
            long youngestRev = GetYoungestRevision();
            FSRevisionRoot root = createRevisionRoot(youngestRev);
            SVNNodeKind kind = root.checkNodeKind(path);

            if (kind == SVNNodeKind.dir)
            {
                SVNErrorManager.error(FSErrors.ErrorNotFile(path, this));
            }
            else if (kind == SVNNodeKind.none)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_NOT_FOUND, "Path ''{0}'' doesn't exist in HEAD revision",
                                           path);
                SVNErrorManager.error(err);
            }

            if (username == null || "".Equals(username))
            {
                SVNErrorManager.error(FSErrors.ErrorNoUser(this));
            }

            if (FSRepository.isValidRevision(currentRevision))
            {
                FSRevisionNode node = root.GetRevisionNode(path);
                long createdRev = node.ID.Revision;
                if (FSRepository.isInvalidRevision(createdRev))
                {
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.FS_OUT_OF_DATE,
                                               "Path ''{0}'' doesn't exist in HEAD revision", path);
                    SVNErrorManager.error(err);
                }
                if (currentRevision < createdRev)
                {
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.FS_OUT_OF_DATE,
                                               "Lock failed: newer version of ''{0}'' exists", path);
                    SVNErrorManager.error(err);
                }
            }

            SVNLock existingLock = getLockHelper(path, true);

            if (existingLock != null)
            {
                if (!stealLock)
                {
                    SVNErrorManager.error(FSErrors.ErrorPathAlreadyLocked(existingLock.Path, existingLock.Owner, this));
                }
                else
                {
                    deleteLock(existingLock);
                }
            }

            SVNLock svnLock;
            if (token == null)
            {
                // Create a GUID and format it in the format XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX
                string guid = Guid.NewGuid().ToString("D");
                
                // Create a new token using Opaque Lock token and a new GUID
                token = SVN_OPAQUE_LOCK_TOKEN + guid;
            }

            // Create an SVN Lock
            svnLock = new SVNLock(path, token, username, comment, DateTime.Now, expirationDate);
            setLock(svnLock);

            return svnLock;
        }

        private void setLock(SVNLock svnLock)
        {
            if (svnLock == null)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.UNKNOWN, "FATAL error: attempted to set a null lock");
                SVNErrorManager.error(err);
            }
            String lastChild = "";
            String path = svnLock.Path;
            IList children = new ArrayList();
            while (true)
            {
                String digestFileName = getDigestFromRepositoryPath(path);
                SVNLock fetchedLock = fetchLockFromDigestFile(null, path, children);

                if (svnLock != null)
                {
                    fetchedLock = svnLock;
                    svnLock = null;
                    lastChild = digestFileName;
                }
                else
                {
                    if ( !(children.Count == 0) && children.Contains(lastChild) )
                    {
                        break;
                    }
                    children.Add(lastChild);
                }

                writeDigestLockFile(fetchedLock, children, path);

                if ("/".Equals(path))
                {
                    break;
                }
                path = SVNPathUtil.removeTail(path);

                if ("".Equals(path))
                {
                    path = "/";
                }
                children.Clear();
            }
        }

        private static bool ensureDirExists(FileSystemInfo dir, bool create)
        {
            if (!Directory.Exists(dir.FullName) && create == true)
            {
                DirectoryInfo dirInfo = Directory.CreateDirectory(dir.FullName);
                return dirInfo.Exists;
            }
            else if (!Directory.Exists(dir.FullName))
            {
               return false;
            }
            return true;
        }

        private void writeDigestLockFile(SVNLock svnLock, IList children, String repositoryPath)
        {
            if (!ensureDirExists(locksRootFile, true))
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.UNKNOWN, "Can't create a directory at ''{0}''", locksRootFile);
                SVNErrorManager.error(err);
            }

            FileInfo digestLockFile = getDigestFileFromRepositoryPath(repositoryPath);
            String digest = getDigestFromRepositoryPath(repositoryPath);
            FileInfo lockDigestSubdir =
                new FileInfo(locksRootFile.FullName + "\\" + digest.Substring(0, (DIGEST_SUBDIR_LEN) - (0)));

            if (!ensureDirExists(lockDigestSubdir, true))
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.UNKNOWN, "Can't create a directory at ''{0}''", lockDigestSubdir);
                SVNErrorManager.error(err);
            }

            IDictionary props = new Hashtable();

            if (svnLock != null)
            {
                props[PATH_LOCK_KEY] = svnLock.Path;
                props[OWNER_LOCK_KEY] = svnLock.Owner;
                props[TOKEN_LOCK_KEY] = svnLock.ID;
                props[IS_DAV_COMMENT_LOCK_KEY] = "0";
                if (svnLock.Comment != null)
                {
                    props[COMMENT_LOCK_KEY] = svnLock.Comment;
                }

                DateTime tempAux = svnLock.CreationDate;
                props[CREATION_DATE_LOCK_KEY] = SVNTimeUtil.formatDate(ref tempAux);
                DateTime tempAux2 = svnLock.ExpirationDate;
                props[EXPIRATION_DATE_LOCK_KEY] = SVNTimeUtil.formatDate(ref tempAux2);
            }
            if (children != null && children.Count > 0)
            {
                string[] digests = new string[children.Count];
                int index = 0;
                foreach(string digestEntry in children)
                {
                    digests[index++] = digestEntry;
                }
                StringBuilder digestValue = new StringBuilder();
                for (int i = 0; i < digests.Length; i++)
                {
                    digestValue.Append(digests[i]);
                    digestValue.Append('\n');
                }
                props[CHILDREN_LOCK_KEY] = digestValue.ToString();
            }
            try
            {
                // TODO: Uncomments and fix the compile error
                //SVNProperties.setProperties(props, digestLockFile,
                //                            SVNFileUtil.createUniqueFile(new FileInfo(digestLockFile.DirectoryName),
                //                                                         digestLockFile.Name, ".tmp"),
                //                            SVNProperties.SVN_HASH_TERMINATOR);
            }
            catch (SVNException svne)
            {
                SVNErrorMessage err =
                    svne.ErrorMessage.wrap("Cannot write lock/entries hashfile ''{0}''", digestLockFile);
                SVNErrorManager.error(err, svne);
            }
        }

        private FSFile openAndSeekTransaction(FSRepresentation rep)
        {
            FSFile file = getTransactionRevisionPrototypeFile(rep.TransactionId);
            file.Seek(rep.Offset);
            return file;
        }

        private FSFile openAndSeekRevision(long revision, long offset)
        {
            FSFile file = getRevisionFile(revision);
            file.Seek(offset);
            return file;
        }

        private IDictionary<string, FSEntry> parsePlainRepresentation(IDictionary<string, string> entries, bool mayContainNulls)
        {
            IDictionary<string, FSEntry> representationMap = new Dictionary<string, FSEntry>();

            foreach (string key in entries.Keys)
            {
                String unparsedEntry = entries[key];

                if (unparsedEntry == null && mayContainNulls)
                {
                    continue;
                }

                FSEntry nextRepEntry = parseRepEntryValue(key, unparsedEntry);
                if (nextRepEntry == null)
                {
                    SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Directory entry corrupt");
                    SVNErrorManager.error(err);
                }
                representationMap[key] = nextRepEntry;
            }
            return representationMap;
        }

        private FSEntry parseRepEntryValue(String name, String value_Renamed)
        {
            if (value_Renamed == null)
            {
                return null;
            }
            int spaceInd = value_Renamed.IndexOf(' ');
            if (spaceInd == -1)
            {
                return null;
            }
            String kind = value_Renamed.Substring(0, (spaceInd) - (0));
            String rawID = value_Renamed.Substring(spaceInd + 1);

            SVNNodeKind type = (SVNNodeKind)Enum.Parse(typeof(SVNNodeKind), kind);
            FSID id = FSID.FromString(rawID);
            if ((type != SVNNodeKind.dir && type != SVNNodeKind.file) || id == null)
            {
                return null;
            }
            return new FSEntry(id, type, name);
        }

        private DateTime getRevisionTime(long revision)
        {
            IDictionary<string, string> revisionProperties = GetRevisionProperties(revision);
            String timeString = revisionProperties[SVNRevisionProperty.DATE];
            if (timeString == null)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_GENERAL, string.Format("Failed to find time on revision {0}",revision));
                SVNErrorManager.error(err);
            }
            return SVNTimeUtil.parseDate(timeString);
        }

        private static bool isRepositoryRoot(FileInfo candidatePath)
        {
            FileInfo formatFile = new FileInfo(Path.Combine(candidatePath.FullName, "format"));
            SVNNodeKind fileType = SVNFileUtil.GetNodeKind(formatFile);
            if (fileType != SVNNodeKind.file)
            {
                return false;
            }
            FileInfo dbFile = new FileInfo(Path.Combine(candidatePath.FullName, SVN_REPOS_DB_DIR));
            fileType = SVNFileUtil.GetNodeKind(dbFile);
            if (fileType != SVNNodeKind.dir )
            {
                return false;
            }
            return true;
        }
    }
}
