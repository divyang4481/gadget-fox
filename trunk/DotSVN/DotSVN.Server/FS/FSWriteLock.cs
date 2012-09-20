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
using System.IO;
using DotSVN.Common.Util;
using DotSVN.Common;

namespace DotSVN.Server.FS
{
    public class FSWriteLock
    {
        private static readonly IDictionary lockCache = new Hashtable();
        private FileStream lockFile;
        private int refCount = 0;
        private readonly FSFS fsfsOwner;
        private readonly String uuid;

        /// <summary>
        /// Initializes an instance of a file write lock
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="fsfsOwner"></param>
        private FSWriteLock(String uuid, FSFS fsfsOwner)
        {
            this.uuid = uuid;
            this.fsfsOwner = fsfsOwner;
        }

        /// <summary>
        /// Gets the file write lock
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static FSWriteLock GetWriteLock(FSFS owner)
        {
            lock( typeof(FSWriteLock) )
            {
                String uuid = owner.UUID;

                // Get the write lock from the cache or create a new one
                FSWriteLock writeLock = (FSWriteLock) lockCache[uuid];
                if (writeLock == null)
                {
                    writeLock = new FSWriteLock(uuid, owner);
                    lockCache[uuid] = writeLock;
                }

                // Increase the reference count of write locks for this file
                writeLock.refCount++;

                return writeLock;
            }
        }

        /// <summary>
        /// Locks the file for writing
        /// </summary>
        public virtual void Lock()
        {
            lock (this)
            {
                FileInfo writeLockFile = fsfsOwner.WriteLockFile;

                try
                {
                    SVNNodeKind type = SVNFileUtil.GetNodeKind(writeLockFile);
                    if (type == SVNNodeKind.unknown || type == SVNNodeKind.none)
                    {
                        SVNFileUtil.createEmptyFile(writeLockFile);
                    }

                    // Open the write lock file in exclusive mode
                    lockFile = new FileStream(writeLockFile.FullName, FileMode.Open, FileAccess.ReadWrite,FileShare.None);
                }
                catch (IOException ioe)
                {
                    Unlock();
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.IO_ERROR, "Can't get exclusive lock on file ''{0}'': {1}",
                                               new Object[] {writeLockFile, ioe.Message});
                    SVNErrorManager.error(err, ioe);
                }
            }
        }

        /// <summary>
        /// Releases the lock
        /// </summary>
        /// <param name="writeLock"></param>
        public static void Release(FSWriteLock writeLock)
        {
            lock( typeof(FSWriteLock) )
            {
                if (writeLock == null)
                {
                    return;
                }
                if ((--writeLock.refCount) == 0)
                {
                    lockCache.Remove(writeLock.uuid);
                }
            }
        }

        /// <summary>
        /// Unlocks the file
        /// </summary>
        public virtual void Unlock()
        {
            lock (this)
            {
                SVNFileUtil.closeFile(lockFile);
                lockFile = null;
            }
        }
    }
}