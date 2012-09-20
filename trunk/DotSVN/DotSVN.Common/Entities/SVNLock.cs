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
using System.Text;

namespace DotSVN.Common.Entities
{
    /// <summary>
    /// The <c>SVNLock</c> class represents a file lock. It holds information about 
    /// a lock path, token, owner, comment, creation and expiration dates.
    /// </summary>
    public class SVNLock
    {
        private readonly String comment;
        private readonly DateTime creationDate;
        private readonly DateTime expirationDate;
        private readonly String id;
        private readonly String owner;
        private readonly String path;

        /// <summary>
        /// Constructs an <c>SVNLock</c> object.
        /// </summary>
        /// <param name="path">a file path, relative to the repository root directory</param>
        /// <param name="id">a string token identifying the lock</param>
        /// <param name="owner">the owner of the lock</param>
        /// <param name="comment">a comment message for the lock (optional)</param>
        /// <param name="created">a datestamp when the lock was created</param>
        /// <param name="expires">a datestamp when the lock expires, i.e. the file is
        /// unlocked (optional)</param>
        public SVNLock(String path, String id, String owner, String comment, DateTime created, DateTime expires)
        {
            this.path = path;
            this.id = id;
            this.owner = owner;
            this.comment = comment;
            creationDate = created;
            expirationDate = expires;
        }

        /// <summary>
        /// Gets the lock comment.
        /// </summary>
        /// <value>The comment.</value>
        /// <returns>The lock comment message
        /// </returns>
        public String Comment
        {
            get { return comment; }
        }

        /// <summary>
        /// Gets the creation datestamp of this lock.
        /// </summary>
        /// <value>The creation date.</value>
        /// <returns>A <see cref="DateTime"/> representing the moment in time when this lock was created</returns>
        public DateTime CreationDate
        {
            get { return creationDate; }
        }

        /// <summary>
        /// Gets the expiration datestamp of this lock.
        /// </summary>
        /// <value>The expiration date.</value>
        /// <returns>A <see cref="DateTime"/>  representing the moment in time when the this lock expires</returns>
        public DateTime ExpirationDate
        {
            get { return expirationDate; }
        }

        /// <summary>
        /// Gets the lock token.
        /// </summary>
        /// <value>The ID.</value>
        /// <returns>A unique string identifying this lock</returns>
        public String ID
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the lock owner.
        /// </summary>
        /// <value>The owner.</value>
        /// <returns>The owner of this lock</returns>
        public String Owner
        {
            get { return owner; }
        }

        /// <summary>
        /// Gets the path of the file for which this lock was created.
        /// The path is relative to the repository root directory.
        /// </summary>
        /// <value>The path.</value>
        /// <returns>The path of the locked file</returns>
        public String Path
        {
            get { return path; }
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>
        /// A string representation of this lock object
        /// </returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("path=");
            result.Append(path);
            result.Append(", token=");
            result.Append(id);
            result.Append(", owner=");
            result.Append(owner);
            if (comment != null)
            {
                result.Append(", comment=");
                result.Append(comment);
            }
            result.Append(", created=");
            result.Append(creationDate);
            result.Append(", expires=");
            result.Append(expirationDate);
            return result.ToString();
        }
    }
}