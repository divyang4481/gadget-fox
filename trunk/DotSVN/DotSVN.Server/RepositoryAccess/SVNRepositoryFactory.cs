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
using System.Text.RegularExpressions;
using DotSVN.Common.Util;
using DotSVN.Server.RepositoryAccess.File;

namespace DotSVN.Server.RepositoryAccess
{
    /// <summary>
    /// 	<c>SVNRepositoryFactory</c> is an abstract factory that is responsible for creating an appropriate
    /// <c>SVNRepository</c> driver specific for the protocol to use.
    /// </summary>
    public abstract class SVNRepositoryFactory
    {
        private static Dictionary<Regex, SVNRepositoryFactory> factories = new Dictionary<Regex, SVNRepositoryFactory>();

        /// <summary>
        /// Initializes the <see cref="SVNRepositoryFactory"/> class.
        /// </summary>
        static SVNRepositoryFactory()
        {
            FSRepositoryFactory.Setup();
        }

        /// <summary>
        /// Registers the repository factory.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="factory">The factory.</param>
        protected static void RegisterRepositoryFactory(Regex protocol, SVNRepositoryFactory factory)
        {
            if (protocol != null && factory != null)
            {
                factories.Add(protocol, factory);
            }
        }

        /// <summary>
        /// Creates an <c>SVNRepository</c> driver according to the protocol that is to be used to access a repository.
        /// <para>The protocol is defined as the beginning part of the URL schema. </para>
        /// </summary>
        /// <param name="url">A repository location URL</param>
        /// <returns>
        /// A protocol specific <c>SVNRepository</c> driver
        /// </returns>
        public static ISVNRepository Create(SVNURL url)
        {
            return Create(url, null);
        }

        /// <summary>
        /// Creates an <c>SVNRepository</c> driver according to the protocol that is to be used to access a repository.
        /// <para>The protocol is defined as the beginning part of the URL schema. </para>
        /// </summary>
        /// <param name="url">A repository location URL</param>
        /// <param name="options">A session options driver</param>
        /// <returns>
        /// A protocol specific <c>SVNRepository</c> driver
        /// </returns>
        public static ISVNRepository Create(SVNURL url, ISVNSession options)
        {
            String urlString = url.ToString();
            foreach (KeyValuePair<Regex, SVNRepositoryFactory> keyValuePair in factories)
            {
                if (keyValuePair.Key.Match(urlString).Success)
                {
                    return keyValuePair.Value.CreateRepository(url, options);
                }
            }

            SVNErrorMessage err =
                SVNErrorMessage.create(SVNErrorCode.BAD_URL, "Unable to Create SVNRepository object for ''{0}''", url);
            SVNErrorManager.error(err);
            return null;
        }

        /// <summary>
        /// Creates the repository.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        protected virtual ISVNRepository CreateRepository(SVNURL url)
        {
            return CreateRepository(url, null);
        }

        /// <summary>
        /// Creates the repository.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        protected abstract ISVNRepository CreateRepository(SVNURL url, ISVNSession options);
    }
}