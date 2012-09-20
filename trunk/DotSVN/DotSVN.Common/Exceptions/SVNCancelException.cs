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
    /// The <c>SVNCancelException</c> is used to signal about an operation cancel event.
    /// </summary>
    /// <seealso cref="SVNException"/>
    [Serializable]
    public class SVNCancelException : SVNException
    {
        /// <summary>
        /// Creates a cancel exception.
        /// </summary>
        public SVNCancelException() : base(SVNErrorMessage.create(SVNErrorCode.CANCELLED, "Operation cancelled"))
        {
        }

        /// <summary>
        /// Constructs an <c>SVNCancelException</c> given the error message.
        /// </summary>
        /// <param name="errorMessage">An error message describing why the operation was cancelled</param>
        public SVNCancelException(SVNErrorMessage errorMessage)
            : base(errorMessage)
        {
        }
    }
}