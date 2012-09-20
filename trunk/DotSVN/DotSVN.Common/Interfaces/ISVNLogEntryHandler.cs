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
    /// <summary>
    /// The <b>ISVNLogEntryHandler</b> interface should be implemented
    /// in order to handle per revision commit information (in a kind of
    /// a revisions history operation)- log entries (represented by
    /// <see cref="SVNLogEntry"/> objects).
    /// </summary>
    public interface ISVNLogEntryHandler
    {
        /// <summary>
        /// Handles a log entry passed.
        /// </summary>
        /// <param name="logEntry">A <see cref="SVNLogEntry"/> object
        /// that represents per revision information (committed paths, log message, etc.)</param>
        void handleLogEntry(SVNLogEntry logEntry);
    }
}