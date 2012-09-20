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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotSVN.Common.Util
{
    /// <summary>
    /// The <c>SVNURL</c> class is used for representing urls. Those DotSVN
    /// API methods, that need repository locations to carry out an operation,
    /// receive a repository location url represented by <c>SVNURL</c>. This
    /// class does all the basic work for a caller:
    /// <list type="bullet">
    /// 		<item>parses an original url string (splitting it to components)</item>
    /// 		<item>encodes/decodes a path component to/from the UTF-8 charset</item>
    /// 		<item>checks for validity (such as protocol support)</item>
    /// 	</list>
    /// If DotSVN does not support a particular protocol, <c>SVNURL</c>
    /// throws a corresponding exception).
    /// <para>To Create a new <c>SVNURL</c> representation, pass an original url
    /// string (like "http://userInfo@host:port/path") to a corresponding
    /// <i>parse</i> method of this class.</para>
    /// </summary>
    public class SVNURL
    {
        #region Private fields

        private String host;
        private bool isPortSpecified = false;
        private String path;
        private int port = -1;
        private String protocol;
        private String userName;

        #endregion

        private static readonly Dictionary<string, int> DefaultProtocolPorts = new Dictionary<string, int>();

        /// <summary>
        /// Initializes the <see cref="SVNURL"/> class.
        /// </summary>
        static SVNURL()
        {
            {
                DefaultProtocolPorts["svn"] = 3690;
                DefaultProtocolPorts["svn+ssh"] = 22;
                DefaultProtocolPorts["http"] = 80;
                DefaultProtocolPorts["https"] = 443;
                DefaultProtocolPorts["file"] = 0;
            }
        }

        /// <summary>
        /// Construct an SVNURL from a string
        /// </summary>
        /// <param name="url">Strin input</param>
        public SVNURL(String url)
        {
            // Try to parse the URL
            try
            {
                // Check whether URL is NULL
                if (url == null)
                {
                    SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.BAD_URL, "URL cannot be NULL");
                    SVNErrorManager.error(err);
                    return;
                }
                // Unescape the escape sequences (like %xx) in the url; otherwise Uri ctor will
                //  encode every "%" in the url as "%25" (this is a bug in Uri.EscapeUriString impl)
                if (url.Contains("%"))
                {
                    url = Uri.UnescapeDataString(url);
                }

                url = url.Replace('\\', '/');
                url = Uri.EscapeUriString(url);

                // Create the URI object. The Uri ctor will parse, canonicalize & escape encodings
                Uri uri = new Uri(url);
                protocol = uri.Scheme;
                host = uri.Host;
                userName = uri.UserInfo;

                // Check if this is a protocol that we support
                if (!DefaultProtocolPorts.ContainsKey(protocol) && !protocol.StartsWith("svn+"))
                {
                    SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.BAD_URL,
                                                                 "URL protocol is not supported ''{0}''", url);
                    SVNErrorManager.error(err);
                }

                // Extract the protocol; if the given port is negative, take the defualt port
                //  for the specified protocol
                if (uri.Port < 0)
                {
                    port = DefaultProtocolPorts[protocol];
                }
                else
                {
                    isPortSpecified = true;
                    port = uri.Port;
                }

                // Extract the file path. 
                if (!string.IsNullOrEmpty(uri.AbsolutePath))
                {
                    path = uri.AbsolutePath.ToLower().Replace('\\', '/');

                    // Add a leading forward slash (/)
                    if (!path.StartsWith("/") && (protocol != "file"))
                    {
                        path = "/" + path;
                    }

                    // If path is empty (just the root path), throw an error for file protocol
                    if ((path == "/") && (protocol == "file"))
                    {
                        SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.BAD_URL,
                                                                     string.Format("Local file path should contain a path '{0}'", url));
                        SVNErrorManager.error(err);
                    }
                }
            }
            catch (UriFormatException formatEx)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.BAD_URL, "Wrong format URL: ''{0}'': {1}",
                                                             new Object[] {url, formatEx.Message});
                SVNErrorManager.error(err, formatEx);
            }
            catch (Exception ex)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.BAD_URL, "Malformed URL: ''{0}'': {1}",
                                                             new Object[] {url, ex.Message});
                SVNErrorManager.error(err, ex);
            }
        }

        #region Properties

        /// <summary>
        /// Returns the protocol component of the url represented by this object.
        /// </summary>
        /// <value>The protocol.</value>
        /// <returns>The protocol name (like http)</returns>
        public String Protocol
        {
            get { return protocol; }
        }

        /// <summary>
        /// Returns the host component of the url represented by this object.
        /// </summary>
        /// <value>The host.</value>
        /// <returns>The host name</returns>
        public String Host
        {
            get { return host; }
        }

        /// <summary>
        /// Returns the path component of the url represented by this object as a uri-decoded string
        /// </summary>
        /// <value>The URL path.</value>
        /// <returns>a uri-decoded path</returns>
        public String Path
        {
            get { return path; }
        }

        /// <summary>
        /// Returns the port number specified (or default) for the host.
        /// </summary>
        /// <value>The port.</value>
        /// <returns>The port number.</returns>
        public int Port
        {
            get { return port; }
        }


        /// <summary>
        /// Returns the user info component of the url represented by this object.
        /// </summary>
        /// <value>The user info.</value>
        /// <returns>The user info part of the url (if it was provided)</returns>
        public String UserInfo
        {
            get { return userName; }
        }

        #endregion

        /// <summary>
        /// Creates a new <c>SVNURL</c> representation from the given url components.
        /// </summary>
        /// <param name="protocol">Protocol component</param>
        /// <param name="userInfo">User info component</param>
        /// <param name="host">Host component</param>
        /// <param name="port">Port number</param>
        /// <param name="path">Path component</param>
        /// <returns>A new <c>SVNURL</c> instance.</returns>
        public static SVNURL Create(String protocol, String userInfo, String host, int port, String path)
        {
            if (host == null || host.IndexOf('@') >= 0)
            {
                SVNErrorManager.error(SVNErrorMessage.create(SVNErrorCode.BAD_URL, "Invalid host name ''{0}''", host));
                return null;
            }
            path = path == null ? "/" : path.Trim();


            if (path.Length > 0 && path[path.Length - 1] == '/')
            {
                path = path.Substring(0, (path.Length - 1) - (0));
            }
            protocol = protocol == null ? "http" : protocol.ToLower();
            String errorMessage = null;
            if (userInfo != null && userInfo.IndexOf('/') >= 0)
            {
                errorMessage = "Malformed URL: user info part could not include '/' symbol";
            }
            else if (!"file".Equals(protocol))
            {
                errorMessage = "Malformed URL: host could not be NULL";
            }
            else if (!"file".Equals(protocol) && host.IndexOf('/') >= 0)
            {
                errorMessage = "Malformed URL: host could not include '/' symbol";
            }
            if (errorMessage != null)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.BAD_URL, errorMessage);
                SVNErrorManager.error(err);
            }
            String url = ComposeURL(protocol, userInfo, host, port, path);
            return new SVNURL(url);
        }

        /// <summary>
        /// Creates a "file:///" <c>SVNURL</c> representation given a
        /// filesystem style repository path.
        /// </summary>
        /// <param name="repositoryPath">A repository path as a filesystem path</param>
        /// <returns>A new <c>SVNURL</c> instance.</returns>
        public static SVNURL FromFile(FileInfo repositoryPath)
        {
            if (repositoryPath == null)
            {
                return null;
            }
            String path = new FileInfo(repositoryPath.FullName).FullName;
            path = path.Replace(System.IO.Path.DirectorySeparatorChar, '/');
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            return new SVNURL("file://" + path);
        }

        /// <summary>
        /// Returns the default port number for the specified protocol.
        /// </summary>
        /// <param name="protocol">A particular access protocol</param>
        /// <returns>
        /// The default port number for the specified protocol
        /// </returns>
        public static int DefaultPortNumberFor(String protocol)
        {
            if (protocol == null)
            {
                return -1;
            }
            protocol = protocol.ToLower();
            if ("file".Equals(protocol))
            {
                return -1;
            }
            if (DefaultProtocolPorts.ContainsKey(protocol))
            {
                int dPort = DefaultProtocolPorts[protocol];
                return dPort;
            }
            return -1;
        }

        /// <summary>
        /// Constructs a new <c>SVNURL</c> representation appending a new path
        /// segment to the path component of this representation.
        /// </summary>
        /// <param name="segment">a new path segment</param>
        /// <returns>a new <c>SVNURL</c> representation</returns>
        public SVNURL AppendPath(String segment)
        {
            if (string.IsNullOrEmpty(segment))
            {
                return this;
            }
            String url = ComposeURL(Protocol, UserInfo, Host, port, System.IO.Path.Combine(path, segment));
            return new SVNURL(url);
        }

        /// <summary>
        /// Creates a new <c>SVNURL</c> object replacing a Path component of  this object with a new provided one.
        /// </summary>
        /// <param name="inputPath">a Path component</param>
        /// <returns>a new <c>SVNURL</c> representation</returns>
        public SVNURL SetPath(String inputPath)
        {
            if (string.IsNullOrEmpty(inputPath))
            {
                inputPath = "/";
            }
            String url = ComposeURL(Protocol, UserInfo, Host, port, inputPath);
            return new SVNURL(url);
        }

        /// <summary>
        /// Composes the URL.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="userInfo">The user info.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="path">The path.</param>
        /// <returns>An encoded url string</returns>
        private static String ComposeURL(String protocol, String userInfo, String host, int port, String path)
        {
            StringBuilder url = new StringBuilder();
            url.Append(protocol);
            url.Append(Uri.SchemeDelimiter);
            if (!String.IsNullOrEmpty(userInfo))
            {
                url.Append(userInfo);
                url.Append("@");
            }
            url.Append(host);
            if (port > 0)
            {
                url.Append(":");
                url.Append(port);
            }
            if (path != null && !path.StartsWith("/"))
            {
                path = '/' + path;
            }
            if ("/".Equals(path))
            {
                path = "";
            }
            url.Append(path);
            return url.ToString();
        }

        /// <summary>
        /// Returns a string representing a UTF-8 encoded url.
        /// </summary>
        /// <returns>An encoded url string</returns>
        public override String ToString()
        {
            return ComposeURL(Protocol, UserInfo, Host, isPortSpecified ? port : -1, path);
        }

        /// <summary>
        /// Compares this object with another one.
        /// </summary>
        /// <param name="obj">an object to compare with</param>
        /// <returns>
        /// true if obj is an instance of <c>SVNURL</c> and has got the same url components as this object has
        /// </returns>
        public override bool Equals(Object obj)
        {
            if (obj == null || obj.GetType() != typeof (SVNURL))
            {
                return false;
            }
            SVNURL url = (SVNURL) obj;
            bool eq = protocol.Equals(url.protocol) && port == url.port && host.Equals(url.host) &&
                      path.Equals(url.path);
            if (userName == null)
            {
                eq &= url.userName == null;
            }
            else
            {
                eq &= userName.Equals(url.userName);
            }
            return eq;
        }

        /// <summary>
        /// Calculates and returns a hash code for this object.
        /// </summary>
        /// <returns>A hash code value</returns>
        public override int GetHashCode()
        {
            int code = protocol.GetHashCode() + host.GetHashCode()*27 + path.GetHashCode()*31 + port*17;
            if (userName != null)
            {
                code += 37*userName.GetHashCode();
            }
            return code;
        }
    }
}