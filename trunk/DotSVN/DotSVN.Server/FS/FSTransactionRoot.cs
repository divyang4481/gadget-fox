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
using System.Text;
using DotSVN.Common.Util;

namespace DotSVN.Server.FS
{
    public class FSTransactionRoot : FSRoot
    {
        private const int MAX_KEY_SIZE = 200;
        public const int SVN_FS_TXN_CHECK_LOCKS = 0x00002;
        public const int SVN_FS_TXN_CHECK_OUT_OF_DATENESS = 0x00001;
        private FileInfo myTxnChangesFile;
        private int myTxnFlags;
        private String myTxnID;
        private FileInfo myTxnRevFile;

        public FSTransactionRoot(FSFS owner, String txnID, int flags) : base(owner)
        {
            myTxnID = txnID;
            myTxnFlags = flags;
        }

        public override FSRevisionNode RootRevisionNode
        {
            get
            {
                if (fsRootRevisionNode == null)
                {
                    FSTransactionInfo txn = Txn;
                    fsRootRevisionNode = Owner.getRevisionNode(txn.RootID);
                }
                return fsRootRevisionNode;
            }
        }

        public virtual FSRevisionNode TxnBaseRootNode
        {
            get
            {
                FSTransactionInfo txn = Txn;
                FSRevisionNode baseRootNode = Owner.getRevisionNode(txn.BaseID);
                return baseRootNode;
            }
        }

        public virtual FSTransactionInfo Txn
        {
            get
            {
                FSID rootID = FSID.CreateTransactionId("0", "0", myTxnID);
                FSRevisionNode revNode = Owner.getRevisionNode(rootID);
                FSTransactionInfo txn = new FSTransactionInfo(revNode.ID, revNode.PredecessorId);
                return txn;
            }
        }

        public override IDictionary<string, FSPathChange> GetChangedPaths()
        {
            FSFile file = Owner.getTransactionChangesFile(myTxnID);
            try
            {
                return fetchAllChanges(file, false);
            }
            finally
            {
                file.Close();
            }
        }

        public virtual int TxnFlags
        {
            get { return myTxnFlags; }

            set { myTxnFlags = value; }
        }

        public virtual String TxnID
        {
            get { return myTxnID; }
        }

        public virtual FileInfo TransactionRevFile
        {
            get
            {
                if (myTxnRevFile == null)
                {
                    myTxnRevFile = new FileInfo(Owner.getTransactionDir(myTxnID).FullName + "\\" + FSFS.TXN_PATH_REV);
                }
                return myTxnRevFile;
            }
        }

        public virtual FileInfo TransactionChangesFile
        {
            get
            {
                if (myTxnChangesFile == null)
                {
                    myTxnChangesFile = new FileInfo(Owner.getTransactionDir(myTxnID).FullName + "\\" + "changes");
                }
                return myTxnChangesFile;
            }
        }

        public override FSCopyInheritance getCopyInheritance(FSParentPath child)
        {
            if (child == null || child.Parent == null || myTxnID == null)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.UNKNOWN, "FATAL error: invalid txn name or child");
                SVNErrorManager.error(err);
                return null;
            }
            FSID childID = child.RevNode.ID;
            FSID parentID = child.Parent.RevNode.ID;
            String childCopyID = childID.CopyID;
            String parentCopyID = parentID.CopyID;

            if (childID.IsTransaction)
            {
                return new FSCopyInheritance(CopyIdInheritanceStyle.Self, null);
            }

            FSCopyInheritance copyInheritance = new FSCopyInheritance(CopyIdInheritanceStyle.Parent, null);

            if (String.CompareOrdinal(childCopyID, "0") == 0)
            {
                return copyInheritance;
            }

            if (String.CompareOrdinal(childCopyID, parentCopyID) == 0)
            {
                return copyInheritance;
            }

            long copyrootRevision = child.RevNode.CopyRootRevision;
            String copyrootPath = child.RevNode.CopyRootPath;

            FSRoot copyrootRoot = Owner.createRevisionRoot(copyrootRevision);
            FSRevisionNode copyrootNode = copyrootRoot.GetRevisionNode(copyrootPath);
            FSID copyrootID = copyrootNode.ID;
            if (copyrootID.CompareTo(childID) == - 1)
            {
                return copyInheritance;
            }

            String idPath = child.RevNode.CreatedPath;
            if (String.CompareOrdinal(idPath, child.AbsPath) == 0)
            {
                copyInheritance.Style = CopyIdInheritanceStyle.Self;
                return copyInheritance;
            }

            copyInheritance.Style = CopyIdInheritanceStyle.New;
            copyInheritance.CopySourcePath = idPath;
            return copyInheritance;
        }

        public virtual IDictionary<string, string> unparseDirEntries(IDictionary<string, FSEntry> entries)
        {
            IDictionary<string, string> unparsedEntries = new Dictionary<string, string>();
            foreach (string name in entries.Keys)
            {
                FSEntry fsEntry = entries[name];
                unparsedEntries[name] = fsEntry.ToString();
            }
            return unparsedEntries;
        }

        private static String createTxnDir(long revision, FSFS owner)
        {
            FileInfo parent = owner.TransactionsParentDir;

            for (int i = 1; i < 99999; i++)
            {
                FileInfo uniquePath =
                    new FileInfo(Path.Combine(parent.FullName, revision + "-" + i + FSFS.TXN_PATH_EXT));
                if (!Directory.Exists(uniquePath.FullName) && Directory.CreateDirectory(uniquePath.FullName).Exists)
                {
                    return revision + "-" + i;
                }
            }
            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.IO_UNIQUE_NAMES_EXHAUSTED,
                                       string.Format(
                                           "Unable to create transaction directory in '{0}' for revision {1}", parent,
                                           revision));
            SVNErrorManager.error(err);
            return null;
        }

        public virtual String[] readNextIDs()
        {
            String[] ids = new String[2];
            String idsToParse;
            FSFile idsFile = new FSFile(Owner.getNextIDsFile(myTxnID));

            try
            {
                idsToParse = idsFile.ReadLine(MAX_KEY_SIZE*2 + 3);
            }
            finally
            {
                idsFile.Close();
            }

            int delimiterInd = idsToParse.IndexOf(' ');

            if (delimiterInd == - 1)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "next-ids file corrupt");
                SVNErrorManager.error(err);
            }

            ids[0] = idsToParse.Substring(0, (delimiterInd) - (0));
            ids[1] = idsToParse.Substring(delimiterInd + 1);
            return ids;
        }

        public virtual FileInfo getTransactionRevNodePropsFile(FSID id)
        {
            return
                new FileInfo(Owner.getTransactionDir(id.TransactionID).FullName + "\\" + FSFS.PATH_PREFIX_NODE +
                             id.NodeID + "." +
                             id.CopyID + FSFS.TXN_PATH_EXT_PROPS);
        }

        public virtual FileInfo getTransactionRevNodeChildrenFile(FSID id)
        {
            return
                new FileInfo(Owner.getTransactionDir(id.TransactionID).FullName + "\\" + FSFS.PATH_PREFIX_NODE +
                             id.NodeID + "." +
                             id.CopyID + FSFS.TXN_PATH_EXT_CHILDREN);
        }

        public static String generateNextKey(String oldKey)
        {
            char[] nextKey = new char[oldKey.Length + 1];
            bool carry = true;
            if (oldKey.Length > 1 && oldKey[0] == '0')
            {
                return null;
            }
            for (int i = oldKey.Length - 1; i >= 0; i--)
            {
                char c = oldKey[i];
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z')))
                {
                    return null;
                }
                if (carry)
                {
                    if (c == 'z')
                    {
                        nextKey[i] = '0';
                    }
                    else
                    {
                        carry = false;
                        if (c == '9')
                        {
                            nextKey[i] = 'a';
                        }
                        else
                        {
                            nextKey[i] = (char) (c + 1);
                        }
                    }
                }
                else
                {
                    nextKey[i] = c;
                }
            }
            int nextKeyLength = oldKey.Length + (carry ? 1 : 0);
            if (nextKeyLength >= MAX_KEY_SIZE)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.UNKNOWN,
                                           string.Format(
                                               "FATAL error: new key length is greater than the threshold {0}",
                                               MAX_KEY_SIZE));
                SVNErrorManager.error(err);
            }
            if (carry)
            {
                Array.Copy(nextKey, 0, nextKey, 1, oldKey.Length);
                nextKey[0] = '1';
            }
            return new String(nextKey, 0, nextKeyLength);
        }

        private static String addKeys(String key1, String key2)
        {
            int i1 = key1.Length - 1;
            int i2 = key2.Length - 1;
            int carry = 0;
            StringBuilder result = new StringBuilder();

            while (i1 >= 0 || i2 >= 0 || carry > 0)
            {
                int val = carry;

                if (i1 >= 0)
                {
                    val += (key1[i1] <= '9' ? key1[i1] - '0' : key1[i1] - 'a' + 10);
                }

                if (i2 >= 0)
                {
                    val += (key2[i2] <= '9' ? key2[i2] - '0' : key2[i2] - 'a' + 10);
                }

                carry = val/36;
                val = val%36;

                char sym = val <= 9 ? (char) ('0' + val) : (char) (val - 10 + 'a');
                result.Append(sym);

                if (i1 >= 0)
                {
                    --i1;
                }

                if (i2 >= 0)
                {
                    --i2;
                }
            }
            return SVNUtil.ReverseString(result.ToString());
        }
    }
}