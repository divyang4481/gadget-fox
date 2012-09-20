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

namespace DotSVN.Common.Util
{
    /// <summary>
    /// The <c>SVNErrorMessage</c> class represents error and warning messages describing
    /// reasons of exceptions occurred during runtime. An error message may be of two levels:
    /// <ul>	
    ///     <li>Error type</li>
    /// 	<li>Warning type</li>
    /// </ul>
    /// An error message may contain an error messages stack trace, what is useful for
    /// error reason investigations. Also such a message contains an error code SVNErrorCode
    /// what gives an ability to find out what kind of an error it is.
    /// <para> Error messages may be supplied within exceptions of the main exception type - <c>SVNException</c>.</para>
    /// </summary>
    [Serializable]
    public class SVNErrorMessage
    {
        #region ERROR_TYPE enum

        /// <summary>
        /// Error type enumeration
        /// </summary>
        public enum ERROR_TYPE
        {
            /// <summary>Error messages of this type are considered to be errors (most critical) rather than warnings. </summary>
            Error = 0,
            /// <summary> Error messages of this type are considered to be warnings, what in certain situations may be OK.</summary>
            Warning = 1
        }

        #endregion

        private static readonly Object[] EMPTY_ARRAY = new Object[0];

        /// <summary> This is a type of an error message denoting an error of an unknown nature.
        /// This corresponds to an {@link SVNErrorCode#UNKNOWN} error.
        /// </summary>
        public static SVNErrorMessage UNKNOWN_ERROR_MESSAGE;

        private SVNErrorMessage childErrorMessage;
        private SVNErrorCode errorCode;
        private ERROR_TYPE errorType;
        private Exception exception;
        private String message;
        private Object[] relatedObjects;

        /// <summary>
        /// Initializes the <see cref="SVNErrorMessage"/> class.
        /// </summary>
        static SVNErrorMessage()
        {
            UNKNOWN_ERROR_MESSAGE = create(SVNErrorCode.UNKNOWN);
        }

        /// <summary>
        /// Creates an error message given an error code, description, an error type
        /// (whether it's a warning or an error) and may be related objects to be
        /// formatted with the error description. To format the provided objects
        /// with the message, you should use valid format patterns
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error description</param>
        /// <param name="relatedObjects">An array of objects related to the error message</param>
        /// <param name="ex">Inner Exception</param>
        /// <param name="type">Error type</param>
        protected internal SVNErrorMessage(SVNErrorCode code, String message, Object[] relatedObjects,
                                           Exception ex,
                                           ERROR_TYPE type)
        {
            errorCode = code;
            if (message != null && message.StartsWith("svn: "))
            {
                this.message = message.Substring("svn: ".Length);
            }
            this.message = message;
            this.relatedObjects = relatedObjects;
            errorType = type;
            exception = ex;
        }

        /// <summary>
        /// Get the type of the error (whether it's a warning or an error).
        /// </summary>
        /// <value>The type of the error.</value>
        public ERROR_TYPE ErrorType
        {
            get { return errorType; }
        }

        /// <summary>
        /// Get the error code of the error.
        /// </summary>
        /// <value>The error code.</value>
        public SVNErrorCode ErrorCode
        {
            get { return errorCode; }
        }

        /// <summary>
        /// Returns an error description formatted with the
        /// related objects if needed. This call is equivalent to a call to {@link #toString()}
        /// </summary>
        /// <value>The message.</value>
        public String Message
        {
            get { return ToString(); }
        }

        /// <summary>
        /// Gets a string representation of the entire stack trace of
        /// error messages (if they were provided) starting with the initial
        /// cause of the error.
        /// </summary>
        /// <value>The full message.</value>
        public String FullMessage
        {
            get
            {
                SVNErrorMessage err = this;
                StringBuilder buffer = new StringBuilder();
                while (err != null)
                {
                    buffer.Append(err.Message);
                    if (err.hasChildErrorMessage())
                    {
                        buffer.Append('\n');
                    }
                    err = err.ChildErrorMessage;
                }
                return buffer.ToString();
            }
        }

        /// <summary>
        /// Returns an error description which may contain message format patterns.
        /// </summary>
        /// <value>The message template.</value>
        public String MessageTemplate
        {
            get { return message; }
        }

        /// <summary>
        /// Returns objects (if any) that were provided to be formatted with the error description.
        /// </summary>
        /// <value>The related objects.</value>
        public Object[] RelatedObjects
        {
            get { return relatedObjects; }
        }

        /// <summary>
        /// Gets or sets the child error message.
        /// </summary>
        /// <value>The child error message.</value>
        public SVNErrorMessage ChildErrorMessage
        {
            get { return childErrorMessage; }
            set { childErrorMessage = value; }
        }

        /// <summary>
        /// Returns throwable that is cause of the error if any.
        /// </summary>
        /// <value>The cause.</value>
        public Exception Cause
        {
            get { return exception; }
        }

        /// <summary>
        /// Returns true if this message is a warning message, not error one.
        /// </summary>
        /// <value><c>true</c> if warning; otherwise, <c>false</c>.</value>
        public bool Warning
        {
            get { return errorType == ERROR_TYPE.Warning; }
        }

        /// <summary>
        /// Creates an error message given an error code.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <returns>A new <c>SVNErrorMessage</c> instance</returns>
        public static SVNErrorMessage create(SVNErrorCode code)
        {
            return create(code, "", ERROR_TYPE.Error);
        }

        /// <summary>
        /// Creates an error message given an error code and description.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error description</param>
        /// <returns>A new <c>SVNErrorMessage</c> instance</returns>
        public static SVNErrorMessage create(SVNErrorCode code, String message)
        {
            return create(code, message, ERROR_TYPE.Error);
        }

        /// <summary>
        /// Creates an error message given an error code and cause.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="cause">Cause of the error</param>
        /// <returns>A new <c>SVNErrorMessage</c> instance</returns>
        public static SVNErrorMessage create(SVNErrorCode code, Exception cause)
        {
            if (cause != null)
            {
                return new SVNErrorMessage(code, cause.Message, new Object[0], cause, ERROR_TYPE.Error);
            }
            return create(code);
        }

        /// <summary>
        /// Creates an error message given an error code, description and may be a related
        /// object to be formatted with the error description.
        /// To format the provided objectwith the message, you
        /// should use valid format patterns parsable for {@link MessageFormat}.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error description</param>
        /// <param name="obj">Object related to the error message</param>
        /// <returns>A new <c>SVNErrorMessage</c> instance.</returns>
        public static SVNErrorMessage create(SVNErrorCode code, String message, Object obj)
        {
            object[] objects = new object[]{obj};
            return create(code, message, (object[]) objects, ERROR_TYPE.Error);
        }

        /// <summary>
        /// Creates an error message given an error code, description and may be related
        /// objects to be formatted with the error description.
        /// To format the provided objects with the message, you
        /// should use valid format patterns parsable for {@link MessageFormat}.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error description</param>
        /// <param name="objects">An array of objects related to the error message</param>
        /// <returns>A new <c>SVNErrorMessage</c> instance.</returns>
        public static SVNErrorMessage create(SVNErrorCode code, String message, Object[] objects)
        {
            return create(code, message, objects, ERROR_TYPE.Error);
        }

        /// <summary>
        /// Creates an error message given an error code, description and a type
        /// (whether it's a warning or an error).
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error description</param>
        /// <param name="type">Error type</param>
        /// <returns>A new <c>SVNErrorMessage</c> instance.</returns>
        public static SVNErrorMessage create(SVNErrorCode code, String message, ERROR_TYPE type)
        {
            return new SVNErrorMessage(code, message, EMPTY_ARRAY, null, type);
        }

        /// <summary>
        /// Creates an error message given an error code, description, an error type
        /// (whether it's a warning or an error) and may be a related object to be
        /// formatted with the error description.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error description</param>
        /// <param name="obj">An object related to the error message</param>
        /// <param name="type">Error type</param>
        /// <returns>A new <c>SVNErrorMessage</c> instance.</returns>
        public static SVNErrorMessage create(SVNErrorCode code, String message, Object obj, ERROR_TYPE type)
        {
            return
                new SVNErrorMessage(code == null ? SVNErrorCode.BASE : code, message == null ? "" : message,
                                    obj == null ? new Object[] {"NULL"} : new Object[] {obj}, null,
                                    type);
        }

        /// <summary>
        /// Creates an error message given an error code, description, an error type
        /// (whether it's a warning or an error) and may be related objects to be
        /// formatted with the error description. To format the provided objects
        /// with the message, you should use valid format patterns
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error description</param>
        /// <param name="objects">An array of objects related to the error message</param>
        /// <param name="type">Error type</param>
        /// <returns>A new <c>SVNErrorMessage</c> instance.</returns>
        public static SVNErrorMessage create(SVNErrorCode code, String message, Object[] objects, ERROR_TYPE type)
        {
            return
                new SVNErrorMessage(code == null ? SVNErrorCode.BASE : code, message == null ? "" : message,
                                    objects == null ? EMPTY_ARRAY : objects, null, type);
        }

        /// <summary>
        /// Says if this error message object has got a child error message.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if a child error messageis defined; otherwise, <c>false</c>.
        /// </returns>
        public bool hasChildErrorMessage()
        {
            return childErrorMessage != null;
        }

        /// <summary>
        /// Returns a string representation of this error message object
        /// formatting (if needed) the error description with the provided related objects.
        /// If no error description pattern has been provided, the return
        /// value includes a string representation of the error code (see {@link SVNErrorCode}).
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            StringBuilder line = new StringBuilder();
            if (ErrorType == ERROR_TYPE.Warning && ErrorCode == SVNErrorCode.REPOS_POST_COMMIT_HOOK_FAILED)
            {
                line.Append("Warning: ");
            }
            else if (ErrorType == ERROR_TYPE.Warning)
            {
                line.Append("svn: warning: ");
            }
            else
            {
                line.Append("svn: ");
            }
            if (String.IsNullOrEmpty(message))
            {
                line.Append(errorCode.Description);
            }
            else
            {
                line.Append(relatedObjects.Length > 0 ? String.Format(message, relatedObjects) : message);
            }
            return line.ToString();
        }

        /// <summary>
        /// Wraps this error message into a new one that is returned as
        /// a parent error message. A parent message is set the error code
        /// of this error message, a new error description and this error
        /// message as its child.
        /// </summary>
        /// <param name="parentMessage">A parent error description</param>
        /// <returns>A new message <c>SVNErrorMessage</c> object.</returns>
        public SVNErrorMessage wrap(String parentMessage)
        {
            SVNErrorMessage parentError = create(ErrorCode, parentMessage);
            parentError.ChildErrorMessage = this;
            return parentError;
        }

        /// <summary>
        /// Wraps this error message into a new one that is returned as
        /// a parent error message. A parent message is set the error code
        /// of this error message, a new error description and this error
        /// message as its child.
        /// </summary>
        /// <param name="parentMessage">Parent error description</param>
        /// <param name="relatedObject">An object to be formatted with parentMessage</param>
        /// <returns>A new message <c>SVNErrorMessage</c> object.</returns>
        public SVNErrorMessage wrap(String parentMessage, Object relatedObject)
        {
            SVNErrorMessage parentError = create(ErrorCode, parentMessage, relatedObject);
            parentError.ChildErrorMessage = this;
            return parentError;
        }

        /// <summary>
        /// Wraps this error message into a new one that is returned as
        /// a parent error message. A parent message is set the error code
        /// of this error message, a new error description and this error
        /// message as its child.
        /// </summary>
        /// <param name="parentMessage">Parent error description</param>
        /// <param name="relatedObjs">An object to be formatted with parentMessage</param>
        /// <returns>A new message <c>SVNErrorMessage</c> object.</returns>
        public SVNErrorMessage wrap(String parentMessage, Object[] relatedObjs)
        {
            SVNErrorMessage parentError = create(ErrorCode, parentMessage, relatedObjs);
            parentError.ChildErrorMessage = this;
            return parentError;
        }
    }
}