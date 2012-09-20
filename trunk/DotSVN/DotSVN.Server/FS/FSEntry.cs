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
using DotSVN.Common;
using DotSVN.Server.FS;

namespace DotSVN.Server.FS
{
    /// <summary>
    /// Repesents and entry in FileSystem
    /// </summary>
    public class FSEntry : IComparable
    {
        private FSID id;
        private SVNNodeKind type;
        private String name;

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public virtual FSID Id
        {
            get { return id; }

            set { id = value; }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public virtual SVNNodeKind Type
        {
            get { return type; }

            set { type = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual String Name
        {
            get { return name; }

            set { name = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FSEntry"/> class.
        /// </summary>
        public FSEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FSEntry"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        public FSEntry(FSID id, SVNNodeKind type, String name)
        {
            this.id = id;
            this.type = type;
            this.name = name;
        }

        /// <summary>
        /// Gives a string representation of FS entry
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return type + " " + id;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than obj. Zero This instance is equal to obj. Greater than zero This instance is greater than obj.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">obj is not the same type as this instance. </exception>
        public virtual int CompareTo(Object obj)
        {
            if (obj == this)
            {
                return 0;
            }
            if (obj == null || obj.GetType() != typeof (FSEntry))
            {
                return 1;
            }
            FSEntry entry = (FSEntry) obj;
            return String.CompareOrdinal(name.ToLower(), entry.name.ToLower());
        }
    }
}