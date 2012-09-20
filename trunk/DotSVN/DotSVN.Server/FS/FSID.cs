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
using DotSVN.Server.RepositoryAccess.File;

namespace DotSVN.Server.FS
{
    /// <summary>
    /// Encapsulated an FS ID
    /// </summary>
    [Serializable]
    public class FSID
    {
        #region Private fields

        private String copyID;
        private String nodeID;
        private long offset;
        private long revision;
        private String transactionID;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FSID"/> class.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="txnId">The TXN id.</param>
        /// <param name="copyId">The copy id.</param>
        /// <param name="revision">The revision.</param>
        /// <param name="offset">The offset.</param>
        private FSID(String nodeId, String txnId, String copyId, long revision, long offset)
        {
            nodeID = nodeId;
            copyID = copyId;
            transactionID = txnId;
            this.revision = revision;
            this.offset = offset;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="FSID"/> is TXN.
        /// </summary>
        /// <value><c>true</c> if TXN; otherwise, <c>false</c>.</value>
        public bool IsTransaction
        {
            get { return transactionID != null; }
        }

        /// <summary>
        /// Gets the node ID.
        /// </summary>
        /// <value>The node ID.</value>
        public String NodeID
        {
            get { return nodeID; }
        }

        /// <summary>
        /// Gets the Transaction ID.
        /// </summary>
        /// <value>The Transaction ID.</value>
        public String TransactionID
        {
            get { return transactionID; }
        }

        /// <summary>
        /// Gets the copy ID.
        /// </summary>
        /// <value>The copy ID.</value>
        public String CopyID
        {
            get { return copyID; }
        }

        /// <summary>
        /// Gets the revision.
        /// </summary>
        /// <value>The revision.</value>
        public long Revision
        {
            get { return revision; }
        }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public long Offset
        {
            get { return offset; }
        }

        /// <summary>
        /// Creates the transaction ID.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="copyId">The copy id.</param>
        /// <param name="txnId">The TXN id.</param>
        /// <returns></returns>
        public static FSID CreateTransactionId(String nodeId, String copyId, String txnId)
        {
            return new FSID(nodeId, txnId, copyId, FSRepository.SVN_INVALID_REVNUM, -1);
        }

        /// <summary>
        /// Creates the revision id.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <param name="copyId">The copy id.</param>
        /// <param name="revision">The revision.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public static FSID CreateRevisionId(String nodeId, String copyId, long revision, long offset)
        {
            return new FSID(nodeId, null, copyId, revision, offset);
        }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns></returns>
        public FSID Copy()
        {
            return new FSID(NodeID, TransactionID, CopyID, Revision, Offset);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(Object obj)
        {
            if (obj == null || obj.GetType() != typeof(FSID))
            {
                return false;
            }
            FSID id = (FSID)obj;
            if (this == id)
            {
                return true;
            }

            if (nodeID != null && !nodeID.Equals(id.NodeID))
            {
                return false;
            }
            else if (nodeID == null && id.NodeID != null)
            {
                return false;
            }

            if (copyID != null && !copyID.Equals(id.CopyID))
            {
                return false;
            }
            else if (copyID == null && id.CopyID != null)
            {
                return false;
            }

            if (transactionID != null && !transactionID.Equals(id.TransactionID))
            {
                return false;
            }
            else if (transactionID == null && id.TransactionID != null)
            {
                return false;
            }

            if (revision != id.Revision || offset != id.Offset)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            int PRIME = 31;
            int result = 1;
            result = PRIME * result + ((nodeID == null) ? 0 : nodeID.GetHashCode());
            result = PRIME * result + ((copyID == null) ? 0 : copyID.GetHashCode());
            result = PRIME * result + ((transactionID == null) ? 0 : transactionID.GetHashCode());
            result = PRIME * result + (int)(revision ^ (revision >> 32));
            result = PRIME * result + (int)(offset ^ (offset >> 32));
            return result;
        }

        /*
		* Return values: 0 - id1 equals to id2 1 - id1 is related to id2 (id2 is a
		* result of user's modifications) -1 - id1 is not related to id2
		* (absolutely different items)
		*/

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="otherID">The other ID.</param>
        /// <returns></returns>
        public int CompareTo(FSID otherID)
        {
            if (otherID == null)
            {
                return -1;
            }
            else if (otherID.Equals(this))
            {
                return 0;
            }
            return IsRelated(otherID) ? 1 : -1;
        }

        /// <summary>
        /// Determines whether the specified other ID is related.
        /// </summary>
        /// <param name="otherID">The other ID.</param>
        /// <returns>
        /// 	<c>true</c> if the specified other ID is related; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRelated(FSID otherID)
        {
            if (otherID == null)
            {
                return false;
            }

            if (this == otherID)
            {
                return true;
            }

            if (nodeID != null && nodeID.StartsWith("_"))
            {
                if (transactionID != null && !transactionID.Equals(otherID.TransactionID))
                {
                    return false;
                }
                else if (transactionID == null && otherID.TransactionID != null)
                {
                    return false;
                }
            }
            return nodeID.Equals(otherID.NodeID);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return nodeID + "." + copyID + "." + (IsTransaction ? "t" + transactionID : "r" + revision + "/" + offset);
        }

        /// <summary>
        /// Froms the string.
        /// </summary>
        /// <param name="revNodeId">The rev node id.</param>
        /// <returns></returns>
        public static FSID FromString(String revNodeId)
        {
            int dotInd = revNodeId.IndexOf('.');
            if (dotInd == -1)
            {
                return null;
            }

            String nodeId = revNodeId.Substring(0, (dotInd) - (0));
            revNodeId = revNodeId.Substring(dotInd + 1);

            dotInd = revNodeId.IndexOf('.');
            if (dotInd == -1)
            {
                return null;
            }

            String copyId = revNodeId.Substring(0, (dotInd) - (0));
            revNodeId = revNodeId.Substring(dotInd + 1);

            if (revNodeId[0] == 'r')
            {
                int slashInd = revNodeId.IndexOf('/');
                long rev = -1;
                long offset = -1;
                try
                {
                    rev = Int64.Parse(revNodeId.Substring(1, (slashInd) - (1)));
                    offset = Int64.Parse(revNodeId.Substring(slashInd + 1));
                }
                catch (FormatException)
                {
                    return null;
                }
                return CreateRevisionId(nodeId, copyId, rev, offset);
            }
            else if (revNodeId[0] == 't')
            {
                String txnId = revNodeId.Substring(1);
                return CreateTransactionId(nodeId, copyId, txnId);
            }
            return null;
        }
    }
}