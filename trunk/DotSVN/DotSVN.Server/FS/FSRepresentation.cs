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
    public class FSRepresentation
    {
        #region Constants

        public const string REP_DELTA = "DELTA";
        public const string REP_PLAIN = "PLAIN";
        public const string REP_TRAILER = "ENDREP";

        #endregion

        #region Private fields

        private long expandedSize;
        private string hexDigest;
        private long offset;
        private long revision;
        private long size;
        private string transactionId;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FSRepresentation"/> class.
        /// </summary>
        /// <param name="revision">The revision.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="expandedSize">Size of the expanded.</param>
        /// <param name="hexDigest">The hex digest.</param>
        public FSRepresentation(long revision, long offset, long size, long expandedSize, String hexDigest)
        {
            this.revision = revision;
            this.offset = offset;
            this.size = size;
            this.expandedSize = expandedSize;
            this.hexDigest = hexDigest;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FSRepresentation"/> class.
        /// </summary>
        /// <param name="representation">The representation.</param>
        public FSRepresentation(FSRepresentation representation)
        {
            revision = representation.Revision;
            offset = representation.Offset;
            size = representation.Size;
            expandedSize = representation.ExpandedSize;
            hexDigest = representation.HexDigest;
            transactionId = representation.transactionId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FSRepresentation"/> class.
        /// </summary>
        public FSRepresentation()
        {
            revision = FSRepository.SVN_INVALID_REVNUM;
            offset = - 1;
            size = - 1;
            expandedSize = - 1;
            hexDigest = null;
        }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        /// <value>The revision.</value>
        public long Revision
        {
            get { return revision; }
            set { revision = value; }
        }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public long Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public long Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Gets or sets the size of the expanded.
        /// </summary>
        /// <value>The size of the expanded.</value>
        public long ExpandedSize
        {
            get { return expandedSize; }
            set { expandedSize = value; }
        }

        /// <summary>
        /// Gets or sets the hex digest.
        /// </summary>
        /// <value>The hex digest.</value>
        public string HexDigest
        {
            get { return hexDigest; }
            set { hexDigest = value; }
        }

        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        /// <value>The transaction id.</value>
        public string TransactionId
        {
            get { return transactionId; }
            set { transactionId = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this transaction valid.
        /// </summary>
        /// <value><c>true</c> if this transaction valid; otherwise, <c>false</c>.</value>
        public bool IsTransaction
        {
            get { return transactionId != null; }
        }

        /// <summary>
        /// Compares two FS representations.
        /// </summary>
        /// <param name="r1">The r1.</param>
        /// <param name="r2">The r2.</param>
        /// <returns></returns>
        public static bool CompareRepresentations(FSRepresentation r1, FSRepresentation r2)
        {
            if (r1 == r2)
            {
                return true;
            }
            else if (r1 == null)
            {
                return false;
            }
            return r1.Equals(r2);
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
            if (obj == null || obj.GetType() != typeof (FSRepresentation))
            {
                return false;
            }
            FSRepresentation rep = (FSRepresentation) obj;
            return revision == rep.Revision && offset == rep.Offset;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return revision + " " + offset + " " + size + " " + expandedSize + " " + hexDigest;
        }

    }
}