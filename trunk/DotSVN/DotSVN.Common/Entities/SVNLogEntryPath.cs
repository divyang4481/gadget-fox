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
    /// <summary> The <c>SVNLogEntryPath</c> class encapsulates information about a single 
    /// item changed in a revision. This information includes an item's path, a 
    /// type of the changes made to the item, and if the item is a copy of another
    /// one - information about the item's ancestor. 
    /// 
    /// <para><c>SVNLogEntryPath</c> objects are held by an <see cref="SVNLogEntry"/> object - 
    /// they are representations of all the changed paths in the revision represented
    /// by that <see cref="SVNLogEntry"/> object.</para> 
    /// </summary>
    /// <seealso cref="SVNLogEntry">
    /// </seealso>
    [Serializable]
    public class SVNLogEntryPath
    {
        #region Constants

        /// <summary> Char 'A' (item added).</summary>
        public const char TYPE_ADDED = 'A';

        /// <summary> Char 'D' (item deleted).</summary>
        public const char TYPE_DELETED = 'D';

        /// <summary> Char 'M' (item modified).</summary>
        public const char TYPE_MODIFIED = 'M';

        /// <summary> Char 'R' (item replaced).</summary>
        public const char TYPE_REPLACED = 'R';

        #endregion

        private String copyPath;
        private long copyRevision;
        private String path;
        private char type;

        /// <summary>
        /// Constructs an <c>SVNLogEntryPath</c> object.
        /// <para>
        /// Use char constants of this class as a change type to pass to this constructor.</para>
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="type">The type.</param>
        /// <param name="copyPath">The copy path.</param>
        /// <param name="copyRevision">The copy revision.</param>
        public SVNLogEntryPath(String path, char type, String copyPath, long copyRevision)
        {
            this.path = path;
            this.type = type;
            this.copyPath = copyPath;
            this.copyRevision = copyRevision;
        }

        /// <summary>
        /// Gets or sets the type of the change applied to the item represented by this
        /// object. This type can be one of the following:
        /// <list>
        ///     <item>'M' - Modified</item>
        ///     <item>'A' - Added</item>
        ///     <item>'D' - Deleted</item>
        ///     <item>'R' - Replaced (means that the object is first deleted, 
        ///     then another object of the same name is added, all within a single revision).</item>
        /// </list>
        /// </summary>
        /// <value>The change type</value>
        /// <returns> a type of the change as a char label
        /// </returns>
        public char ChangeType
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Returns the path of the ancestor of the item represented by this object.
        /// </summary>
        /// <returns>
        /// the origin path from where the item, represented by this
        /// object, was copied, or <c>null</c> if it wasn't copied
        /// </returns>
        public virtual String CopyPath
        {
            get { return copyPath; }
            set { copyPath = value; }
        }

        /// <summary>
        /// Returns the revision of the ancestor of the item represented by this object.
        /// </summary>
        /// <returns>
        /// the revision of the origin path from where the item,
        /// represented by this object, was copied, or -1 if the item was not copied
        /// </returns>
        public virtual long CopyRevision
        {
            get { return copyRevision; }
            set { copyRevision = value; }
        }

        /// <summary>
        /// Gets or sets the path of the item represented by this object.
        /// </summary>
        /// <returns>
        /// The changed path represented by this object
        /// </returns>
        public virtual String Path
        {
            get { return path; }
            set { path = value;}
        }

        /// <summary>
        /// Calculates and returns a hash code for this object.
        /// </summary>
        /// <returns>a hash code</returns>
        public override int GetHashCode()
        {
            int PRIME = 31;
            int result = 1;
            result = PRIME * result + ((path == null) ? 0 : path.GetHashCode());
            result = PRIME * result + type;
            result = PRIME * result + ((copyPath == null) ? 0 : copyPath.GetHashCode());
            result = PRIME * result + (int) (copyRevision ^ (copyRevision >> 32));
            return result;
        }

        /// <summary>
        /// Compares this object with another one.
        /// </summary>
        /// <param name="obj">an object to compare with</param>
        /// <returns><code>true</code> if this object is the same as the <code>obj</code> argument
        /// </returns>
        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || !(obj is SVNLogEntryPath))
            {
                return false;
            }
            SVNLogEntryPath other = (SVNLogEntryPath) obj;
            return
                copyRevision == other.copyRevision && type == other.type &&
                SVNLogEntry.compare(path, other.path) && SVNLogEntry.compare(copyPath, other.copyPath);
        }

        /// <summary>
        /// Gives a string representation of this oobject.
        /// </summary>
        /// <returns>a string representing this object</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(type);
            result.Append(' ');
            result.Append(path);
            if (copyPath != null)
            {
                result.Append("(from ");
                result.Append(copyPath);
                result.Append(':');
                result.Append(copyRevision);
                result.Append(')');
            }
            return result.ToString();
        }
    }
}