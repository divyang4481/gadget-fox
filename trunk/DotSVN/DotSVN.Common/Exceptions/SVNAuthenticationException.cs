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
using DotSVN.Common.Util;

namespace DotSVN.Common.Exceptions
{
    /// <summary>
    /// An exception class that is used to signal about the fact that errors
    /// occured exactly during an authentication try. Provides the same kind
    /// of information as its base class does.
    /// </summary>
    /// <seealso cref="SVNException">
    /// </seealso>
    [Serializable]
    public class SVNAuthenticationException : SVNException
    {
        /// <summary>
        /// Creates a new authentication exception given detailed error
        /// information and the original cause.
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <param name="cause">Original cause</param>
        public SVNAuthenticationException(SVNErrorMessage errorMessage, Exception cause)
            : base(errorMessage, cause)
        {
        }

        /// <summary>
        /// Creates a new authentication exception given detailed error
        /// information.
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        public SVNAuthenticationException(SVNErrorMessage errorMessage) : base(errorMessage)
        {
        }
    }
}