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

using System.Collections.Generic;
using DotSVN.Common.Exceptions;
using DotSVN.Common.Util;
using NUnit.Framework;

namespace DotSVN.Tests.Common.Util
{
    /// <summary>
    /// Test the following types of URLS:
    /// http://userInfo@host:port/path      eg: http://george@dotSVN:80/dotsvn/trunk
    /// file:///[drive]:/path               eg: file:///D:/Temp/repos
    /// file://localhost/[drive]:/path      eg: file://localhost/D:/Temp/repos 
    /// </summary>
    [TestFixture]
    public class SVNURLTest
    {
        public SVNURLTest()
        {
        }

        [Test]
        public void BasicHTTPURLTest()
        {
            bool urlIsValid = true;
            string testURL = @"http://localhost/a/b/c";
            try
            {
                SVNURL httpURL = new SVNURL(testURL);
            }
            catch
            {
                urlIsValid = false;
            }
            Assert.IsTrue((urlIsValid == true),
                          string.Format("URL {0} is valid; but SVNURL said it is not valid",
                                        testURL));
        }

        [Test]
        [ExpectedException(typeof (SVNException))]
        public void FileWithNoPathTest()
        {
            string testURL = @"file:///";
            SVNURL fileURL = new SVNURL(testURL);
        }


        [Test]
        public void IsURLSafeTest()
        {
            Dictionary<string, bool> testURLs = new Dictionary<string, bool>();

            #region TestData

            testURLs.Add("http://svn.collab.net/repos", true);
            testURLs.Add("http://svn.collab.net:122/repos%", false);
            testURLs.Add("http://svn.collab.net/repos%/svn", false);
            testURLs.Add("http://svn.collab.net/repos%2g", false);
            testURLs.Add("http://svn.collab.net/repos%2g/svn", false);
            testURLs.Add("http://svn.collab.net/repos%%", false);
            testURLs.Add("http://svn.collab.net/repos%%/svn", false);
            testURLs.Add("http://svn.collab.net/repos%2a", true);
            testURLs.Add("http://svn.collab.net/repos%2a/svn", true);

            #endregion // TestData

            foreach (KeyValuePair<string, bool> testURL in testURLs)
            {
                bool isSafe = true;
                try
                {
                    SVNURL url = new SVNURL(testURL.Key);
                }
                catch
                {
                    isSafe = false;
                }
                Assert.IsTrue((isSafe == testURL.Value),
                              string.Format("URL {0} is marked as {1} but it should be {2}",
                                            testURL.Key, isSafe, testURL.Value));
            }
        }

        [Test]
        public void IsURLTest()
        {
            Dictionary<string, bool> testURLs = new Dictionary<string, bool>();

            #region TestData

            testURLs.Add("://blah/blah", false);
            testURLs.Add("a:abb://boo/", false);
            testURLs.Add("http://svn.collab.net/repos/svn", true);
            testURLs.Add("scheme/with://slash/", false);
            testURLs.Add("file:///path/to/repository", true);
            testURLs.Add("file://", false);
            testURLs.Add("file:/", false);
            testURLs.Add("http://svn.collab.net/repos/svn://", true);

            #endregion  // TestData

            foreach (KeyValuePair<string, bool> testURL in testURLs)
            {
                bool isURL = true;
                try
                {
                    SVNURL url = new SVNURL(testURL.Key);
                }
                catch
                {
                    isURL = false;
                }
                string message = string.Format("Test URL \"{0}\" is {1} a URL, but SVNURL said it is {2} a URL",
                                               testURL.Key,
                                               (testURL.Value == true) ? "" : "NOT",
                                               (isURL == true) ? "" : "NOT");
                Assert.IsTrue((isURL == testURL.Value), message);
            }
        }

        [Test]
        public void URLEncodeTest()
        {
            Dictionary<string, string> testURLs = new Dictionary<string, string>();

            #region TestData

            testURLs.Add("http://subversion.tigris.org", "http://subversion.tigris.org:80");
            testURLs.Add("http://subversion.tigris.org/ special_at_beginning",
                         "http://subversion.tigris.org:80/%20special_at_beginning");
            testURLs.Add("http://subversion.tigris.org:55/special_at_end ",
                         "http://subversion.tigris.org:55/special_at_end%20");
            testURLs.Add("http://subversion.tigris.org:55/special in middle",
                         "http://subversion.tigris.org:55/special%20in%20middle");
            // testURLs.Add("\"Ouch!\"  \"Did that hurt?\"", "%22Ouch!%22%20%20%22Did%20that%20hurt%3F%22");
            testURLs.Add("http://subversion.tigris.org:55/\"Ouch!\"  \"DID that hurt\"",
                         "http://subversion.tigris.org:55/%22ouch!%22%20%20%22did%20that%20hurt%22");

            #endregion  // TestData

            foreach (KeyValuePair<string, string> testUrl in testURLs)
            {
                string encodedUrl = null;
                try
                {
                    SVNURL svnUrl = new SVNURL(testUrl.Key);
                    encodedUrl = svnUrl.ToString();
                }
                catch
                {
                    encodedUrl = string.Empty;
                }

                string message =
                    string.Format("Test URL {0} should have been encoded as {1}, but SVNURL encoded it as {2}",
                                  testUrl.Key, testUrl.Value, encodedUrl);
                Assert.IsTrue((encodedUrl == testUrl.Value), message);
            }
        }

        [Test]
        public void UTF8EncodingTest()
        {
        }
    }
}