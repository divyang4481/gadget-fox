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

using System.Text.RegularExpressions;
using DotSVN.Common.Util;

namespace DotSVN.Server.RepositoryAccess.File
{
    /// <summary>
    /// 
    /// </summary>
    public class FSRepositoryFactory : SVNRepositoryFactory
    {
        /// <summary>
        /// Sets up the FSRepository
        /// </summary>
        public static void Setup()
        {
            RegisterRepositoryFactory(new Regex("^file://.*$"), new FSRepositoryFactory());
        }

        /// <summary>
        /// Creates an <see cref="FSRepository"/>
        /// </summary>
        /// <param name="url">Location of the repository</param>
        /// <param name="session">Session options</param>
        /// <returns>
        /// Returns an instance of <see cref="ISVNRepository"/>
        /// </returns>
        protected override ISVNRepository CreateRepository(SVNURL url, ISVNSession session)
        {
            return new FSRepository(url, session);
        }
    }
}