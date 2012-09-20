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

namespace DotSVN.Server.FS
{
    public class FSParentPath
    {
        private FSCopyInheritance copyInheritance;
        private String entryName;

        private FSParentPath parent;
        private FSRevisionNode revNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="FSParentPath"/> class.
        /// </summary>
        /// <param name="newParentPath">The new parent path.</param>
        public FSParentPath(FSParentPath newParentPath)
        {
            revNode = newParentPath.revNode;
            entryName = newParentPath.entryName;
            parent = newParentPath.parent;
            copyInheritance = newParentPath.copyInheritance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FSParentPath"/> class.
        /// </summary>
        /// <param name="newRevNode">The new rev node.</param>
        /// <param name="newEntry">The new entry.</param>
        /// <param name="newParentPath">The new parent path.</param>
        public FSParentPath(FSRevisionNode newRevNode, String newEntry, FSParentPath newParentPath)
        {
            revNode = newRevNode;
            entryName = newEntry;
            parent = newParentPath;
            if (newRevNode != null)
            {
                copyInheritance =
                    new FSCopyInheritance(CopyIdInheritanceStyle.Unknown, newRevNode.CopyFromPath);
            }
            else
            {
                copyInheritance = new FSCopyInheritance(CopyIdInheritanceStyle.Unknown, null);
            }
        }

        /// <summary>
        /// Sets the parent path.
        /// </summary>
        /// <param name="newRevNode">The new rev node.</param>
        /// <param name="newEntry">The new entry.</param>
        /// <param name="newParentPath">The new parent path.</param>
        public void setParentPath(FSRevisionNode newRevNode, String newEntry, FSParentPath newParentPath)
        {
            revNode = newRevNode;
            entryName = newEntry;
            parent = newParentPath;
            copyInheritance = new FSCopyInheritance(CopyIdInheritanceStyle.Unknown, null);
        }

        #region Properties

        public FSRevisionNode RevNode
        {
            get { return revNode; }

            set { revNode = value; }
        }

        public String NameEntry
        {
            get { return entryName; }

            set { entryName = value; }
        }

        public FSParentPath Parent
        {
            get { return parent; }
        }

        public CopyIdInheritanceStyle CopyStyle
        {
            get { return copyInheritance.Style; }

            set { copyInheritance.Style = value; }
        }

        public String CopySourcePath
        {
            get { return copyInheritance.CopySourcePath; }

            set { copyInheritance.CopySourcePath = value; }
        }

        public string AbsPath
        {
            get
            {
                string pathSoFar = "/";
                if (parent != null)
                {
                    pathSoFar = parent.AbsPath;
                }
                string absPath = (NameEntry == null) ? pathSoFar : Path.Combine(pathSoFar, NameEntry);
                return absPath;
            }
        }

        #endregion
    }
}