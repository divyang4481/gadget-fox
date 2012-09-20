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

namespace DotSVN.Server.FS
{
    public class FSTransactionInfo
    {
        private FSID baseID;
        private long baseRevision;
        private FSID rootID;
        private String transactionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="FSTransactionInfo"/> class.
        /// </summary>
        /// <param name="revision">The revision.</param>
        /// <param name="id">The id.</param>
        public FSTransactionInfo(long revision, String id)
        {
            baseRevision = revision;
            transactionId = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FSTransactionInfo"/> class.
        /// </summary>
        /// <param name="rootID">The root ID.</param>
        /// <param name="baseID">The base ID.</param>
        public FSTransactionInfo(FSID rootID, FSID baseID)
        {
            this.rootID = rootID;
            this.baseID = baseID;
            transactionId = this.rootID.TransactionID;
            baseRevision = this.baseID.Revision;
        }

        public long BaseRevision
        {
            get { return baseRevision; }

            set { baseRevision = value; }
        }

        public String TransactionId
        {
            get { return transactionId; }

            set { transactionId = value; }
        }

        public virtual FSID BaseID
        {
            get { return baseID; }
        }

        public virtual FSID RootID
        {
            get { return rootID; }
        }
    }
}