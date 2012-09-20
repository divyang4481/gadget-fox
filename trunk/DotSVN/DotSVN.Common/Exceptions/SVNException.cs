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
    /// Common exception class - All DotSVN exceptions derive from this
    /// </summary>
    public class SVNException : Exception
    {
        private SVNErrorMessage errorMessage = null;

        /// <summary>
        /// Creates an exception given an error message and the cause exception.
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        public SVNException(SVNErrorMessage errorMessage)
            : base(errorMessage.Message)
        {
        }

        /// <summary>
        /// Creates an exception given an error message and the cause exception.
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <param name="cause">The real cause of the error</param>
        public SVNException(SVNErrorMessage errorMessage, Exception cause)
            : base(errorMessage.Message, cause)
        {
        }

        /// <summary> Returns an error message provided to this exception object.
        /// 
        /// </summary>
        /// <returns> an error message that contains details on the error
        /// </returns>
        public SVNErrorMessage ErrorMessage
        {
            get { return errorMessage; }
        }

        /// <summary> Returns the informational message describing the cause
        /// of this exception.
        /// 
        /// </summary>
        /// <returns> an informational message
        /// </returns>
        public override String Message
        {
            get
            {
                SVNErrorMessage error = ErrorMessage;
                if (error != null)
                {
                    return error.FullMessage;
                }
                return base.Message;
            }
        }
    }
}