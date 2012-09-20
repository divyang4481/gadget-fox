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
using System.Collections.Generic;
using System.IO;
using DotSVN.Common;
using DotSVN.Common.Entities;
using DotSVN.Common.Exceptions;
using DotSVN.Common.Util;
using DotSVN.Server.Delta;
using DotSVN.Server.FS;

namespace DotSVN.Server.RepositoryAccess.File
{
    /// <summary>
    /// FSRepository driver
    /// </summary>
    public class FSRepository : SVNRepository
    {
        public const int SVN_INVALID_REVNUM = -1;

        private FSFS myFSFS = null;
        private FileInfo reposRootDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="FSRepository"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="session">The session.</param>
        public FSRepository(SVNURL url, ISVNSession session)
            : base(url, session)
        {
        }

        /// <summary>
        /// Tries to access a repository. Used to check if there're no problems
        /// with accessing a repository and to cache a repository UUID and root directory location.
        /// </summary>
        public override void TestConnection()
        {
            try
            {
                OpenRepository();
            }
            finally
            {
                CloseRepository();
            }
        }

        /// <summary>
        /// Opens the current Repository
        /// </summary>
        public override void OpenRepository()
        {
            try
            {
                openRepositoryRoot();
            }
            catch (SVNException svne)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.RA_LOCAL_REPOS_OPEN_FAILED, "Unable to open repository '{0}'",
                                           Location);
                err.ChildErrorMessage = svne.ErrorMessage;
                SVNErrorManager.error(err.wrap("Unable to open an ra_local session to URL"));
            }
        }

        private void openRepositoryRoot()
        {
            lockRepository();

            String hostName = Location.Host;
            bool hasCustomHostName = !string.IsNullOrEmpty(hostName) && !string.Equals(hostName, "localhost", StringComparison.CurrentCultureIgnoreCase);

            if (hasCustomHostName)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.RA_ILLEGAL_URL, string.Format("Local URL '{0}' contains unsupported hostname", Location));
                SVNErrorManager.error(err);
            }

            String startPath = Uri.UnescapeDataString(Location.Path);
            String rootPath = FSFS.findRepositoryRoot(hasCustomHostName ? hostName : null, startPath);
            if (rootPath == null)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.RA_LOCAL_REPOS_OPEN_FAILED, "Unable to open repository '{0}'", Location);
                SVNErrorManager.error(err);
            }

            reposRootDir = hasCustomHostName
                                 ? new FileInfo(@"\\" + hostName + @"\" + rootPath)
                                 : new FileInfo(rootPath);
            myFSFS = new FSFS(reposRootDir);
            myFSFS.Open();
            setRepositoryCredentials(myFSFS.UUID, Location.SetPath(rootPath));
        }

        /// <summary>
        /// Closes the current Repository
        /// </summary>
        public override void CloseRepository()
        {
            unlock();
            myFSFS = null;
        }

        public override SVNNodeKind CheckPath(String path, long revision)
        {
            try
            {
                OpenRepository();
                if (!SVNRepository.isValidRevision(revision))
                {
                    revision = myFSFS.GetYoungestRevision();
                }
                String repositoryPath = GetRepositoryPath(path);
                FSRevisionRoot root = myFSFS.createRevisionRoot(revision);
                return root.checkNodeKind(repositoryPath);
            }
            finally
            {
                CloseRepository();
            }
        }

        /// <summary> Returns a path relative to the repository root directory given
        /// a path relative to the location to which this driver object is set.
        /// 
        /// </summary>
        /// <param name="relativePath">a path relative to the location to which
        /// this <b>SVNRepository</b> is set
        /// </param>
        /// <returns>              a path relative to the repository root
        /// </returns>
        public string GetRepositoryPath(string relativePath)
        {
            if (relativePath == null)
            {
                return "/";
            }
            if (relativePath.Length > 0 && relativePath[0] == '/')
            {
                return relativePath;
            }
            String fullPath = SVNPathUtil.append(Location.Path, relativePath);
            String repositoryPath = fullPath.Substring(GetRepositoryRoot(true).Path.Length);
            if ("".Equals(repositoryPath))
            {
                return "/";
            }
            return repositoryPath;
        }


        public new static bool isInvalidRevision(long revision)
        {
            return SVNRepository.isInvalidRevision(revision);
        }

        public new static bool isValidRevision(long revision)
        {
            return SVNRepository.isValidRevision(revision);
        }

        public override long GetLatestRevision()
        {
            EnsureOpen();

            return myFSFS.GetYoungestRevision();
        }

        public override ICollection<SVNDirEntry> GetDir(string path, long revision, IDictionary<string, string> properties)
        {
            EnsureOpen();

            if (!SVNRepository.isValidRevision(revision))
            {
                revision = myFSFS.GetYoungestRevision();
            }

            String repositoryPath = GetRepositoryPath(path);
            FSRevisionRoot root = myFSFS.createRevisionRoot(revision);

            FSRevisionNode parent = root.GetRevisionNode(repositoryPath);


            SVNURL parentURL = Location.AppendPath(path);

            ICollection<SVNDirEntry> entriesCollection = getDirEntries(parent, parentURL, false);

            if (properties != null)
            {
                collectProperties(parent, ref properties);
            }
            return entriesCollection;
        }


        private ICollection<SVNDirEntry> getDirEntries(FSRevisionNode parent, SVNURL parentURL, bool includeLogs)
        {
            IDictionary<string, FSEntry> fsEntries = parent.getDirEntries(myFSFS);

            List<SVNDirEntry> dirEntriesList = new List<SVNDirEntry>();

            foreach (FSEntry fsEntry in fsEntries.Values)
            {
                if (fsEntry != null)
                {
                    dirEntriesList.Add(buildDirEntry(fsEntry, parentURL, null, includeLogs));
                }
            }
            return dirEntriesList;
        }


        private void collectProperties(FSRevisionNode revNode, ref IDictionary<string, string> properties)
        {
            IDictionary<string, string> versionedProps = revNode.getProperties(myFSFS);
            if (versionedProps != null && versionedProps.Count > 0)
            {
                foreach (string key in versionedProps.Keys)
                {
                    if (properties.ContainsKey(key))
                    {
                        properties[key] = versionedProps[key];
                    }
                    else
                    {
                        properties.Add(key, versionedProps[key]);
                    }
                }
            }

            IDictionary<string, string> metaProperties = null;
            try
            {
                metaProperties = myFSFS.compoundMetaProperties(revNode.ID.Revision);
            }
            catch (SVNException)
            {
                //
            }
            if (metaProperties != null && metaProperties.Count > 0)
            {
                foreach (string key in metaProperties.Keys)
                {
                    properties.Add(key, metaProperties[key]);
                }
            }
            return;
        }

        private SVNDirEntry buildDirEntry(FSEntry repEntry, SVNURL parentURL, FSRevisionNode entryNode, bool includeLogs)
        {
            entryNode = entryNode == null ? myFSFS.getRevisionNode(repEntry.Id) : entryNode;
            long size = 0;

            if (entryNode.Type == SVNNodeKind.file)
            {
                size = entryNode.FileLength;
            }

            IDictionary<string, string> props;
            props = entryNode.getProperties(myFSFS);
            bool hasProps = (props == null || props.Count == 0) ? false : true;

            IDictionary<string, string> revProps;
            revProps = myFSFS.GetRevisionProperties(repEntry.Id.Revision);

            String lastAuthor = null;
            String log = null;
            DateTime lastCommitDate = SVNTimeUtil.EmptyDateTime;

            if (revProps != null && revProps.Count > 0)
            {
                lastAuthor = revProps[SVNRevisionProperty.AUTHOR];
                log = revProps[SVNRevisionProperty.LOG];
                String timeString = revProps[SVNRevisionProperty.DATE];

                if (timeString != null)
                    lastCommitDate = SVNTimeUtil.parseDate(timeString);
            }

            SVNURL entryURL = parentURL.AppendPath(repEntry.Name);
            SVNDirEntry dirEntry =
                new SVNDirEntry(entryURL, repEntry.Name, repEntry.Type, size, hasProps, repEntry.Id.Revision,
                                ref lastCommitDate, lastAuthor, includeLogs ? log : null);
            dirEntry.RelativePath = repEntry.Name;
            return dirEntry;
        }


        private void EnsureOpen()
        {
            if (myFSFS == null)
            {
                OpenRepository();
            }
        }

        /// <summary>
        /// Fetches the contents and or properties of a file located at the specified path
        /// in a particular revision. If contents arg is not null it will be written with file contents.
        /// If properties arg is not null it will receive the properties of the file.
        /// This includes all properties: not just ones controlled by a user and stored in the repository filesystem,
        /// but also non-tweakable ones (e.g. 'wcprops', 'entryprops', etc.). Property names (keys) are mapped to property values.
        /// <para>The path arg can be both relative to the location of this driver and absolute to the repository root
        /// (starts with "/").</para>
        /// 	<para>If revision is invalid (negative), HEAD revision will be used.</para>
        /// </summary>
        /// <param name="path">a file path</param>
        /// <param name="revision">File revision</param>
        /// <param name="properties">File properties receiver map</param>
        /// <param name="contents">An output stream to write the file contents to</param>
        /// <returns>The revision the file has been taken at</returns>
        public override long GetFile(string path, long revision, IDictionary<string, string> properties, Stream contents)
        {
            EnsureOpen();
            if (!SVNRepository.isValidRevision(revision))
            {
                revision = myFSFS.GetYoungestRevision();
            }

            String repositoryPath = GetRepositoryPath(path);
            FSRevisionRoot root = myFSFS.createRevisionRoot(revision);

            if (contents != null)
            {
                Stream fileStream = null;
                try
                {
                    fileStream = root.getFileStreamForPath(new SVNDeltaCombiner(), repositoryPath);
                    FSRepositoryUtil.copy(fileStream, contents);
                }
                finally
                {
                    SVNFileUtil.closeFile(fileStream);
                }
            }
            if (properties != null)
            {
                FSRevisionNode revNode = root.GetRevisionNode(repositoryPath);
                if (revNode.FileChecksum != null)
                {
                    properties[SVNProperty.CHECKSUM] = revNode.FileChecksum;
                }
                if (revision >= 0)
                {
                    properties[SVNProperty.REVISION] = Convert.ToString(revision);
                }
                collectProperties(revNode, ref properties);
            }
            return revision;
        }
    }
}
