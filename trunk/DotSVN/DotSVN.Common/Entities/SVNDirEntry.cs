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
using System.Text;
using DotSVN.Common.Util;

namespace DotSVN.Common.Entities
{
    /// <summary>
    /// The <c>SVNDirEntry</c> class is a representation of a versioned directory entry.
    /// <para><c>SVNDirEntry</c> keeps an entry name, entry kind (is it a file or directory),
    /// file size (in case an entry is a file), the last changed revision, the date when
    /// the entry was last changed, the name of the author who last changed the entry, the
    /// commit log message for the last changed revision.
    /// <c>SVNDirEntry</c> also knows if the entry has any properties.</para>
    /// </summary>
    public class SVNDirEntry
    {
        private String myCommitMessage;
        private DateTime myCreatedDate;
        private bool myHasProperties;
        private SVNNodeKind myKind;
        private String myLastAuthor;
        private SVNLock myLock;
        private String myName;
        private String myPath;
        private long myRevision;
        private long mySize;
        private SVNURL myURL;

        /// <summary>Constructs an instance of <c>SVNDirEntry</c>.</summary>
        /// <param name="url">Url of this entry</param>
        /// <param name="name">Entry name</param>
        /// <param name="kind">Node kind for the entry</param>
        /// <param name="size">Entry size in bytes</param>
        /// <param name="hasProperties">True if the entry has properties, otherwise False</param>
        /// <param name="revision">Last changed revision of the entry</param>
        /// <param name="createdDate">Date the entry was last changed</param>
        /// <param name="lastAuthor">The person who last changed the entry</param>
        public SVNDirEntry(SVNURL url, String name, SVNNodeKind kind, long size, bool hasProperties, long revision,
                           ref DateTime createdDate, String lastAuthor)
        {
            myURL = url;
            myName = name;
            myKind = kind;
            mySize = size;
            myHasProperties = hasProperties;
            myRevision = revision;
            myCreatedDate = createdDate;
            myLastAuthor = lastAuthor;
        }

        /// <summary> Constructs an instance of <c>SVNDirEntry</c>.
        /// <param name="url">Url of this entry</param>
        /// <param name="name">Entry name</param>
        /// <param name="kind">Node kind for the entry</param>
        /// <param name="size">Entry size in bytes</param>
        /// <param name="hasProperties">True if the entry has properties, otherwise False</param>
        /// <param name="revision">Last changed revision of the entry</param>
        /// <param name="createdDate">Date the entry was last changed</param>
        /// <param name="lastAuthor">The person who last changed the entry</param>
        /// <param name="commitMessage">The log message of the last change commit</param></summary>
        public SVNDirEntry(SVNURL url, String name, SVNNodeKind kind, long size, bool hasProperties, long revision,
                           ref DateTime createdDate, String lastAuthor, String commitMessage)
        {
            myURL = url;
            myName = name;
            myKind = kind;
            mySize = size;
            myHasProperties = hasProperties;
            myRevision = revision;
            myCreatedDate = createdDate;
            myLastAuthor = lastAuthor;
            myCommitMessage = commitMessage;
        }

        /// <summary>Gets the entry's URL.</summary>
        public SVNURL URL
        {
            get { return myURL; }
        }

        /// <summary> Gets the the directory entry name</summary>
        public String Name
        {
            get { return myName; }
        }

        /// <summary>Gets the file size in bytes (if this entry is a file).</summary>
        public long Size
        {
            get { return mySize; }
        }

        /// <summary>Gets the entry node kind.</summary>
        public SVNNodeKind Kind
        {
            get { return myKind; }
        }

        /// <summary>Gets the date the entry was last changed.</summary>
        public DateTime Date
        {
            get { return myCreatedDate; }
        }

        /// <summary> Gets the last changed revision of this entry.</summary>
        public long Revision
        {
            get { return myRevision; }
        }

        /// <summary>Gets the name of the author who last changed this entry. </summary>
        public string Author
        {
            get { return myLastAuthor; }
        }

        /// <summary>Gets and Sets the entry's path.
        /// <para>Always returns the name of an entry (i.e. a path relative to the parent folder) when an <c>SVNDirEntry</c> 
        /// object is provided by an SVNRepository driver.</para></summary>
        public String RelativePath
        {
            get { return myPath == null ? Name : myPath; }

            set { myPath = value; }
        }


        /// <summary>Gets and Sets the commit log message for the revision of this entry.</summary>
        public String CommitMessage
        {
            get { return myCommitMessage; }

            set { myCommitMessage = value; }
        }

        /// <summary> Gets the lock object for this entry (if it's locked).</summary>
        public SVNLock Lock
        {
            get { return myLock; }

            set { myLock = value; }
        }

        /// <summary> Tells if the entry has any properties.</summary>
        /// <returns> 	<span class="javakeyword">true</span> if has, 
        /// <span class="javakeyword">false</span> otherwise
        /// </returns>
        public bool HasProperties
        {
            get { return myHasProperties; }
        }

        /// <summary>
        /// Returns a String that represents the current Object.
        /// </summary>
        /// <returns>A String that represents the current Object.</returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("name=");
            result.Append(myName);
            result.Append(", kind=");
            result.Append(myKind);
            result.Append(", size=");
            result.Append(mySize);
            result.Append(", hasProps=");
            result.Append(myHasProperties);
            result.Append(", lastchangedrev=");
            result.Append(myRevision);
            if (myLastAuthor != null)
            {
                result.Append(", lastauthor=");
                result.Append(myLastAuthor);
            }
            if (myCreatedDate != null)
            {
                result.Append(", lastchangeddate=");
                result.Append(myCreatedDate.ToLocalTime().ToString());
            }
            return result.ToString();
        }
    }
}