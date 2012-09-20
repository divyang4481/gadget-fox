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
using DotSVN.Common.Exceptions;
using log4net;

namespace DotSVN.Common.Util
{
    /// <summary>
    /// Error manager class
    /// </summary>
    public class SVNErrorManager
    {
        // Create a logger for use in this class
        
        private static readonly ILog log = LogManager.GetLogger(typeof(SVNErrorManager));
        private static readonly bool isErrorEnabled = log.IsErrorEnabled;
        private static readonly bool isInfoEnabled = log.IsInfoEnabled;

        /// <summary>
        /// Creates and throws an <see cref="SVNCancelException"/> with the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Cancel(String message)
        {
            if (isInfoEnabled)
                log.Info(message);

            throw new SVNCancelException(SVNErrorMessage.create(SVNErrorCode.CANCELLED, message));
        }

        /// <summary>
        /// Creates and throws an <see cref="SVNAuthenticationException"/> with the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messageObject">The message object.</param>
        public static void AuthenticationFailed(String message, Object messageObject)
        {
            SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.RA_NOT_AUTHORIZED, message, messageObject);
            if (isErrorEnabled)
                log.Error(err.Message);
            throw new SVNAuthenticationException(err);
        }

        /// <summary>
        /// Creates and throws an <seealso cref="SVNException"/> based on the <see cref="SVNErrorMessage"/>.
        /// </summary>
        /// <param name="err">The <see cref="SVNErrorMessage"/> to be used.</param>
        public static void error(SVNErrorMessage err)
        {
            if (err == null)
            {
                err = SVNErrorMessage.create(SVNErrorCode.UNKNOWN);
            }
            if (isInfoEnabled)
                log.Info(err.FullMessage);

            if (err.ErrorCode == SVNErrorCode.CANCELLED)
            {
                throw new SVNCancelException(err);
            }
            else if (err.ErrorCode.Authentication)
            {
                throw new SVNAuthenticationException(err);
            }
            else
            {
                throw new SVNException(err);
            }
        }

        /// <summary>
        /// Creates and throws an <seealso cref="SVNException"/> based on the <see cref="SVNErrorMessage"/> and inner exception.
        /// </summary>
        /// <param name="errorMessage">The ErrorMessage.</param>
        /// <param name="cause">The inner Exception.</param>
        public static void error(SVNErrorMessage errorMessage, Exception cause)
        {
            if (errorMessage == null)
            {
                errorMessage = SVNErrorMessage.create(SVNErrorCode.UNKNOWN);
            }

            if (isInfoEnabled)
                log.Info(errorMessage.Message);

            if (errorMessage.ErrorCode == SVNErrorCode.CANCELLED)
            {
                throw new SVNCancelException(errorMessage);
            }
            else if (errorMessage.ErrorCode.Authentication)
            {
                throw new SVNAuthenticationException(errorMessage);
            }
            else
            {
                throw new SVNException(errorMessage, cause);
            }
        }

        public static void error(SVNErrorMessage err1, SVNErrorMessage err2)
        {
            if (err1 == null)
            {
                error(err2);
            }
            else if (err2 == null)
            {
                error(err1);
            }
            err1.ChildErrorMessage = err2;

            if (isInfoEnabled)
                log.Info(err1.Message);

            if (err1.ErrorCode == SVNErrorCode.CANCELLED || err2.ErrorCode == SVNErrorCode.CANCELLED)
            {
                throw new SVNCancelException(err1);
            }
            else if (err1.ErrorCode.Authentication || err2.ErrorCode.Authentication)
            {
                throw new SVNAuthenticationException(err1);
            }
            throw new SVNException(err1);
        }

        public static void LogError(Exception exception)
        {
            LogError(exception.Message);
        }

        public static void LogError(string message)
        {
            if (isErrorEnabled)
                log.Error(message);
        }


    }
}