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
using System.Diagnostics;
using System.IO;
using System.Text;
using DotSVN.Common.Util;
using DotSVN.Server.Properties;

namespace DotSVN.Server.FS
{
    public class FSHooks
    {
        #region Constants

        public const String REVPROP_ADD = "A";
        public const String REVPROP_DELETE = "D";
        public const String REVPROP_MODIFY = "M";
        public const String SVN_REPOS_HOOK_DESC_EXT = ".tmpl";
        public const String SVN_REPOS_HOOK_POST_COMMIT = "post-commit";
        public const String SVN_REPOS_HOOK_POST_LOCK = "post-lock";
        public const String SVN_REPOS_HOOK_POST_REVPROP_CHANGE = "post-revprop-change";
        public const String SVN_REPOS_HOOK_POST_UNLOCK = "post-unlock";
        public const String SVN_REPOS_HOOK_PRE_COMMIT = "pre-commit";
        public const String SVN_REPOS_HOOK_PRE_LOCK = "pre-lock";
        public const String SVN_REPOS_HOOK_PRE_REVPROP_CHANGE = "pre-revprop-change";
        public const String SVN_REPOS_HOOK_PRE_UNLOCK = "pre-unlock";
        public const String SVN_REPOS_HOOK_READ_SENTINEL = "read-sentinels";
        public const String SVN_REPOS_HOOK_START_COMMIT = "start-commit";
        public const String SVN_REPOS_HOOK_WRITE_SENTINEL = "write-sentinels";
        public const String SVN_REPOS_HOOKS_DIR = "hooks";
        private static readonly String[] winExtensions = new String[] {".exe", ".bat", ".cmd"};

        #endregion

        private static Boolean? isHooksEnabled;

        public static bool HooksEnabled
        {
            get
            {
                if (!isHooksEnabled.HasValue)
                {
                    isHooksEnabled = Settings.Default.EnableHooks;
                }
                return isHooksEnabled.Value;
            }
        }

        public static FileInfo getHookFile(FileInfo reposRootDir, String hookName)
        {
            if (!HooksEnabled)
            {
                return null;
            }
            for (int index = 0; index < winExtensions.Length; index++)
            {
                FileInfo hookFile;
                hookFile = new FileInfo(getHooksDir(reposRootDir).FullName + "\\" + hookName + winExtensions[index]);
                if (hookFile.Exists)
                {
                    return hookFile;
                }
            }
            return null;
        }

        public static FileInfo getHooksDir(FileInfo reposRootDir)
        {
            return new FileInfo(reposRootDir.FullName + "\\" + SVN_REPOS_HOOKS_DIR);
        }

        public static void runPreLockHook(FileInfo reposRootDir, String path, String username)
        {
            runLockHook(reposRootDir, SVN_REPOS_HOOK_PRE_LOCK, path, username, null);
        }

        public static void runPostLockHook(FileInfo reposRootDir, String[] paths, String username)
        {
            StringBuilder pathsStr = new StringBuilder();
            for (int i = 0; i < paths.Length; i++)
            {
                pathsStr.Append(paths[i]);
                pathsStr.Append("\n");
            }
            runLockHook(reposRootDir, SVN_REPOS_HOOK_POST_LOCK, null, username, pathsStr.ToString());
        }

        public static void runPreUnlockHook(FileInfo reposRootDir, String path, String username)
        {
            runLockHook(reposRootDir, SVN_REPOS_HOOK_PRE_UNLOCK, path, username, null);
        }

        public static void runPostUnlockHook(FileInfo reposRootDir, String[] paths, String username)
        {
            StringBuilder pathsStr = new StringBuilder();
            for (int i = 0; i < paths.Length; i++)
            {
                pathsStr.Append(paths[i]);
                pathsStr.Append("\n");
            }
            runLockHook(reposRootDir, SVN_REPOS_HOOK_POST_UNLOCK, null, username, pathsStr.ToString());
        }

        private static void runLockHook(FileInfo reposRootDir, String hookName, String path, String username,
                                        String paths)
        {
            FileInfo hookFile = getHookFile(reposRootDir, hookName);
            if (hookFile == null)
            {
                return;
            }
            username = username == null ? "" : username;
            Process hookProc = null;
            String reposPath = reposRootDir.FullName.Replace(Path.DirectorySeparatorChar, '/');
            try
            {
                String executableName = hookFile.Name.ToLower();
                if ((executableName.EndsWith(".bat") || executableName.EndsWith(".cmd")) )
                {
                    String cmd = "cmd /C \"" + "\"" + hookFile.FullName + "\" " + "\"" + reposPath + "\" " +
                                 (path != null ? "\"" + path + "\" " : "") + "\"" + username + "\"";
                    Process.GetCurrentProcess();
                    hookProc = Process.Start(cmd);
                }
                else
                {
                    if (path != null)
                    {
                        String[] cmd =
                            new String[] {hookFile.FullName, reposPath, path, !"".Equals(username) ? username : "\"\""};
                        // TODO: Uncomment this
                        //hookProc = SupportClass.ExecSupport(cmd);
                    }
                    else
                    {
                        String[] cmd =
                            new String[] {hookFile.FullName, reposPath, !"".Equals(username) ? username : "\"\""};
                        // TODO: Uncomment this
                        // hookProc = SupportClass.ExecSupport(cmd);
                    }
                }
            }
            catch (IOException ioe)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.REPOS_HOOK_FAILURE, "Failed to start ''{0}'' hook: {1}",
                                           new Object[] {hookFile, ioe.Message});
                SVNErrorManager.error(err, ioe);
            }
            runHook(hookFile, hookName, hookProc, paths, path != null);
        }

        public static void runPreRevPropChangeHook(FileInfo reposRootDir, String propName, String propNewValue,
                                                   String author, long revision, String action)
        {
            runChangeRevPropHook(reposRootDir, SVN_REPOS_HOOK_PRE_REVPROP_CHANGE, propName, propNewValue, author,
                                 revision, action, true);
        }

        public static void runPostRevPropChangeHook(FileInfo reposRootDir, String propName, String propOldValue,
                                                    String author, long revision, String action)
        {
            runChangeRevPropHook(reposRootDir, SVN_REPOS_HOOK_POST_REVPROP_CHANGE, propName, propOldValue, author,
                                 revision, action, false);
        }

        public static void runStartCommitHook(FileInfo reposRootDir, String author)
        {
            author = author == null ? "" : author;
            runCommitHook(reposRootDir, SVN_REPOS_HOOK_START_COMMIT, author, true);
        }

        public static void runPreCommitHook(FileInfo reposRootDir, String txnName)
        {
            runCommitHook(reposRootDir, SVN_REPOS_HOOK_PRE_COMMIT, txnName, true);
        }

        public static void runPostCommitHook(FileInfo reposRootDir, long committedRevision)
        {
            runCommitHook(reposRootDir, SVN_REPOS_HOOK_POST_COMMIT, Convert.ToString(committedRevision), true);
        }

        private static void runCommitHook(FileInfo reposRootDir, String hookName, String secondArg, bool readErrorStream)
        {
            FileInfo hookFile = getHookFile(reposRootDir, hookName);
            if (hookFile == null)
            {
                return;
            }
            Process hookProc = null;
            String reposPath = reposRootDir.FullName.Replace(Path.DirectorySeparatorChar, '/');
            try
            {
                String executableName = hookFile.Name.ToLower();
                if ((executableName.EndsWith(".bat") || executableName.EndsWith(".cmd")))
                {
                    String cmd = "cmd /C \"" + "\"" + hookFile.FullName + "\" " + "\"" + reposPath + "\" " + "\"" +
                                 secondArg + "\"";
                    Process.GetCurrentProcess();
                    hookProc = Process.Start(cmd);
                }
                else
                {
                    String[] cmd =
                        new String[]
                            {
                                hookFile.FullName, reposPath,
                                secondArg != null && !"".Equals(secondArg) ? secondArg : "\"\""
                            };
                    // TODO: Uncomment this
                    // hookProc = SupportClass.ExecSupport(cmd);
                }
            }
            catch (IOException ioe)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.REPOS_HOOK_FAILURE, "Failed to start ''{0}'' hook: {1}",
                                           new Object[] {hookFile, ioe.Message});
                SVNErrorManager.error(err, ioe);
            }
            runHook(hookFile, hookName, hookProc, null, readErrorStream);
        }

        public static void runChangeRevPropHook(FileInfo reposRootDir, String hookName, String propName,
                                                String propValue, String author, long revision, String action,
                                                bool isPre)
        {
            FileInfo hookFile = getHookFile(reposRootDir, hookName);
            if (hookFile == null && isPre)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.REPOS_DISABLED_FEATURE,
                                           "Repository has not been enabled to accept revision propchanges;\nask the administrator to create a pre-revprop-change hook");
                SVNErrorManager.error(err);
            }
            else if (hookFile == null)
            {
                return;
            }
            author = author == null ? "" : author;
            Process hookProc = null;
            String reposPath = reposRootDir.FullName.Replace(Path.DirectorySeparatorChar, '/');
            try
            {
                String executableName = hookFile.Name.ToLower();
                if ((executableName.EndsWith(".bat") || executableName.EndsWith(".cmd")))
                {
                    String cmd = "cmd /C \"" + "\"" + hookFile.FullName + "\" " + "\"" + reposPath + "\" " +
                                 Convert.ToString(revision) + " " + "\"" + author + "\" " + "\"" + propName + "\" " +
                                 action + "\"";
                    Process.GetCurrentProcess();
                    hookProc = Process.Start(cmd);
                }
                else
                {
                    String[] cmd =
                        new String[]
                            {
                                hookFile.FullName, reposPath, Convert.ToString(revision),
                                !"".Equals(author) ? author : "\"\"", propName, action
                            };
                    // TODO: Uncomment this
                    // hookProc = SupportClass.ExecSupport(cmd);
                }
            }
            catch (IOException ioe)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.REPOS_HOOK_FAILURE, "Failed to start ''{0}'' hook: {1}",
                                           new Object[] {hookFile, ioe.Message});
                SVNErrorManager.error(err, ioe);
            }
            runHook(hookFile, hookName, hookProc, propValue, isPre);
        }

        private static void runHook(FileInfo hook, String hookName, Process hookProcess, String stdInValue,
                                    bool readErrorStream)
        {
//            if (hookProcess == null)
//            {
//                SVNErrorMessage err =
//                    SVNErrorMessage.create(SVNErrorCode.REPOS_HOOK_FAILURE, "Failed to start ''{0}'' hook", hook);
//                SVNErrorManager.error(err);
//            }
//            if (stdInValue != null)
//            {
//                Stream osToStdIn = hookProcess.StandardOutput.BaseStream;
//                try
//                {
//                    //UPGRADE_TODO: Method 'java.lang.String.getBytes' was converted to 'System.Text.Encoding.GetEncoding(string).GetBytes(string)' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javalangStringgetBytes_javalangString'"
//                    sbyte[] bytes = SupportClass.ToSByteArray(Encoding.GetEncoding("UTF-8").GetBytes(stdInValue));
//                    for (int i = 0; i < bytes.Length; i += 1024)
//                    {
//                        osToStdIn.Write(SupportClass.ToByteArray(bytes), i, Math.Min(1024, bytes.Length - i));
//                        osToStdIn.Flush();
//                    }
//                }
//                catch (IOException ioe)
//                {
//                    // 
//                }
//                finally
//                {
//                    SVNFileUtil.closeFile(osToStdIn);
//                }
//            }
//
//            StreamGobbler inputGobbler = new StreamGobbler(hookProcess.StandardInput.BaseStream);
//            StreamGobbler errorGobbler = new StreamGobbler(hookProcess.StandardError.BaseStream);
//            inputGobbler.Start();
//            errorGobbler.Start();
//
//            int rc = - 1;
//            try
//            {
//                hookProcess.WaitForExit();
//                rc = hookProcess.ExitCode;
//            }
//            catch (ThreadInterruptedException threadInterruptedException)
//            {
//                SVNErrorMessage err =
//                    SVNErrorMessage.create(SVNErrorCode.REPOS_HOOK_FAILURE, "Failed to start ''{0}'' hook: {1}",
//                                           new Object[] {hook, threadInterruptedException.Message});
//                SVNErrorManager.error(err, threadInterruptedException);
//            }
//            finally
//            {
//                errorGobbler.close();
//                inputGobbler.close();
//                hookProcess.Kill();
//            }
//
//            if (errorGobbler.Error != null)
//            {
//                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.IO_ERROR, errorGobbler.Error.Message);
//                SVNErrorManager.error(err, errorGobbler.Error);
//            }
//
//            if (rc != 0)
//            {
//                if (!readErrorStream)
//                {
//                    SVNErrorMessage err =
//                        SVNErrorMessage.create(SVNErrorCode.REPOS_HOOK_FAILURE,
//                                               "'{0}' hook failed; no error output available", hookName);
//                    SVNErrorManager.error(err);
//                }
//                else
//                {
//                    String errString = errorGobbler.Result;
//                    SVNErrorMessage err =
//                        SVNErrorMessage.create(SVNErrorCode.REPOS_HOOK_FAILURE,
//                                               "''{0}'' hook failed with error output:\n{1}",
//                                               new Object[] {hookName, errString});
//                    SVNErrorManager.error(err);
//                }
//            }
        }

        #region Nested type: StreamGobbler

//        private class StreamGobbler : SupportClass.ThreadClass
//        {
//            internal IOException error;
//            internal Stream is_Renamed;
//            private bool myIsClosed;
//            internal StringBuilder result;
//
//            internal StreamGobbler(Stream is_Renamed)
//            {
//                this.is_Renamed = is_Renamed;
//                result = new StringBuilder();
//            }
//
//            public virtual String Result
//            {
//                get { return result.ToString(); }
//            }
//
//            public virtual IOException Error
//            {
//                get { return error; }
//            }
//
//            public virtual void close()
//            {
//                myIsClosed = true;
//                SVNFileUtil.closeFile(is_Renamed);
//            }
//
//            public override void Run()
//            {
//                try
//                {
//                    int r;
//                    while ((r = is_Renamed.ReadByte()) >= 0)
//                    {
//                        result.Append((char) (r & 0xFF));
//                    }
//                }
//                catch (IOException ioe)
//                {
//                    if (!myIsClosed)
//                    {
//                        error = ioe;
//                    }
//                }
//                finally
//                {
//                    if (!myIsClosed)
//                    {
//                        SVNFileUtil.closeFile(is_Renamed);
//                    }
//                }
//            }
//        }

        #endregion
    }
}