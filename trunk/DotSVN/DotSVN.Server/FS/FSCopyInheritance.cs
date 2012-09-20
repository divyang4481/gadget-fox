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
    // Copy id inheritance style
    public enum CopyIdInheritanceStyle
    {
        Unknown = 0,
        Self,
        Parent,
        New
    }

    public class FSCopyInheritance
    {
        private String copySourcePath;
        private CopyIdInheritanceStyle style;

        /// <summary>
        /// Initializes a new instance of the <see cref="FSCopyInheritance"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="path">The path.</param>
        public FSCopyInheritance(CopyIdInheritanceStyle style, String path)
        {
            this.style = style;
            copySourcePath = path;
        }

        /// <summary>
        /// Gets or sets the copy source path.
        /// </summary>
        /// <value>The copy source path.</value>
        public String CopySourcePath
        {
            get { return copySourcePath; }
            set { copySourcePath = value; }
        }

        /// <summary>
        /// Gets or sets the copy id inheritance style.
        /// </summary>
        /// <value>The copy id inheritance style.</value>
        public CopyIdInheritanceStyle Style
        {
            get { return style; }
            set { style = value; }
        }
    }
}