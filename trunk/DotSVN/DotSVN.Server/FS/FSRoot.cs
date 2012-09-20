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
using DotSVN.Common.Exceptions;
using DotSVN.Common.Util;
using DotSVN.Server.Delta;
using DotSVN.Server.RepositoryAccess.File;

namespace DotSVN.Server.FS
{
    public abstract class FSRoot
    {
        private readonly FSFS parentFSFS;
        private RevisionCache revisionNodesCache;
        protected internal FSRevisionNode fsRootRevisionNode;

        protected internal FSRoot(FSFS owner)
        {
            parentFSFS = owner;
        }

        public FSFS Owner
        {
            get { return parentFSFS; }
        }

        public abstract FSRevisionNode RootRevisionNode { get; }
        public abstract IDictionary<string, FSPathChange> GetChangedPaths();

        public FSRevisionNode GetRevisionNode(String path)
        {
            FSRevisionNode node = FetchRevNodeFromCache(path);
            if (node == null)
            {
                FSParentPath parentPath = OpenPath(path, true, false);
                node = parentPath.RevNode;
            }
            return node;
        }

        public abstract FSCopyInheritance getCopyInheritance(FSParentPath child);

        public FSParentPath OpenPath(String path, bool lastEntryMustExist, bool storeParents)
        {
            String canonPath = path;
            FSRevisionNode here = RootRevisionNode;
            String pathSoFar = "/";

            FSParentPath parentPath = new FSParentPath(here, null, null);
            parentPath.CopyStyle = CopyIdInheritanceStyle.Self;

            // skip the leading '/'
            String rest = canonPath.Substring(1);

            while (true)
            {
                String entry = SVNPathUtil.head(rest);
                String next = SVNPathUtil.removeHead(rest);
                pathSoFar = Path.Combine(pathSoFar, entry);
                FSRevisionNode child;
                if (entry == null || "".Equals(entry))
                {
                    child = here;
                }
                else
                {
                    FSRevisionNode cachedRevNode = FetchRevNodeFromCache(pathSoFar);
                    if (cachedRevNode != null)
                    {
                        child = cachedRevNode;
                    }
                    else
                    {
                        try
                        {
                            child = here.getChildDirNode(entry, Owner);
                        }
                        catch (SVNException svne)
                        {
                            if (svne.ErrorMessage.ErrorCode == SVNErrorCode.FS_NOT_FOUND)
                            {
                                if (!lastEntryMustExist && (next == null || "".Equals(next)))
                                {
                                    return new FSParentPath(null, entry, parentPath);
                                }
                                SVNErrorManager.error(FSErrors.ErrorNotFound(this, path), svne);
                            }
                            throw;
                        }
                    }

                    parentPath.setParentPath(child, entry, storeParents ? new FSParentPath(parentPath) : null);

                    if (storeParents)
                    {
                        FSCopyInheritance copyInheritance = getCopyInheritance(parentPath);
                        if (copyInheritance != null)
                        {
                            parentPath.CopyStyle = copyInheritance.Style;
                            parentPath.CopySourcePath = copyInheritance.CopySourcePath;
                        }
                    }

                    if (cachedRevNode == null)
                    {
                        putRevNodeToCache(pathSoFar, child);
                    }
                }
                if (next == null || "".Equals(next))
                {
                    break;
                }

                if (child.Type != SVNNodeKind.dir)
                {
                    SVNErrorMessage err = FSErrors.ErrorNotDirectory(pathSoFar, Owner);
                    SVNErrorManager.error(err.wrap("Failure opening ''{0}''", path));
                }
                rest = next;
                here = child;
            }
            return parentPath;
        }

        public SVNNodeKind checkNodeKind(String path)
        {
            FSRevisionNode revNode;
            try
            {
                revNode = GetRevisionNode(path);
            }
            catch (SVNException svne)
            {
                if (svne.ErrorMessage.ErrorCode == SVNErrorCode.FS_NOT_FOUND)
                {
                    return SVNNodeKind.none;
                }
                throw;
            }
            return revNode.Type;
        }

        public void putRevNodeToCache(String path, FSRevisionNode node)
        {
            if (!path.StartsWith("/"))
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.UNKNOWN, "Invalid path ''{0}''", path);
                SVNErrorManager.error(err);
            }
            if (revisionNodesCache == null)
            {
                revisionNodesCache = new RevisionCache(100);
            }
            revisionNodesCache.Put(path, node);
        }

        public void removeRevNodeFromCache(String path)
        {
            if (!path.StartsWith("/"))
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.UNKNOWN, "Invalid path ''{0}''", path);
                SVNErrorManager.error(err);
            }
            if (revisionNodesCache == null)
            {
                return;
            }
            revisionNodesCache.Delete(path);
        }

        protected internal FSRevisionNode FetchRevNodeFromCache(String path)
        {
            if (revisionNodesCache == null)
            {
                return null;
            }
            if (!path.StartsWith("/"))
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.UNKNOWN, "Invalid path ''{0}''", path);
                SVNErrorManager.error(err);
            }
            return revisionNodesCache.Fetch(path);
        }

        private void foldChange(IDictionary<string, FSPathChange> mapChanges, FSPathChange change)
        {
            if (change == null)
            {
                return;
            }
            mapChanges = mapChanges != null ? mapChanges : new Dictionary<string, FSPathChange>();
            FSPathChange newChange;
            String copyfromPath;
            long copyfromRevision;

            FSPathChange oldChange = mapChanges[change.Path];
            if (oldChange != null)
            {
                copyfromPath = oldChange.CopyPath;
                copyfromRevision = oldChange.CopyRevision;

                if ((change.RevNodeId == null) && (FSPathChangeKind.reset != change.ChangeKind))
                {
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Missing required node revision ID");
                    SVNErrorManager.error(err);
                }
                if ((change.RevNodeId != null) && (!oldChange.RevNodeId.Equals(change.RevNodeId)) &&
                    (oldChange.ChangeKind != FSPathChangeKind.delete))
                {
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT,
                                               "Invalid change ordering: new node revision ID without delete");
                    SVNErrorManager.error(err);
                }
                if (FSPathChangeKind.delete == oldChange.ChangeKind &&
                    !(FSPathChangeKind.replace == change.ChangeKind ||
                      FSPathChangeKind.reset == change.ChangeKind ||
                      FSPathChangeKind.add == change.ChangeKind) )
                {
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT,
                                               "Invalid change ordering: non-add change on deleted path");
                    SVNErrorManager.error(err);
                }
                if (FSPathChangeKind.modify == change.ChangeKind)
                {
                    if (change.IsTextModified)
                    {
                        oldChange.IsTextModified = true;
                    }
                    if (change.IsPropertyModified)
                    {
                        oldChange.IsPropertyModified = true;
                    }
                }
                else if (FSPathChangeKind.add == change.ChangeKind ||
                         FSPathChangeKind.replace == change.ChangeKind)
                {
                    oldChange.ChangeKind = FSPathChangeKind.replace;
                    oldChange.RevNodeId = change.RevNodeId;
                    oldChange.IsTextModified = change.IsTextModified;
                    oldChange.IsPropertyModified = change.IsPropertyModified;
                    if (change.CopyPath != null)
                    {
                        copyfromPath = change.CopyPath;
                        copyfromRevision = change.CopyRevision;
                    }
                }
                else if (FSPathChangeKind.delete == change.ChangeKind)
                {
                    if (FSPathChangeKind.add == oldChange.ChangeKind)
                    {
                        oldChange = null;
                        mapChanges.Remove(change.Path);
                    }
                    else
                    {
                        oldChange.ChangeKind = FSPathChangeKind.delete;
                        oldChange.IsPropertyModified = change.IsPropertyModified;
                        oldChange.IsTextModified = change.IsTextModified;
                    }

                    copyfromPath = null;
                    copyfromRevision = FSRepository.SVN_INVALID_REVNUM;
                }
                else if (FSPathChangeKind.reset == change.ChangeKind)
                {
                    oldChange = null;
                    copyfromPath = null;
                    copyfromRevision = FSRepository.SVN_INVALID_REVNUM;
                    mapChanges.Remove(change.Path);
                }

                newChange = oldChange;
            }
            else
            {
                copyfromPath = change.CopyPath;
                copyfromRevision = change.CopyRevision;
                newChange = change;
            }

            if (newChange != null)
            {
                newChange.CopyPath = copyfromPath;
                newChange.CopyRevision = copyfromRevision;
                mapChanges[change.Path] = newChange;
            }
        }

        protected internal IDictionary<string, FSPathChange> fetchAllChanges(FSFile changesFile, bool prefolded)
        {
            IDictionary<string, FSPathChange> changedPaths = new Dictionary<string, FSPathChange>();
            FSPathChange change = readChange(changesFile);
            while (change != null)
            {
                foldChange(changedPaths, change);
                if ( (FSPathChangeKind.delete == change.ChangeKind || FSPathChangeKind.replace == change.ChangeKind) 
                     && !prefolded )
                {
                    foreach( string keyPath in changedPaths.Keys )
                    {
                        if (keyPath == change.Path)
                        {
                            continue;
                        }
                        if( SVNPathUtil.pathIsChild(change.Path, keyPath) != null)
                        {
                            changedPaths.Remove(keyPath);
                        }
                    }
                }
                change = readChange(changesFile);
            }
            return changedPaths;
        }

        public IDictionary<string, FSPathChange> detectChanged()
        {
            IDictionary<string, FSPathChange> changes = GetChangedPaths();
            if (changes.Count == 0)
            {
                return changes;
            }

            foreach (string keyPath in changes.Keys)
            {
                if (changes[keyPath].ChangeKind == FSPathChangeKind.reset )
                {
                    changes.Remove(keyPath);
                }
            }
            return changes;
        }

        private FSPathChange readChange(FSFile raReader)
        {
            String changeLine;
            try
            {
                changeLine = raReader.ReadLine(4096);
            }
            catch (SVNException svne)
            {
                if (svne.ErrorMessage.ErrorCode == SVNErrorCode.STREAM_UNEXPECTED_EOF)
                {
                    return null;
                }
                throw;
            }
            if (changeLine == null || changeLine.Length == 0)
            {
                return null;
            }
            String copyfromLine = raReader.ReadLine(4096);
            return FSPathChange.fromString(changeLine, copyfromLine);
        }

        // TODO: Uncomment this
        public Stream getFileStreamForPath(SVNDeltaCombiner combiner, String path)
        {
            FSRevisionNode fileNode = GetRevisionNode(path);
            return FSInputStream.createDeltaStream(combiner, fileNode, Owner);
        }

        #region Nested type: RevisionCache

        /// <summary>
        /// Caches <see cref="FSRevisionNode"/> with a limit
        /// </summary>
        private sealed class RevisionCache
        {
            private readonly Dictionary<string, FSRevisionNode> revisionNodeCache;
            private readonly List<string> keys;
            private readonly int sizeLimit;

            /// <summary>
            /// Initializes a new instance of the <see cref="RevisionCache"/> class.
            /// </summary>
            /// <param name="limit">Sets max number of cached entries.</param>
            public RevisionCache(int limit)
            {
                sizeLimit = limit;
                keys = new List<string>();
                revisionNodeCache = new Dictionary<string, FSRevisionNode>();
            }

            /// <summary>
            /// Adds the <see cref="FSRevisionNode"/> to cache.
            /// </summary>
            /// <param name="key">The path.</param>
            /// <param name="revisionNode">The <see cref="FSRevisionNode"/> to be cached.</param>
            public void Put(string key, FSRevisionNode revisionNode)
            {
                if (sizeLimit <= 0)
                {
                    return;
                }
                if (keys.Count == sizeLimit)
                {
                    string cachedKey = keys[keys.Count - 1];
                    keys.RemoveAt(keys.Count - 1);
                    revisionNodeCache.Remove(cachedKey);
                }
                keys.Insert(0, key);
                revisionNodeCache[key] = revisionNode;
            }

            /// <summary>
            /// Deletes the specified <see cref="FSRevisionNode"/> from cache.
            /// </summary>
            /// <param name="key">The <see cref="FSRevisionNode"/> to be removed from cache.</param>
            public void Delete(string key)
            {
                keys.Remove(key);
                revisionNodeCache.Remove(key);
            }

            /// <summary>
            /// Reterives the specified <see cref="FSRevisionNode"/> for the key.
            /// </summary>
            /// <param name="key">The path.</param>
            /// <returns>The <see cref="FSRevisionNode"/> cached for the path</returns>
            public FSRevisionNode Fetch(string key)
            {
                int ind = keys.IndexOf(key);
                if (ind != - 1)
                {
                    if (ind != 0)
                    {
                        string cachedKey = keys[ind];
                        keys.RemoveAt(ind);
                        keys.Insert(0, cachedKey);
                    }
                    return revisionNodeCache[key];
                }
                return null;
            }

            /// <summary>
            /// Clears all the cache.
            /// </summary>
            public void Clear()
            {
                keys.Clear();
                revisionNodeCache.Clear();
            }
        }

        #endregion
    }
}
