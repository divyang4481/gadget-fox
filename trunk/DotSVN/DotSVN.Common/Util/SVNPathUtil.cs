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
using System.Collections;
using System.IO;
using System.Text;

namespace DotSVN.Common.Util
{
    /// <summary>
    /// Utility class to format, resolve paths
    /// </summary>
    public class SVNPathUtil
    {
        /// <summary>
        /// Determines whether the specified name is single path component.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the specified name is single path component; otherwise, <c>false</c>.
        /// </returns>
        public static bool isSinglePathComponent(String name)
        {
            /* Can't be empty or `..'  */
            if (string.IsNullOrEmpty(name) || "..".Equals(name))
            {
                return true;
            }
            /* Slashes are bad */
            if (name.IndexOf('/') != - 1)
            {
                return false;
            }
            /* It is valid.  */
            return true;
        }

        /// <summary>
        /// Determines whether the specified path is canonical.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if the specified path is canonical; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCanonical(String path)
        {
            return
                (path != null && !(path.Length == 1 && path[0] == '.') 
                              && (path.Length <= 1 || path[path.Length - 1] != '/'));
        }

        /// <summary>
        /// Removes the tail of the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static String removeTail(String path)
        {
            int index = path.Length - 1;
            while (index >= 0)
            {
                if (path[index] == '/')
                {
                    return path.Substring(0, (index) - (0));
                }
                index--;
            }
            return "";
        }

        /// <summary>
        /// Gets the common path ancestor.
        /// </summary>
        /// <param name="path1">The path1.</param>
        /// <param name="path2">The path2.</param>
        /// <returns></returns>
        public static String getCommonPathAncestor(String path1, String path2)
        {
            if (path1 == null || path2 == null)
            {
                return null;
            }
            path1 = path1.Replace(Path.DirectorySeparatorChar, '/');
            path2 = path2.Replace(Path.DirectorySeparatorChar, '/');

            int index = 0;
            int separatorIndex = 0;
            while (index < path1.Length && index < path2.Length)
            {
                if (path1[index] != path2[index])
                {
                    break;
                }
                if (path1[index] == '/')
                {
                    separatorIndex = index;
                }
                index++;
            }
            if (index == path1.Length && index == path2.Length)
            {
                return path1;
            }
            else if (index == path1.Length && path2[index] == '/')
            {
                return path1;
            }
            else if (index == path2.Length && path1[index] == '/')
            {
                return path2;
            }
            return path1.Substring(0, (separatorIndex) - (0));
        }

        /// <summary>
        /// Canonicalizes the absolute path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static String canonicalizeAbsPath(String path)
        {
            //No path, no problem
            if (path == null)
            {
                return null;
            }

            //If no content in path
            if ("".Equals(path))
            {
                return "/";
            }

            StringBuilder newString = new StringBuilder();
            //Set leading '/' character
            if (!path.StartsWith("/"))
            {
                newString.Append('/');
            }

            //dispose of slashes number of that is 
            bool eatingSlashes = false;
            for (int count = 0; count < path.Length; count++)
            {
                if (path[count] == '/')
                {
                    if (eatingSlashes)
                    {
                        continue;
                    }
                    eatingSlashes = true;
                }
                else
                {
                    if (eatingSlashes)
                    {
                        eatingSlashes = false;
                    }
                }
                newString.Append(path[count]);
            }

            if (newString.Length > 1 && newString[newString.Length - 1] == '/')
            {
                newString.Remove(newString.Length - 1, 1);
            }

            return newString.ToString();
        }

        /// <summary>
        /// Checks whether the path is valid.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void checkPathIsValid(String path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                char ch = path[i];
                if (IsASCIIControlChar(ch))
                {
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.FS_PATH_SYNTAX,
                                               "Invalid control character '{0}' in path '{1}'",
                                               new String[]
                                                   {
                                                       (sbyte)ch + "''", path
                                                   });
                    SVNErrorManager.error(err);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified character is ASCII control char.
        /// </summary>
        /// <param name="ch">The ch.</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is ASCII control char; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsASCIIControlChar(char ch)
        {
            return (ch >= 0x00 && ch <= 0x1f) || ch == 0x7f;
        }

        /// <summary>
        /// Gets the common URL ancestor.
        /// </summary>
        /// <param name="url1">The url1.</param>
        /// <param name="url2">The url2.</param>
        /// <returns></returns>
        public static String getCommonURLAncestor(String url1, String url2)
        {
            // skip protocol and host, if they are different -> return "";
            if (url1 == null || url2 == null)
            {
                return null;
            }
            int index = 0;
            StringBuilder protocol = new StringBuilder();
            while (index < url1.Length && index < url2.Length)
            {
                char ch1 = url1[index];
                if (ch1 != url2[index])
                {
                    return "";
                }
                if (ch1 == ':')
                {
                    break;
                }
                protocol.Append(ch1);
                index++;
            }
            index += 3; // skip ://
            protocol.Append("://");
            if (index >= url1.Length || index >= url2.Length)
            {
                return "";
            }
            protocol.Append(getCommonPathAncestor(url1.Substring(index), url2.Substring(index)));
            return protocol.ToString();
        }

        /// <summary>
        /// Condences the paths.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <param name="condencedPaths">The condenced path.</param>
        /// <param name="removeRedundantPaths">if set to <c>true</c> removes redundant paths.</param>
        /// <returns></returns>
        public static string condencePaths(string[] paths, IList condencedPaths, bool removeRedundantPaths)
        {
            if (paths == null || paths.Length == 0)
            {
                return null;
            }

            if (paths.Length == 1)
            {
                return paths[0];
            }
            String rootPath = paths[0];
            for (int i = 0; i < paths.Length; i++)
            {
                String url = paths[i];
                rootPath = getCommonPathAncestor(rootPath, url);
            }
            if (condencedPaths != null && removeRedundantPaths)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    String path1 = paths[i];
                    if (path1 == null)
                    {
                        continue;
                    }
                    for (int j = 0; j < paths.Length; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }
                        String path2 = paths[j];
                        if (path2 == null)
                        {
                            continue;
                        }
                        String common = getCommonPathAncestor(path1, path2);

                        if ("".Equals(common) || common == null)
                        {
                            continue;
                        }
                        // remove logner path here
                        if (common.Equals(path1))
                        {
                            paths[j] = null;
                        }
                        else if (common.Equals(path2))
                        {
                            paths[i] = null;
                        }
                    }
                }
                for (int j = 0; j < paths.Length; j++)
                {
                    String path = paths[j];
                    if (path != null && path.Equals(rootPath))
                    {
                        paths[j] = null;
                    }
                }
            }

            if (condencedPaths != null)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    String path = paths[i];
                    if (path == null)
                    {
                        continue;
                    }
                    if (rootPath != null && !"".Equals(rootPath))
                    {
                        path = path.Substring(rootPath.Length);
                        if (path.StartsWith("/"))
                        {
                            path = path.Substring(1);
                        }
                    }

                    condencedPaths.Add(path);
                }
            }
            return rootPath;
        }

        /// <summary>
        /// Resolves a path (".", "..". etc.) and returns the resolved path
        /// </summary>
        /// <param name="path">Path to be resolved</param>
        /// <returns>Resolved path</returns>
        public static String validateFilePath(String path)
        {
            path = path.Replace(Path.DirectorySeparatorChar, '/');
            Uri uriPath = new Uri(path);
            return uriPath.AbsolutePath;
        }

        /// <summary>
        /// Determines whether [is child of] [the specified parent file].
        /// </summary>
        /// <param name="parentFile">The parent file.</param>
        /// <param name="childFile">The child file.</param>
        /// <returns>
        /// 	<c>true</c> if [is child of] [the specified parent file]; otherwise, <c>false</c>.
        /// </returns>
        public static bool isChildOf(FileInfo parentFile, FileInfo childFile)
        {
            if (parentFile == null || childFile == null)
            {
                return false;
            }
            childFile = new FileInfo(validateFilePath(new FileInfo(childFile.DirectoryName).FullName));
            parentFile = new FileInfo(validateFilePath(parentFile.FullName));
            while (childFile != null)
            {
                if (childFile.Equals(parentFile))
                {
                    return true;
                }
                childFile = new FileInfo(childFile.DirectoryName);
            }
            return false;
        }

        /// <summary>
        /// Tails the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static String tail(String path)
        {
            int index = path.Length - 1;
            if (index >= 0 && index < path.Length && path[index] == '/')
            {
                index--;
            }
            for (int i = index; i >= 0; i--)
            {
                if (path[i] == '/')
                {
                    return path.Substring(i + 1, (index + 1) - (i + 1));
                }
            }
            return path;
        }

        /// <summary>
        /// Heads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static String head(String path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '/')
                {
                    return path.Substring(0, (i) - (0));
                }
            }
            return path;
        }

        /// <summary>
        /// Removes the head from the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static String removeHead(String path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '/')
                {
                    int ind = i;
                    for (; ind < path.Length; ind++)
                    {
                        if (path[ind] == '/')
                        {
                            continue;
                        }
                        break;
                    }
                    return path.Substring(ind);
                }
            }
            return "";
        }

        /// <summary>
        /// Gets the segments count.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static int getSegmentsCount(String path)
        {
            int count = path.Length > 0 ? 1 : 0;
            // skipe first char, then count number of '/'
            for (int i = 1; i < path.Length; i++)
            {
                if (path[i] == '/')
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Determines whether the specified parent path is ancestor.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="ancestorPath">The ancestor path.</param>
        /// <returns>
        /// 	<c>true</c> if the specified parent path is ancestor; otherwise, <c>false</c>.
        /// </returns>
        public static bool isAncestor(String parentPath, String ancestorPath)
        {
            parentPath = parentPath == null ? "" : parentPath;
            ancestorPath = ancestorPath == null ? "" : ancestorPath;

            if (parentPath.Length == 0)
            {
                return !ancestorPath.StartsWith("/");
            }

            if (ancestorPath.StartsWith(parentPath))
            {
                return
                    parentPath.Length == ancestorPath.Length || parentPath.EndsWith("/") ||
                    ancestorPath[parentPath.Length] == '/';
            }
            return false;
        }

        /// <summary>
        /// Determines whether the first path is a child of second path
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="pathChild">The child path</param>
        /// <returns>The child path</returns>
        public static String pathIsChild(String path, String pathChild)
        {
            if (path == null || pathChild == null)
            {
                return null;
            }
            if (String.CompareOrdinal(pathChild, path) == 0)
            {
                return null;
            }
            int count;
            for (count = 0; count < path.Length && count < pathChild.Length; count++)
            {
                if (path[count] != pathChild[count])
                {
                    return null;
                }
            }
            if (count == path.Length && count < pathChild.Length)
            {
                if (pathChild[count] == '/')
                {
                    return pathChild.Substring(count + 1);
                }
                else if (count == 1 && path[0] == '/')
                {
                    return pathChild.Substring(1);
                }
            }
            return null;
        }

        public static String append(String f, String s)
        {
            f = f == null ? "" : f;
            s = s == null ? "" : s;
            int l1 = f.Length;
            int l2 = s.Length;
            char[] r = new char[l1 + l2 + 2];
            int index = 0;
            for (int i = 0; i < l1; i++)
            {
                char ch = f[i];
                if (i + 1 == l1 && ch == '/')
                {
                    break;
                }
                r[index++] = ch;
            }
            for (int i = 0; i < l2; i++)
            {
                char ch = s[i];
                if (i == 0 && ch != '/' && index > 0)
                {
                    r[index++] = '/';
                }
                if (i + 1 == l2 && ch == '/')
                {
                    break;
                }
                r[index++] = ch;
            }
            return new String(r, 0, index);
        }
    }
}