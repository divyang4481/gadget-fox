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

namespace DotSVN.Server.FS
{
    public class FSRevisionRoot : FSRoot
    {
        private long changedPathOffset;
        private long myRevision;
        private long rootOffset;
        private static readonly string OffsetLinePattern = @"^(?'rootOffset'\d+)\s(?'cpOffset'\d+)$";
        private static readonly Regex OffsetLineRegex = new Regex(OffsetLinePattern, RegexOptions.Compiled | RegexOptions.Singleline);

        public FSRevisionRoot(FSFS owner, long revision) : base(owner)
        {
            myRevision = revision;
            rootOffset = - 1;
            changedPathOffset = - 1;
        }

        public virtual long Revision
        {
            get { return myRevision; }
        }

        public override IDictionary<string, FSPathChange> GetChangedPaths()
        {
            FSFile file = Owner.getRevisionFile(Revision);
            LoadOffsets(file);
            try
            {
                file.Seek(changedPathOffset);
                return fetchAllChanges(file, true);
            }
            finally
            {
                file.Close();
            }
        }

        public override FSRevisionNode RootRevisionNode
        {
            get
            {
                if (fsRootRevisionNode == null)
                {
                    FSFile file = Owner.getRevisionFile(Revision);
                    try
                    {
                        LoadOffsets(file);
                        file.Seek(rootOffset);
                        IDictionary<string, string> headers = file.ReadHeader();
                        fsRootRevisionNode = FSRevisionNode.fromMap(headers);
                    }
                    finally
                    {
                        file.Close();
                    }
                }
                return fsRootRevisionNode;
            }
        }

        public override FSCopyInheritance getCopyInheritance(FSParentPath child)
        {
            return null;
        }

        /// <summary>
        /// Load the two offsets (root offset and changed path offset) from the revision file
        /// </summary>
        /// <param name="fsRevisionFile"></param>
        private void LoadOffsets(FSFile fsRevisionFile)
        {
            if (rootOffset >= 0)
            {
                return;
            }

            string offsetLine = fsRevisionFile.ReadOffsets();
            if (!String.IsNullOrEmpty(offsetLine))
            {
                // Extract the root offset and changed path offset from this line
                // OffsetLine is of the form "[root-offset] [cp-offset]"; look for that pattern

                // match the offset line
                Match match = OffsetLineRegex.Match(offsetLine);
                string rootOffsetText = string.Empty;
                string cpOffsetText = string.Empty;
                if (match.Success)
                {
                    rootOffsetText = match.Groups["rootOffset"].Value;
                    cpOffsetText = match.Groups["cpOffset"].Value;
                }
                else
                {
                    // The regex match failed which means that header line is malformed; report error
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT,
                                               "Malformed offsets in revision file: Offset line does not match the pattern [root-offset] [cp-offset]");
                    SVNErrorManager.error(err);
                }

                // Convert the offset text to long
                bool offsetParsed = Int64.TryParse(rootOffsetText, out rootOffset);
                offsetParsed |= Int64.TryParse(cpOffsetText, out changedPathOffset);

                // If the parsing failed, report error
                if (!offsetParsed)
                {
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT,
                                               "Malformed offsets in revision file: Root offset and/or cp offset is not a number");
                    SVNErrorManager.error(err);
                }
            }
        }
    }
}