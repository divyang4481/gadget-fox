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
using DotSVN.Common;
using DotSVN.Common.Util;
using DotSVN.Server.RepositoryAccess.File;

namespace DotSVN.Server.FS
{
    /// <summary>
    /// Represents a FS Revision Node
    /// </summary>
    public class FSRevisionNode
    {
        #region Constants
        // rev-node files keywords
        public const String HEADER_COPYFROM = "copyfrom";
        public const String HEADER_COPYROOT = "copyroot";
        public const String HEADER_COUNT = "count";
        public const String HEADER_CPATH = "cpath";
        public const String HEADER_ID = "id";
        public const String HEADER_PRED = "pred";
        public const String HEADER_PROPS = "props";
        public const String HEADER_TEXT = "text";
        public const String HEADER_TYPE = "type";
        #endregion

        #region Private fields

        // id: a.b.r<revID>/offset
        private FSID id;                    

        // type: 'dir' or 'file'
        private SVNNodeKind nodeKind;

        // count: count of revs since base
        private long myCount;
        
        // (_)a.(_)b.tx-y
        // pred: a.b.r<revID>/offset
        private FSID predecessorId;

        // text: <rev> <offset> <length> <size> <digest>
        private FSRepresentation textRepresentation;

        // props: <rev> <offset> <length> <size> <digest>
        private FSRepresentation propsRepresentation;
       
        // cpath: <path>
        private String createdPath;

        // copyfrom: <revID> <path>
        private String copyFromPath;
        private long copyFromRevision;

        // copyroot: <revID> <created-path>
        private String copyRootPath;
        private long copyRootRevision;

        // for only node-revs representing dirs
        private IDictionary<string, FSEntry> dirContents;

        #endregion


        /// <summary>
        /// Gets or sets the ID of the revision.
        /// </summary>
        /// <value>The ID.</value>
        public virtual FSID ID
        {
            get { return id; }

            set { id = value; }
        }

        /// <summary>
        /// Gets or sets the type of the revision node.
        /// </summary>
        /// <value>The type.</value>
        public virtual SVNNodeKind Type
        {
            get { return nodeKind; }

            set { nodeKind = value; }
        }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        public virtual long Count
        {
            get { return myCount; }

            set { myCount = value; }
        }

        /// <summary>
        /// Gets or sets the predecessor id.
        /// </summary>
        /// <value>The predecessor id.</value>
        public virtual FSID PredecessorId
        {
            get { return predecessorId; }

            set { predecessorId = value; }
        }

        /// <summary>
        /// Gets or sets the text representation.
        /// </summary>
        /// <value>The text representation.</value>
        public virtual FSRepresentation TextRepresentation
        {
            // text

            get { return textRepresentation; }

            set { textRepresentation = value; }
        }

        /// <summary>
        /// Gets or sets the props representation.
        /// </summary>
        /// <value>The props representation.</value>
        public virtual FSRepresentation PropsRepresentation
        {
            // props

            get { return propsRepresentation; }

            set { propsRepresentation = value; }
        }

        /// <summary>
        /// Gets or sets the created path.
        /// </summary>
        /// <value>The created path.</value>
        public virtual String CreatedPath
        {
            get { return createdPath; }

            set { createdPath = value; }
        }

        /// <summary>
        /// Gets or sets the copy from revision.
        /// </summary>
        /// <value>The copy from revision.</value>
        public virtual long CopyFromRevision
        {
            get { return copyFromRevision; }

            set { copyFromRevision = value; }
        }

        /// <summary>
        /// Gets or sets the copy from path.
        /// </summary>
        /// <value>The copy from path.</value>
        public virtual String CopyFromPath
        {
            get { return copyFromPath; }

            set { copyFromPath = value; }
        }

        /// <summary>
        /// Gets or sets the copy root revision.
        /// </summary>
        /// <value>The copy root revision.</value>
        public virtual long CopyRootRevision
        {
            get { return copyRootRevision; }

            set { copyRootRevision = value; }
        }

        /// <summary>
        /// Gets or sets the copy root path.
        /// </summary>
        /// <value>The copy root path.</value>
        public virtual String CopyRootPath
        {
            get { return copyRootPath; }

            set { copyRootPath = value; }
        }

        /// <summary>
        /// Gets the file checksum.
        /// </summary>
        /// <value>The file checksum.</value>
        public virtual String FileChecksum
        {
            get
            {
                if (Type != SVNNodeKind.file)
                {
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.FS_NOT_FILE,
                                               "Attempted to get checksum of a *non*-file node");
                    SVNErrorManager.error(err);
                }
                return ((TextRepresentation != null) ? TextRepresentation.HexDigest : "");
            }
        }

        /// <summary>
        /// Gets the length of the file.
        /// </summary>
        /// <value>The length of the file.</value>
        public virtual long FileLength
        {
            get
            {
                if (Type != SVNNodeKind.file)
                {
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.FS_NOT_FILE, "Attempted to get length of a *non*-file node");
                    SVNErrorManager.error(err);
                }
                return TextRepresentation != null ? TextRepresentation.ExpandedSize : 0;
            }
        }

        public static FSRevisionNode dumpRevisionNode(FSRevisionNode revNode)
        {
            FSRevisionNode clone = new FSRevisionNode();
            clone.ID = revNode.ID;
            if (revNode.PredecessorId != null)
            {
                clone.PredecessorId = revNode.PredecessorId;
            }
            clone.Type = revNode.Type;
            clone.CopyFromPath = revNode.CopyFromPath;
            clone.CopyFromRevision = revNode.CopyFromRevision;
            clone.CopyRootPath = revNode.CopyRootPath;
            clone.CopyRootRevision = revNode.CopyRootRevision;
            clone.Count = revNode.Count;
            clone.CreatedPath = revNode.CreatedPath;
            if (revNode.PropsRepresentation != null)
            {
                clone.PropsRepresentation = new FSRepresentation(revNode.PropsRepresentation);
            }
            if (revNode.TextRepresentation != null)
            {
                clone.TextRepresentation = new FSRepresentation(revNode.TextRepresentation);
            }
            return clone;
        }

        protected internal virtual IDictionary<string, FSEntry> getDirContents()
        {
            return dirContents;
        }

        public virtual void setDirContents(IDictionary<string, FSEntry> directoryContents)
        {
            dirContents = directoryContents;
        }

        public static FSRevisionNode fromMap(IDictionary<string, string> headers)
        {
            FSRevisionNode revNode = new FSRevisionNode();

            // Read the rev-node id.
            String revNodeId;
            headers.TryGetValue(HEADER_ID, out revNodeId);
            if (revNodeId == null)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Missing node-id in node-rev");
                SVNErrorManager.error(err);
            }

            FSID revnodeID = FSID.FromString(revNodeId);
            if (revnodeID == null)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Corrupt node-id in node-rev");
                SVNErrorManager.error(err);
                return null;
            }
            revNode.ID = revnodeID;

            // Read the type.
            SVNNodeKind nodeKind = (SVNNodeKind) Enum.Parse( typeof(SVNNodeKind), headers[HEADER_TYPE] );
            if (nodeKind == SVNNodeKind.none || nodeKind == SVNNodeKind.unknown)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Missing kind field in node-rev");
                SVNErrorManager.error(err);
            }
            revNode.Type = nodeKind;

            // Read the 'count' field.
            String countString;
            headers.TryGetValue(HEADER_COUNT, out countString);
            if (countString == null)
            {
                revNode.Count = 0;
            }
            else
            {
                long cnt = - 1;
                try
                {
                    cnt = Int64.Parse(countString);
                }
                catch (FormatException formatException)
                {
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Corrupt count field in node-rev");
                    SVNErrorManager.error(err, formatException);
                }
                revNode.Count = cnt;
            }

            // Get the properties location (if any).
            if (headers.ContainsKey(HEADER_PROPS))
            {
                String propsRepr = headers[HEADER_PROPS];
                if (propsRepr != null)
                {
                    parseRepresentationHeader(propsRepr, revNode, revnodeID.TransactionID, false);
                }
            }

            // Get the data location (if any).
            String textRepr;
            headers.TryGetValue(HEADER_TEXT, out textRepr);
            if (textRepr != null)
            {
                parseRepresentationHeader(textRepr, revNode, revnodeID.TransactionID, true);
            }

            // Get the created path.
            String cpath;
            headers.TryGetValue(HEADER_CPATH, out cpath);
            if (cpath == null)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Missing cpath in node-rev");
                SVNErrorManager.error(err);
            }
            revNode.CreatedPath = cpath;

            // Get the predecessor rev-node id (if any).
            String predId;
            headers.TryGetValue(HEADER_PRED, out predId);
            if (predId != null)
            {
                FSID predRevNodeId = FSID.FromString(predId);
                if (predRevNodeId == null)
                {
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Corrupt predecessor node-id in node-rev");
                    SVNErrorManager.error(err);
                }
                revNode.PredecessorId = predRevNodeId;
            }

            // Get the copyroot.
            String copyroot;
            headers.TryGetValue(HEADER_COPYROOT, out copyroot);
            if (copyroot == null)
            {
                revNode.CopyRootPath = revNode.CreatedPath;
                revNode.CopyRootRevision = revNode.ID.Revision;
            }
            else
            {
                parseCopyRoot(copyroot, revNode);
            }

            // Get the copyfrom.
            if (headers.ContainsKey(HEADER_COPYFROM))
            {
                String copyfrom = headers[HEADER_COPYFROM];
                if (copyfrom == null)
                {
                    revNode.CopyFromPath = null;
                    revNode.CopyFromRevision = FSRepository.SVN_INVALID_REVNUM;
                }
                else
                {
                    parseCopyFrom(copyfrom, revNode);
                }
            }

            return revNode;
        }

        private static void parseCopyFrom(String copyfrom, FSRevisionNode revNode)
        {
            if (copyfrom == null || copyfrom.Length == 0)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed copyfrom line in node-rev");
                SVNErrorManager.error(err);
                return;
            }

            int delimiterInd = copyfrom.IndexOf(' ');
            if (delimiterInd == - 1)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed copyfrom line in node-rev");
                SVNErrorManager.error(err);
            }

            String copyfromRev = copyfrom.Substring(0, (delimiterInd) - (0));
            String copyfromPath = copyfrom.Substring(delimiterInd + 1);

            long rev = - 1;
            try
            {
                rev = Int64.Parse(copyfromRev);
            }
            catch (FormatException formatException)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed copyfrom line in node-rev");
                SVNErrorManager.error(err, formatException);
            }
            revNode.CopyFromRevision = rev;
            revNode.CopyFromPath = copyfromPath;
        }

        private static void parseCopyRoot(String copyroot, FSRevisionNode revNode)
        {
            if (copyroot == null || copyroot.Length == 0)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed copyroot line in node-rev");
                SVNErrorManager.error(err);
                return;
            }

            int delimiterInd = copyroot.IndexOf(' ');
            if (delimiterInd == - 1)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed copyroot line in node-rev");
                SVNErrorManager.error(err);
            }

            String copyrootRev = copyroot.Substring(0, (delimiterInd) - (0));
            String copyrootPath = copyroot.Substring(delimiterInd + 1);

            long rev = - 1;
            try
            {
                rev = Int64.Parse(copyrootRev);
            }
            catch (FormatException formatException)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed copyroot line in node-rev");
                SVNErrorManager.error(err, formatException);
            }
            revNode.CopyRootRevision = rev;
            revNode.CopyRootPath = copyrootPath;
        }

        private static void parseRepresentationHeader(String representation, FSRevisionNode revNode, String txnId,
                                                      bool isData)
        {
            if (revNode == null)
            {
                return;
            }

            FSRepresentation rep = new FSRepresentation();

            int delimiterInd = representation.IndexOf(' ');
            String revision;
            if (delimiterInd == - 1)
            {
                revision = representation;
            }
            else
            {
                revision = representation.Substring(0, (delimiterInd) - (0));
            }

            long rev = - 1;
            try
            {
                rev = Int64.Parse(revision);
            }
            catch (FormatException formatException)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed text rep offset line in node-rev");
                SVNErrorManager.error(err, formatException);
            }
            rep.Revision = rev;

            if (FSRepository.isInvalidRevision(rep.Revision))
            {
                rep.TransactionId = txnId;
                if (isData)
                {
                    revNode.TextRepresentation = rep;
                }
                else
                {
                    revNode.PropsRepresentation = rep;
                }
                // is it a mutable representation?
                if (!isData || revNode.Type == SVNNodeKind.dir)
                {
                    return;
                }
            }

            representation = representation.Substring(delimiterInd + 1);

            delimiterInd = representation.IndexOf(' ');
            if (delimiterInd == - 1)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed text rep offset line in node-rev");
                SVNErrorManager.error(err);
            }
            String repOffset = representation.Substring(0, (delimiterInd) - (0));

            long offset = - 1;
            try
            {
                offset = Int64.Parse(repOffset);
                if (offset < 0)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException formatException)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed text rep offset line in node-rev");
                SVNErrorManager.error(err, formatException);
            }
            rep.Offset = offset;

            representation = representation.Substring(delimiterInd + 1);
            delimiterInd = representation.IndexOf(' ');
            if (delimiterInd == - 1)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed text rep offset line in node-rev");
                SVNErrorManager.error(err);
            }
            String repSize = representation.Substring(0, (delimiterInd) - (0));

            long size = - 1;
            try
            {
                size = Int64.Parse(repSize);
                if (size < 0)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException formatException)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed text rep offset line in node-rev");
                SVNErrorManager.error(err, formatException);
            }
            rep.Size = size;

            representation = representation.Substring(delimiterInd + 1);
            delimiterInd = representation.IndexOf(' ');
            if (delimiterInd == - 1)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed text rep offset line in node-rev");
                SVNErrorManager.error(err);
            }
            String repExpandedSize = representation.Substring(0, (delimiterInd) - (0));

            long expandedSize = - 1;
            try
            {
                expandedSize = Int64.Parse(repExpandedSize);
                if (expandedSize < 0)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException formatException)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed text rep offset line in node-rev");
                SVNErrorManager.error(err, formatException);
            }
            rep.ExpandedSize = expandedSize;

            String hexDigest = representation.Substring(delimiterInd + 1);
            if (hexDigest.Length != 32)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed text rep offset line in node-rev");
                SVNErrorManager.error(err);
            }
            rep.HexDigest = hexDigest;
            if (isData)
            {
                revNode.TextRepresentation = rep;
            }
            else
            {
                revNode.PropsRepresentation = rep;
            }
        }

        public virtual FSRevisionNode getChildDirNode(String childName, FSFS fsfsOwner)
        {
            if (!SVNPathUtil.isSinglePathComponent(childName))
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_NOT_SINGLE_PATH_COMPONENT,
                                           "Attempted to open node with an illegal name ''{0}''", childName);
                SVNErrorManager.error(err);
            }

            IDictionary<string, FSEntry> entries = getDirEntries(fsfsOwner);
            FSEntry entry = entries != null ? entries[childName] : null;

            if (entry == null)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_NOT_FOUND,
                                           "Attempted to open non-existent child node ''{0}''", childName);
                SVNErrorManager.error(err);
                return null;
            }

            return fsfsOwner.getRevisionNode(entry.Id);
        }

        public virtual IDictionary<string, FSEntry> getDirEntries(FSFS fsfsOwner)
        {
            if (Type != SVNNodeKind.dir)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_NOT_DIRECTORY, "Can't get entries of non-directory");
                SVNErrorManager.error(err);
            }
            IDictionary<string, FSEntry> directoryContents = getDirContents();

            if (directoryContents == null)
            {
                directoryContents = fsfsOwner.getDirContents(this);
                setDirContents(directoryContents);
            }
            return directoryContents;
        }

        public virtual IDictionary<string, string> getProperties(FSFS fsfsOwner)
        {
            return fsfsOwner.GetProperties(this);
        }

        public virtual FSRepresentation chooseDeltaBase(FSFS fsfsOwner)
        {
            if (Count == 0)
            {
                return null;
            }

            long count = Count;
            count = count & (count - 1);
            FSRevisionNode baseNode = this;
            while ((count++) < Count)
            {
                baseNode = fsfsOwner.getRevisionNode(baseNode.PredecessorId);
            }
            return baseNode.TextRepresentation;
        }
    }
}