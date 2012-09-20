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
using System.Text.RegularExpressions;
using DotSVN.Common.Entities;
using DotSVN.Common.Util;
using DotSVN.Server.RepositoryAccess.File;

namespace DotSVN.Server.FS
{
    /// <summary> The kind of change that occurred on the path.
    /// </summary>
    public enum FSPathChangeKind
    {
        ///<summary>Modify</summary>
        modify = 0,
        ///<summary>Add</summary>
        add,
        ///<summary>Delete</summary>
        delete,
        ///<summary>Replace</summary>
        replace,
        ///<summary>Reset</summary>
        reset 
    }

    [Serializable]
    public class FSPathChange : SVNLogEntryPath
    {
        private String myPath;
        private FSID myRevNodeId;
        private FSPathChangeKind myChangeKind;
        private bool isTextModified;
        private bool isPropertyModified;
        private static readonly string revisionIDPattern = @"^(?'NodeRevId'[\S]*)\s(?'Action'[\S]*)\s(?'TextMode'[\S]*)\s(?'PropMode'[\S]*)\s(?'Path'.*)$";
        private static readonly Regex revisionIDRegex = new Regex(revisionIDPattern, RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly string copyFromPattern = @"^(?'CopyFromRev'[\S]*)\s(?'CopyFromPath'.*)$";
        private static readonly Regex copyFromRegex = new Regex(FSPathChange.copyFromPattern, RegexOptions.Singleline | RegexOptions.Compiled);

        public override String Path
        {
            get { return myPath; }
        }

        /// <summary>
        /// Gets or sets the kind of the change.
        /// </summary>
        /// <value>The kind of the change.</value>
        public FSPathChangeKind ChangeKind
        {
            get { return myChangeKind; }

            set
            {
                myChangeKind = value;
                // Return the enum's name as a character
                base.ChangeType = GetType(value);
            }
        }

        /// <summary>
        /// Gets or sets the revision node id.
        /// </summary>
        /// <value>The revision node id.</value>
        public FSID RevNodeId
        {
            get { return myRevNodeId; }

            set { myRevNodeId = value; }
        }

        /// <summary>
        /// Sets a value indicating whether one or more properties has been modified.
        /// </summary>
        /// <value><c>true</c> if any property is modified; otherwise, <c>false</c>.</value>
        public bool IsPropertyModified
        {
            get { return isPropertyModified; }

            set { isPropertyModified = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text has been modified.
        /// </summary>
        /// <value><c>true</c> if text is modified; otherwise, <c>false</c>.</value>
        public bool IsTextModified
        {
            get { return isTextModified; }

            set { isTextModified = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FSPathChange"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="id">The id.</param>
        /// <param name="kind">The <see cref="FSPathChangeKind"/>.</param>
        /// <param name="textModified">if set to <c>true</c> the text is modified.</param>
        /// <param name="propsModified">if set to <c>true</c> the props is modified.</param>
        /// <param name="copyfromPath">The copyfrom path.</param>
        /// <param name="copyfromRevision">The copyfrom revision.</param>
        public FSPathChange(String path,
                            FSID id,
                            FSPathChangeKind kind,
                            bool textModified,
                            bool propsModified,
                            String copyfromPath,
                            long copyfromRevision) : base(path, GetType(kind), copyfromPath, copyfromRevision)
        {
            myPath = path;
            myRevNodeId = id;
            myChangeKind = kind;
            isTextModified = textModified;
            isPropertyModified = propsModified;
        }

        private static char GetType(FSPathChangeKind kind)
        {
            return kind.ToString()[0];
        }

        /// <summary>
        /// Gets a <see cref="FSPathChangeKind"/> from the given string
        /// </summary>
        /// <param name="changesKindStr">String to be resolved as a <see cref="FSPathChangeKind"/></param>
        /// <returns></returns>
        private static FSPathChangeKind GetPathChangeKind(string changesKindStr)
        {
            FSPathChangeKind changeKind = FSPathChangeKind.replace;
            try
            {
                changeKind = (FSPathChangeKind) Enum.Parse(typeof (FSPathChangeKind), changesKindStr);
            }
            catch
            {
            }
            return changeKind;
        }

        /// <summary>
        /// Gets a <see cref="FSPathChange"/> from a changeline and copyline string.
        /// </summary>
        /// <param name="changeLine">The change line.</param>
        /// <param name="copyFromLine">The copyfrom line.</param>
        /// <returns></returns>
        public static FSPathChange fromString(String changeLine, String copyFromLine)
        {
            // Extract the Node Revision ID, Change kind, Text Modified flag, Props modified flag and path
            //  from the change line. The change line has the following format:
            //  "<id> <action> <text-mod> <prop-mod> <path>\n", 
            //  where <id> is the node-rev ID of the new node-rev, 
            //  <action> is "add", "delete", "replace", or "modify", 
            //  <text-mod> and <prop-mod> are "true" or "false" indicating whether 
            //  the text and/or properties changed, and 
            //  <path> is the changed pathname. 
            Match match = revisionIDRegex.Match(changeLine);

            if (!match.Success)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Invalid changes line in rev-file");
                SVNErrorManager.error(err);
            }
            
            // The regex match succeeded, now get the tokens one by one

            // 1. Get Node Revision ID and convert to FSID
            string nodeRevIdStr = match.Groups["NodeRevId"].Value;
            FSID nodeRevID = FSID.FromString(nodeRevIdStr);

            // 2. Get Action and convert to ChangeKind
            string actionStr = match.Groups["Action"].Value;
            FSPathChangeKind changesKind = GetPathChangeKind(actionStr);

            // 3. Get Text mode and convert to bool
            string textModeStr = match.Groups["TextMode"].Value;
            bool textModeBool;
            if (!Boolean.TryParse(textModeStr, out textModeBool))
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Invalid text-mod flag in rev-file");
                SVNErrorManager.error(err);
            }

            // 4. Get Properties mode and convert to bool
            string propModeStr = match.Groups["PropMode"].Value;
            bool propModeBool;
            if (!Boolean.TryParse(propModeStr, out propModeBool))
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Invalid prop-mod flag in rev-file");
                SVNErrorManager.error(err);
            }

            // 5. Get the Path
            string pathStr = match.Groups["Path"].Value;

            // We are done with change line; now let's parse copy from line if present
            //  The copyfrom line has the format "<rev> <path>\n" 
            //  Extract the node revision and path from the CopyFrom line, if present.
            match = copyFromRegex.Match(copyFromLine);

            if (!match.Success)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Invalid copy from line in rev-file");
                SVNErrorManager.error(err);
            }

            string copyFromRevStr = match.Groups["CopyFromRev"].Value;
            string copyFromPath = match.Groups["CopyFromPath"].Value;

            long copyfromRevision = FSRepository.SVN_INVALID_REVNUM;
            Int64.TryParse(copyFromRevStr, out copyfromRevision);

            return
                new FSPathChange(pathStr, nodeRevID, changesKind, textModeBool, propModeBool, copyFromPath,
                                 copyfromRevision);
        }
    }
}