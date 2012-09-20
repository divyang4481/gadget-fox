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
using System.Text;
using DotSVN.Common.Interfaces;

namespace DotSVN.Common.Entities
{
    /// <summary>
    /// The <c>SVNLogEntry</c> class encapsulates such per revision information as:
    /// a revision number, the datestamp when the revision was committed, the author
    /// of the revision, a commit log message and all paths changed in that revision.
    /// </summary>
    /// <seealso cref="SVNLogEntryPath">
    /// </seealso>
    /// <seealso cref="ISVNLogEntryHandler">
    /// </seealso>
    [Serializable]
    public class SVNLogEntry
    {
        private readonly String author;
        private readonly IDictionary changedPaths;
        private DateTime date;
        private readonly String message;
        private readonly long revision;

        /// <summary>
        /// Constructs an <c>SVNLogEntry</c> object.
        /// </summary>
        /// <param name="changedPaths">a map collection which keys are all the paths that were changed in
        /// <code>revision</code>, and values are <c>SVNLogEntryPath</c> representation objects</param>
        /// <param name="revision">a revision number</param>
        /// <param name="author">the author of <code>revision</code></param>
        /// <param name="date">the datestamp when the revision was committed</param>
        /// <param name="message">an commit log message for <code>revision</code></param>
        /// <seealso cref="SVNLogEntryPath">
        /// </seealso>
        public SVNLogEntry(IDictionary changedPaths, long revision, String author, ref DateTime date, String message)
        {
            this.revision = revision;
            this.author = author;
            this.date = date;
            this.message = message;
            this.changedPaths = changedPaths;
        }

        /// <summary>
        /// Gets a map containing all the paths that were changed in the revision that this object represents.
        /// </summary>
        /// <value>The changed paths.</value>
        /// <returns>A map which keys are all the paths that were changed and values are <c>SVNLogEntryPath</c> objects.
        /// </returns>
        public virtual IDictionary ChangedPaths
        {
            get { return changedPaths; }
        }

        /// <summary>
        /// Returns the author of the revision that this object represents.
        /// </summary>
        /// <value>The author.</value>
        /// <returns>The author of the revision
        /// </returns>
        public virtual String Author
        {
            get { return author; }
        }

        /// <summary>
        /// Gets the datestamp when the revision was committed.
        /// </summary>
        /// <value>The date.</value>
        /// <returns>The moment in time when the revision was committed</returns>
        public virtual DateTime Date
        {
            get { return date; }
        }

        /// <summary>
        /// Gets the log message attached to the revision.
        /// </summary>
        /// <value>The message.</value>
        /// <returns>The commit log message</returns>
        public virtual String Message
        {
            get { return message; }
        }

        /// <summary>
        /// Gets the number of the revision that this object represents.
        /// </summary>
        /// <value>The revision.</value>
        /// <returns>A revision number</returns>
        public virtual long Revision
        {
            get { return revision; }
        }

        /// <summary>
        /// Calculates and returns a hash code for this object.
        /// </summary>
        /// <returns>a hash code</returns>
        public override int GetHashCode()
        {
            int PRIME = 31;
            int result = 1;
            result = PRIME * result + (int)(revision ^ (revision >> 32));
            result = PRIME * result + ((author == null) ? 0 : author.GetHashCode());
            int dateHashCode = date.GetHashCode();
            result = PRIME * result + dateHashCode;
            result = PRIME * result + ((message == null) ? 0 : message.GetHashCode());
            result = PRIME * result + ((changedPaths == null) ? 0 : changedPaths.GetHashCode());
            return result;
        }

        /// <summary>
        /// Compares this object with another one.
        /// </summary>
        /// <param name="obj">an object to compare with</param>
        /// <returns><c>true</span> if this object is the same as the <code>obj</code> argument
        /// </returns>
        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            SVNLogEntry other = (SVNLogEntry) obj;
            return
                revision == other.revision && compare(author, other.author) &&
                compare(message, other.message) && compare(date, other.date) &&
                compare(changedPaths, other.changedPaths);
        }

        /// <summary>
        /// Gives a string representation of this oobject.
        /// </summary>
        /// <returns>a string representing this object</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(revision);
            if (date != null)
            {
                result.Append(' ');
                result.Append(date);
            }
            if (author != null)
            {
                result.Append(' ');
                result.Append(author);
            }
            if (message != null)
            {
                result.Append('\n');
                result.Append(message);
            }
            if (changedPaths != null && !(changedPaths.Count == 0))
            {
                for (IEnumerator paths = changedPaths.Values.GetEnumerator(); paths.MoveNext();)
                {
                    result.Append('\n');
                    SVNLogEntryPath path = (SVNLogEntryPath) paths.Current;
                    result.Append(path.ToString());
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Compares two objects.
        /// </summary>
        /// <param name="o1">the first object to compare</param>
        /// <param name="o2">the second object to compare</param>
        /// <returns><c>true</c> if either both <code>o1</code> and <code>o2</code> are <c>null</c>
        /// or <code>o1.equals(o2)</code> returns <c>true</c>
        /// </returns>
        internal static bool compare(Object o1, Object o2)
        {
            if (o1 == null)
            {
                return o2 == null;
            }
            return o1.Equals(o2);
        }
    }
}