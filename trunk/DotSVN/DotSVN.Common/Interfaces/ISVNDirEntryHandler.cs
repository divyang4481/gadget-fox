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

using DotSVN.Common.Entities;

namespace DotSVN.Common.Interfaces
{
    /// <summary> The <see cref="ISVNDirEntryHandler"/> interface is used to handle information
    /// about directory entries while retrieving dir contents.    
    /// </summary>
    public interface ISVNDirEntryHandler
    {
        /// <summary>
        /// Handles a directory entry passed.
        /// </summary>
        /// <param name="dirEntry">The dir entry.</param>
        /// <seealso cref="SVNDirEntry">
        /// </seealso>
        void handleDirEntry(SVNDirEntry dirEntry);
    }
}