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
using System.Diagnostics;
using System.Globalization;
using DotSVN.Common.Entities;
using DotSVN.Common.Util;
using DotSVN.Server.RepositoryAccess;
using DotSVN.Tests.Utils;
using NUnit.Framework;

namespace DotSVN.Samples
{
    internal class Program
    {
        private string testRepositoryPath;
        private readonly long expectedRevision = 7;
        private readonly string expectedUUID = "c05fa231-13bb-1140-932e-d33687eeb1a3";

        public void CreateRepository()
        {
            testRepositoryPath = Core.ExtractRepository();
        }

        public void CreateFSRepository()
        {
            string reposPath = "file://" + testRepositoryPath;
            try
            {
                ISVNRepository repository = SVNRepositoryFactory.Create(new SVNURL(reposPath));

                string repostoryUUID = repository.GetRepositoryUUID(true);
                Assert.AreEqual(expectedUUID, repostoryUUID,
                                string.Format("Expected repository UUID is : {0}, but we got {1}", expectedUUID,
                                              repostoryUUID));

                long latestRev = repository.GetLatestRevision();
                Assert.AreEqual(expectedRevision, latestRev,
                                string.Format("Expected Revision is {0}, but returned {1}", expectedRevision,
                                              latestRev));

                IDictionary<string, string> properties = new Dictionary<string, string>();
                string rootDir = @"/doc";
                // Other valid paths: "", "/" , "bin", "bin/Debug","/bin/Debug" etc.
                ICollection<SVNDirEntry> dirEntries = repository.GetDir(rootDir, -1, properties);

                Debug.Indent();
                Debug.WriteLine("\n[DotSVN Output]\n \tDirectory Structure...");
                foreach (SVNDirEntry dirEntry in dirEntries)
                {
                    Debug.WriteLine(dirEntry.ToString());
                    Console.WriteLine(dirEntry.ToString());
                }

                Debug.WriteLine("\n\tProperties....");
                Console.WriteLine("\nProperties\n");
                foreach (String propKey in properties.Keys)
                {
                    string propValue = properties[propKey];
                    if (propKey == SVNProperty.COMMITTED_DATE)
                    {
                        DateTime parsedDate = DateTime.Parse(properties[propKey],
                                                             new CultureInfo("en-US"),
                                                             DateTimeStyles.AssumeLocal);
                        propValue = parsedDate.ToLocalTime().ToString();
                    }
                    string output = string.Format("{0}: {1}", propKey, propValue);
                    Debug.WriteLine(output);
                    Console.WriteLine(output);
                }

                repository.CloseRepository();
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception: " + ex.Message);
            }
        }

        private static void Main(string[] args)
        {
            Program pg = new Program();
            pg.CreateRepository();
            pg.CreateFSRepository();
        }
    }
}