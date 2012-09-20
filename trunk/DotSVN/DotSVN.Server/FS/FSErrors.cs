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
using DotSVN.Common.Util;

namespace DotSVN.Server.FS
{

    public class FSErrors
    {
        public static SVNErrorMessage ErrorDanglingId(FSID id, FSFS owner)
        {
            FileInfo fsDir = owner.DBRootDir;
            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.FS_ID_NOT_FOUND,
                                       "Reference to non-existent node ''{0}'' in filesystem ''{1}''",
                                       new Object[] {id, fsDir});
            return err;
        }

        public static SVNErrorMessage ErrorTxnNotMutable(String txnId, FSFS owner)
        {
            FileInfo fsDir = owner.DBRootDir;
            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.FS_TRANSACTION_NOT_MUTABLE,
                                       "Cannot modify transaction named ''{0}'' in filesystem ''{1}''",
                                       new Object[] {txnId, fsDir});
            return err;
        }

        public static SVNErrorMessage ErrorNotMutable(long revision, String path, FSFS owner)
        {
            FileInfo fsDir = owner.DBRootDir;
            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.FS_NOT_MUTABLE,
                                       string.Format("File is not mutable: filesystem '{0}', revision {1}, path '{2}'", fsDir, revision, path));
            return err;
        }

        public static SVNErrorMessage ErrorNotFound(FSRoot root, String path)
        {
            SVNErrorMessage err;
            if (root is FSTransactionRoot)
            {
                FSTransactionRoot txnRoot = (FSTransactionRoot) root;
                err =
                    SVNErrorMessage.create(SVNErrorCode.FS_NOT_FOUND,
                                           "File not found: transaction ''{0}'', path ''{1}''",
                                           new Object[] {txnRoot.TxnID, path});
            }
            else
            {
                FSRevisionRoot revRoot = (FSRevisionRoot) root;
                err =
                    SVNErrorMessage.create(SVNErrorCode.FS_NOT_FOUND,
                                           string.Format("File not found: revision {0}, path '{1}'", revRoot.Revision, path));
            }
            return err;
        }

        public static SVNErrorMessage ErrorNotDirectory(String path, FSFS owner)
        {
            FileInfo fsDir = owner.DBRootDir;
            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.FS_NOT_DIRECTORY, "''{0}'' is not a directory in filesystem ''{1}''",
                                       new Object[] {path, fsDir});
            return err;
        }

        public static SVNErrorMessage ErrorCorruptLockFile(String path, FSFS owner)
        {
            FileInfo fsDir = owner.DBRootDir;
            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT,
                                       "Corrupt lockfile for path ''{0}'' in filesystem ''{1}''",
                                       new Object[] {path, fsDir});
            return err;
        }

        public static SVNErrorMessage ErrorOutOfDate(String path, String txnId)
        {
            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.FS_TXN_OUT_OF_DATE, "Out of date: ''{0}'' in transaction ''{1}''",
                                       new Object[] {path, txnId});
            return err;
        }

        public static SVNErrorMessage ErrorAlreadyExists(FSRoot root, String path, FSFS owner)
        {
            FileInfo fsDir = owner.DBRootDir;
            SVNErrorMessage err;
            if (root is FSTransactionRoot)
            {
                FSTransactionRoot txnRoot = (FSTransactionRoot) root;
                err =
                    SVNErrorMessage.create(SVNErrorCode.FS_ALREADY_EXISTS,
                                           "File already exists: filesystem ''{0}'', transaction ''{1}'', path ''{2}''",
                                           new Object[] {fsDir, txnRoot.TxnID, path});
            }
            else
            {
                FSRevisionRoot revRoot = (FSRevisionRoot) root;
                err =
                    SVNErrorMessage.create(SVNErrorCode.FS_ALREADY_EXISTS,
                                           "File already exists: filesystem ''{0}'', revision {1}, path ''{2}''",
                                           new Object[] {fsDir, revRoot.Revision, path});
            }
            return err;
        }

        public static SVNErrorMessage ErrorNotTxn()
        {
            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.FS_NOT_TXN_ROOT, "Root object must be a transaction root");
            return err;
        }

        public static SVNErrorMessage ErrorConflict(String path)
        {
            SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.FS_CONFLICT, "Conflict at ''{0}''", path);
            return err;
        }

        public static SVNErrorMessage ErrorNoSuchLock(String path, FSFS owner)
        {
            FileInfo fsDir = owner.DBRootDir;
            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.FS_NO_SUCH_LOCK, "No lock on path ''{0}'' in filesystem ''{1}''",
                                       new Object[] {path, fsDir});
            return err;
        }

        public static SVNErrorMessage ErrorLockExpired(String lockToken, FSFS owner)
        {
            FileInfo fsDir = owner.DBRootDir;
            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.FS_LOCK_EXPIRED,
                                       "Lock has expired:  lock-token ''{0}'' in filesystem ''{1}''",
                                       new Object[] {lockToken, fsDir});
            return err;
        }

        public static SVNErrorMessage ErrorNoUser(FSFS owner)
        {
            FileInfo fsDir = owner.DBRootDir;
            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.FS_NO_USER,
                                       "No username is currently associated with filesystem ''{0}''", fsDir);
            return err;
        }

        public static SVNErrorMessage ErrorLockOwnerMismatch(String username, String lockOwner, FSFS owner)
        {
            FileInfo fsDir = owner.DBRootDir;
            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.FS_LOCK_OWNER_MISMATCH,
                                       "User ''{0}'' is trying to use a lock owned by ''{1}'' in filesystem ''{2}''",
                                       new Object[] {username, lockOwner, fsDir});
            return err;
        }

        public static SVNErrorMessage ErrorNotFile(String path, FSFS owner)
        {
            FileInfo fsDir = owner.DBRootDir;
            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.FS_NOT_FILE, "''{0}'' is not a file in filesystem ''{1}''",
                                       new Object[] {path, fsDir});
            return err;
        }

        public static SVNErrorMessage ErrorPathAlreadyLocked(String path, String owner, FSFS fsfsOwner)
        {
            FileInfo fsDir = fsfsOwner.DBRootDir;
            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.FS_PATH_ALREADY_LOCKED,
                                       "Path ''{0}'' is already locked by user ''{1}'' in filesystem ''{2}''",
                                       new Object[] {path, owner, fsDir});
            return err;
        }

        public static bool IsLockError(SVNErrorMessage err)
        {
            if (err == null)
            {
                return false;
            }
            SVNErrorCode errCode = err.ErrorCode;
            return errCode == SVNErrorCode.FS_PATH_ALREADY_LOCKED || errCode == SVNErrorCode.FS_OUT_OF_DATE;
        }

        public static bool IsUnlockError(SVNErrorMessage err)
        {
            if (err == null)
            {
                return false;
            }
            SVNErrorCode errCode = err.ErrorCode;
            return
                errCode == SVNErrorCode.FS_PATH_NOT_LOCKED || errCode == SVNErrorCode.FS_BAD_LOCK_TOKEN ||
                errCode == SVNErrorCode.FS_LOCK_OWNER_MISMATCH || errCode == SVNErrorCode.FS_NO_SUCH_LOCK ||
                errCode == SVNErrorCode.RA_NOT_LOCKED || errCode == SVNErrorCode.FS_LOCK_EXPIRED;
        }
    }
}