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
using System.IO;
using DotSVN.Tests.TestData;

namespace DotSVN.Tests.Utils
{
    public class Core
    {
        #region Fields

        private static readonly string BasePath;
        private static readonly string RepositoryName = "DummyRepos";
        private static readonly string RepositoryZipFileName;
        private static readonly string WCZipFileName;

        #endregion

        static Core()
        {
            BasePath = Path.Combine(Path.GetTempPath(), @"DotSVN");
            string prefix = typeof (DummyType).Namespace;
            RepositoryZipFileName = prefix + @".repos.zip";
            WCZipFileName = prefix + @".wc.zip";
        }

        private static string CreateFSFSRepository()
        {
            return ExtractRepository();
        }

        /// <summary>
        /// extract our test repository
        /// </summary>
        public static string ExtractRepository()
        {
            string reposPath = Path.Combine(BasePath, RepositoryName);
            ExtractRepository(RepositoryZipFileName, reposPath, typeof (Core));
            return reposPath;
        }

        public static void ExtractRepository(string resourceName, string path, Type type)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Zip.ExtractZipResource(path, type, resourceName);
        }
    }
}