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
using DotSVN.Common;
using DotSVN.Common.Entities;
using DotSVN.Common.Util;
using DotSVN.Server.RepositoryAccess;
using DotSVN.Tests.Properties;
using DotSVN.Tests.Utils;
using NUnit.Framework;

namespace DotSVN.Tests.Server.RepositoryAccess
{
    [TestFixture]
    public class SVNRepositoryFactoryTests
    {
        private string testRepositoryPath;
        private readonly long expectedRevision = 7;
        private readonly string expectedUUID = "c05fa231-13bb-1140-932e-d33687eeb1a3";
        private readonly string repositoryDumpPath = Path.Combine(Path.GetTempPath(), "RepositoryDump");
        #region Setup/Teardown

        [SetUp]
        public void CreateRepository()
        {
            testRepositoryPath = Core.ExtractRepository();
            if(Directory.Exists(repositoryDumpPath))
                Directory.Delete(repositoryDumpPath, true);
            Directory.CreateDirectory(repositoryDumpPath);
        }

        #endregion

        [Test]
        public void TestConnectionToFSRepository()
        {
            string reposPath = "file://" + testRepositoryPath;
            try
            {
                ISVNRepository repository = SVNRepositoryFactory.Create(new SVNURL(reposPath));
                repository.TestConnection();
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception: " + ex.Message);
            }
        }

        [Test]
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

                ICollection<SVNDirEntry> dirEntries = repository.GetDir("", -1, null);
                foreach (SVNDirEntry dirEntry in dirEntries)
                {
                    string DirName = dirEntry.Name;
                }

                repository.CloseRepository();
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception: " + ex.Message);
            }
        }

        [Test]
        public void TestGetFile()
        {
            string reposPath = "file://" + testRepositoryPath;
            try
            {
                ISVNRepository repository = SVNRepositoryFactory.Create(new SVNURL(reposPath));

                IDictionary<string, string> properties = new Dictionary<string, string>();
                using (Stream outStream = File.OpenWrite(Path.Combine(Path.GetTempPath(), "Form.cs")))
                {
                    long revision = repository.GetFile("Form.cs", -1, properties, outStream);
                    Assert.AreEqual(revision, expectedRevision, string.Format("Version expected is : {0}, but got {1}", 
                        expectedRevision, revision));
                    Assert.AreEqual(outStream.Length, 4697, "Stream length does not match");
                }
                repository.CloseRepository();
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception: " + ex.Message);
            }
        }

        [Test]
        public void TestDirectoryStructure()
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
                
                EnumerateDirectories(repository, "");

                repository.CloseRepository();
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception: " + ex.Message);
            }
        }

        private void EnumerateDirectories(ISVNRepository repository, string path)
        {
            ICollection<SVNDirEntry> dirEntries = repository.GetDir(path, -1, null);
            System.Diagnostics.Debug.Indent();
            foreach (SVNDirEntry dirEntry in dirEntries)
            {
                string DirName = dirEntry.Name;
                System.Diagnostics.Debug.WriteLine(DirName);
                if(dirEntry.Kind == SVNNodeKind.dir)
                {
                    EnumerateDirectories(repository, string.IsNullOrEmpty(path) ? dirEntry.Name :
                                                                                    path + "/" + dirEntry.Name);
                }
                else
                {
                    string dirName = Path.Combine(repositoryDumpPath, path);
                    if (!Directory.Exists(dirName))
                    {
                        dirName = Directory.CreateDirectory(dirName).FullName;
                    }

                    string newFileName = Path.Combine(dirName, dirEntry.Name);
                    using (Stream outStream = File.OpenWrite(newFileName))
                    {
                        string fileName = string.IsNullOrEmpty(path) ? dirEntry.Name : path + "/" + dirEntry.Name;
                        IDictionary<string, string> properties = new Dictionary<string, string>();
                        repository.GetFile(fileName, -1, properties, outStream);
                    }
                }
            }
            System.Diagnostics.Debug.Unindent();
        }

        [Test]
        public void TestConfiguredRepositoryStructure()
        {
            if(string.IsNullOrEmpty(Settings.Default.TestRepositoryPath))
                return;

            string reposPath = "file://" + Settings.Default.TestRepositoryPath;
            try
            {
                ISVNRepository repository = SVNRepositoryFactory.Create(new SVNURL(reposPath));

                EnumerateDirectories(repository, "");
                repository.CloseRepository();
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception: " + ex.Message);
            }
        }
    }
}