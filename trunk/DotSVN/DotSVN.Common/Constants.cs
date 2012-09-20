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
using System.Collections.Specialized;

namespace DotSVN.Common
{
    /// <summary>
    /// The <c>SVNRevisionProperty</c> class represents revision properties - those
    /// unversioned properties supported by Subversion.
    /// <para>Revision properties are unversioned, so there is always a risk to lose information when modifying revision property values.</para>
    /// </summary>
    public class SVNRevisionProperty
    {
#pragma warning disable 1591
        /// <summary> An "svn:author" revision property (that holds the name of the revision's author).</summary>
        public const String AUTHOR = "svn:author";
        public const String AUTOVERSIONED = "svn:autoversioned";
        public const String CURRENTLY_COPYING = "svn:sync-currently-copying";
        /// <summary> An "svn:date" revision property that is a date and time stamp representing the time when the revision was created.</summary>
        public const String DATE = "svn:date";
        public const String FROM_URL = "svn:sync-from-url";
        public const String FROM_UUID = "svn:sync-from-uuid";
        public const String LAST_MERGED_REVISION = "svn:sync-last-merged-rev";
        public const String LOCK = "svn:sync-lock";
        /// <summary>An "svn:log" revision property -  the one that stores a log message attached to a revision during a commit operation.</summary>
        public const String LOG = "svn:log";
        public const String ORIGINAL_DATE = "svn:original-date";
        private static readonly StringCollection REVISION_PROPS = new StringCollection();
#pragma warning restore 1591

        static SVNRevisionProperty()
        {
            {
                REVISION_PROPS.Add(AUTHOR);
                REVISION_PROPS.Add(LOG);
                REVISION_PROPS.Add(DATE);
                REVISION_PROPS.Add(ORIGINAL_DATE);
                REVISION_PROPS.Add(AUTOVERSIONED);
            }
        }

        /// <summary>
        /// Says if the given revision property name is really a valid revision property name.
        /// </summary>
        /// <param name="name">a property name</param>
        /// <returns>
        /// True if it's a revision property name, False otherwise
        /// </returns>
        public static bool IsRevisionProperty(String name)
        {
            return name != null && REVISION_PROPS.Contains(name);
        }
    }

    /// <summary>The enum is used to describe the kind of a directory entry (node, in other words). 
    /// This can be:
    /// <ul>
    /// <li>a directory - The node is a directory</li>
    /// <li>a file      - The node is a file</li>
    /// <li>none        - The node is missing (does not exist)</li>
    /// <li>unknown     - The node kind can not be recognized</li>
    /// </ul>
    /// </summary>
    public enum SVNNodeKind
    {
        /// <summary>The node is a directory </summary>
        dir = 0,
        /// <summary>The node is a file </summary>
        file = 1,
        /// <summary>The node is missing (does not exist) </summary>
        none = 2,
        /// <summary>The node kind can not be recognized </summary>
        unknown = 3
    }
}