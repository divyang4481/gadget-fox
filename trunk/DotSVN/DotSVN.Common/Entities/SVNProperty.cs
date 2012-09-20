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

namespace DotSVN.Common.Entities
{
    /// <summary>
    /// The <b>SVNProperty</b> class is a representation class for both versioned
    /// properties (user-managed svn specials) and for metaproperties (untweakable)
    /// supported by Subversion. This class holds string constants that are property
    /// names, and gives some useful methods to operate with properties (in particular).
    /// </summary>
    public class SVNProperty
    {
        /// <summary> An "svn:" prefix.</summary>
        public const String SVN_PREFIX = "svn:";

        /// <summary> An "svn:wc:" prefix.</summary>
        public const String SVN_WC_PREFIX = "svn:wc:";

        /// <summary> A special property used in a commit transaction. </summary>
        public static readonly String TXN_CHECK_LOCKS = SVN_PREFIX + "check-locks";

        /// <summary> A special property used in a commit transaction. </summary>
        public static readonly String TXN_CHECK_OUT_OF_DATENESS = SVN_PREFIX + "check-ood";

        /// <summary> An "svn:entry:" prefix.</summary>
        public const String SVN_ENTRY_PREFIX = "svn:entry:";

        /// <summary> An "svn:eol-style" SVN special property.</summary>
        public static readonly String EOL_STYLE = SVN_PREFIX + "eol-style";

        /// <summary> An "svn:ignore" SVN special property.</summary>
        public static readonly String IGNORE = SVN_PREFIX + "ignore";

        /// <summary> An "svn:mime-type" SVN special property.</summary>
        public static readonly String MIME_TYPE = SVN_PREFIX + "mime-type";

        /// <summary> An "svn:keywords" SVN special property.</summary>
        public static readonly String KEYWORDS = SVN_PREFIX + "keywords";

        /// <summary> An "svn:executable" SVN special property.</summary>
        public static readonly String EXECUTABLE = SVN_PREFIX + "executable";

        /// <summary> An "svn:externals" SVN special property.</summary>
        public static readonly String EXTERNALS = SVN_PREFIX + "externals";

        /// <summary> An "svn:special" SVN special property.</summary>
        public static readonly String SPECIAL = SVN_PREFIX + "special";

        /// <summary> An "svn:entry:revision" SVN untweakable metaproperty.</summary>
        public static readonly String REVISION = SVN_ENTRY_PREFIX + "revision";

        /// <summary> An "svn:entry:committed-rev" SVN untweakable metaproperty.</summary>
        public static readonly String COMMITTED_REVISION = SVN_ENTRY_PREFIX + "committed-rev";

        /// <summary> An "svn:entry:committed-date" SVN untweakable metaproperty.</summary>
        public static readonly String COMMITTED_DATE = SVN_ENTRY_PREFIX + "committed-date";

        /// <summary> "has-props" SVN untweakable metaproperty.</summary>
        /// <since> 1.1, new in Subversion 1.4
        /// </since>
        public const String HAS_PROPS = "has-props";

        /// <summary> "has-prop-mods" SVN untweakable metaproperty.</summary>
        /// <since> 1.1, new in Subversion 1.4
        /// </since>
        public const String HAS_PROP_MODS = "has-prop-mods";

        /// <summary> "cachable-props" SVN untweakable metaproperty.</summary>
        /// <since> 1.1, new in Subversion 1.4
        /// </since>
        public const String CACHABLE_PROPS = "cachable-props";

        /// <summary> "present-props" SVN untweakable metaproperty.</summary>
        /// <since> 1.1, new in Subversion 1.4
        /// </since>
        public const String PRESENT_PROPS = "present-props";

        /// <summary> An "svn:entry:checksum" SVN untweakable metaproperty.</summary>
        public static readonly String CHECKSUM = SVN_ENTRY_PREFIX + "checksum";

        /// <summary> An "svn:entry:url" SVN untweakable metaproperty.</summary>
        public static readonly String URL = SVN_ENTRY_PREFIX + "url";

        /// <summary> An "svn:entry:copyfrom-url" SVN untweakable metaproperty.</summary>
        public static readonly String COPYFROM_URL = SVN_ENTRY_PREFIX + "copyfrom-url";

        /// <summary> An "svn:entry:copyfrom-rev" SVN untweakable metaproperty.</summary>
        public static readonly String COPYFROM_REVISION = SVN_ENTRY_PREFIX + "copyfrom-rev";

        /// <summary> An "svn:entry:schedule" SVN untweakable metaproperty.</summary>
        public static readonly String SCHEDULE = SVN_ENTRY_PREFIX + "schedule";

        /// <summary> An "svn:entry:copied" SVN untweakable metaproperty.</summary>
        public static readonly String COPIED = SVN_ENTRY_PREFIX + "copied";

        /// <summary> An "svn:entry:last-author" SVN untweakable metaproperty.</summary>
        public static readonly String LAST_AUTHOR = SVN_ENTRY_PREFIX + "last-author";

        /// <summary> An "svn:entry:uuid" SVN untweakable metaproperty.</summary>
        public static readonly String UUID = SVN_ENTRY_PREFIX + "uuid";

        /// <summary> An "svn:entry:repos" SVN untweakable metaproperty.</summary>
        public static readonly String REPOS = SVN_ENTRY_PREFIX + "repos";

        /// <summary> An "svn:entry:prop-time" SVN untweakable metaproperty.</summary>
        public static readonly String PROP_TIME = SVN_ENTRY_PREFIX + "prop-time";

        /// <summary> An "svn:entry:text-time" SVN untweakable metaproperty.</summary>
        public static readonly String TEXT_TIME = SVN_ENTRY_PREFIX + "text-time";

        /// <summary> An "svn:entry:name" SVN untweakable metaproperty.</summary>
        public static readonly String NAME = SVN_ENTRY_PREFIX + "name";

        /// <summary> An "svn:entry:kind" SVN untweakable metaproperty.</summary>
        public static readonly String KIND = SVN_ENTRY_PREFIX + "kind";

        /// <summary> An "svn:entry:conflict-old" SVN untweakable metaproperty.</summary>
        public static readonly String CONFLICT_OLD = SVN_ENTRY_PREFIX + "conflict-old";

        /// <summary> An "svn:entry:conflict-new" SVN untweakable metaproperty.</summary>
        public static readonly String CONFLICT_NEW = SVN_ENTRY_PREFIX + "conflict-new";

        /// <summary> An "svn:entry:conflict-wrk" SVN untweakable metaproperty.</summary>
        public static readonly String CONFLICT_WRK = SVN_ENTRY_PREFIX + "conflict-wrk";

        /// <summary> An "svn:entry:prop-reject-file" SVN untweakable metaproperty.</summary>
        public static readonly String PROP_REJECT_FILE = SVN_ENTRY_PREFIX + "prop-reject-file";

        /// <summary> An "svn:entry:deleted" SVN untweakable metaproperty.</summary>
        public static readonly String DELETED = SVN_ENTRY_PREFIX + "deleted";

        /// <summary> An "svn:entry:absent" SVN untweakable metaproperty.</summary>
        public static readonly String ABSENT = SVN_ENTRY_PREFIX + "absent";

        /// <summary> An "svn:entry:incomplete" SVN untweakable metaproperty.</summary>
        public static readonly String INCOMPLETE = SVN_ENTRY_PREFIX + "incomplete";

        /// <summary> An "svn:entry:corrupted" SVN untweakable metaproperty.</summary>
        public static readonly String CORRUPTED = SVN_ENTRY_PREFIX + "corrupted";

        /// <summary> An "svn:wc:ra_dav:version-url" SVN untweakable metaproperty.</summary>
        public static readonly String WC_URL = SVN_WC_PREFIX + "ra_dav:version-url";

        /// <summary> An "svn:entry:lock-token" SVN untweakable metaproperty.</summary>
        public static readonly String LOCK_TOKEN = SVN_ENTRY_PREFIX + "lock-token";

        /// <summary> An "svn:entry:lock-comment" SVN untweakable metaproperty.</summary>
        public static readonly String LOCK_COMMENT = SVN_ENTRY_PREFIX + "lock-comment";

        /// <summary> An "svn:entry:lock-owner" SVN untweakable metaproperty.</summary>
        public static readonly String LOCK_OWNER = SVN_ENTRY_PREFIX + "lock-owner";

        /// <summary> An "svn:entry:lock-creation-date" SVN untweakable metaproperty.</summary>
        public static readonly String LOCK_CREATION_DATE = SVN_ENTRY_PREFIX + "lock-creation-date";

        /// <summary> An "svn:needs-lock" SVN special property.</summary>
        public static readonly String NEEDS_LOCK = SVN_PREFIX + "needs-lock";

        /// <summary> One of the two possible values of the {@link #KIND} property - 
        /// "dir" 
        /// </summary>
        public const String KIND_DIR = "dir";

        /// <summary> One of the two possible values of the property - "file" </summary>
        public const String KIND_FILE = "file";

        /// <summary> One of the four possible values of the property - "LF" (line feed) </summary>
        public const String EOL_STYLE_LF = "LF";

        /// <summary> One of the four possible values of the property: "CR" (linefeed) </summary>
        public const String EOL_STYLE_CR = "CR";

        /// <summary> One of the four possible values of the property - "CRLF" </summary>
        public const String EOL_STYLE_CRLF = "CRLF";

        /// <summary> One of the four possible values of the property - "native" </summary>
        public const String EOL_STYLE_NATIVE = "native";

        /// <summary> One of the three possible values of the property - "add" </summary>
        public const String SCHEDULE_ADD = "add";

        /// <summary> One of the three possible values of the {@link #SCHEDULE} property - 
        /// "delete" 
        /// </summary>
        public const String SCHEDULE_DELETE = "delete";

        /// <summary> One of the three possible values of the property - "replace" </summary>
        public const String SCHEDULE_REPLACE = "replace";

        private static readonly byte[] EOL_LF_BYTES = UTF8Encoding.UTF8.GetBytes("\n");
        private static readonly byte[] EOL_CRLF_BYTES = UTF8Encoding.UTF8.GetBytes("\r\n");
        private static readonly byte[] EOL_CR_BYTES = UTF8Encoding.UTF8.GetBytes("\r");
        private static readonly byte[] EOL_NATIVE_BYTES = UTF8Encoding.UTF8.GetBytes(Environment.NewLine);

        /// <summary>
        /// Says if the given property name starts with the {@link #SVN_WC_PREFIX}
        /// prefix.
        /// </summary>
        /// <param name="name">a property name to check</param>
        /// <returns>
        /// 	<c>true</c> if <code>name</code> is not <c>null</c> and starts with
        /// the "svn:wc:" prefix, otherwise <c>false</c>
        /// </returns>
        public static bool isWorkingCopyProperty(String name)
        {
            return name != null && name.StartsWith(SVN_WC_PREFIX);
        }

        /// <summary>
        /// Says if the given property name starts with the {@link #SVN_ENTRY_PREFIX}
        /// prefix.
        /// </summary>
        /// <param name="name">a property name to check</param>
        /// <returns>
        /// 	<c>true</c> if <code>name</code> is  not <c>null</c> and starts with
        /// the "svn:wc:" prefix, otherwise <c>false</c>
        /// </returns>
        public static bool isEntryProperty(String name)
        {
            return name != null && name.StartsWith(SVN_ENTRY_PREFIX);
        }

        /// <summary> Says if the given property name starts with the "svn:wc:" prefix. </summary>
        /// <param name="name"> a property name to check
        /// </param>
        /// <returns>       <c>true</c> if <code>name</code> is
        /// not <c>null</c> and starts with
        /// the "svn:wc:" prefix, otherwise <c>false</c>
        /// </returns>
        public static bool isSVNProperty(String name)
        {
            return name != null && name.StartsWith(SVN_PREFIX);
        }

        /// <summary> Checks if a property is regular. Regular are some "svn:" 
        /// properties and all user props, i.e. ones stored in the repository filesystem.
        /// 
        /// </summary>
        /// <param name="name">a property name
        /// </param>
        /// <returns>      <c>true</c> if regular, otherwise 
        /// <c>false</c>
        /// </returns>
        public static bool isRegularProperty(String name)
        {
            if (name == null)
            {
                return false;
            }
            else if (name.StartsWith(SVN_WC_PREFIX) || name.StartsWith(SVN_ENTRY_PREFIX))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Says if the given MIME-type corresponds to a text type.
        /// </summary>
        /// <param name="mimeType">a value of a file {@link #MIME_TYPE} property</param>
        /// <returns>
        /// 	<c>true</c> if <code>mimeType</code>
        /// is either <c>null</c> or is a text
        /// type (starts with "text/")
        /// </returns>
        /// <seealso cref="isBinaryMimeType(String)">
        /// </seealso>
        public static bool isTextMimeType(String mimeType)
        {
            return mimeType == null || mimeType.StartsWith("text/");
        }

        /// <summary>
        /// Says if the given MIME-type corresponds to a binary (non-textual) type.
        /// </summary>
        /// <param name="mimeType">a value of a file {@link #MIME_TYPE} property</param>
        /// <returns>
        /// 	<c>true</c> if <code>mimeType</code>
        /// is not a text type
        /// </returns>
        /// <seealso cref="isTextMimeType(String)">
        /// </seealso>
        public static bool isBinaryMimeType(String mimeType)
        {
            return !isTextMimeType(mimeType);
        }

        /// <summary>
        /// Returns eol-marker bytes according to the given eol type.
        /// </summary>
        /// <param name="eolType">a requested eol-marker type (platform specific)</param>
        /// <returns>
        /// 	<c>null</c> if <code>eolType</code> is
        /// <c>null</c>, or an array of bytes
        /// for one of the four possible eol types
        /// </returns>
        /// <seealso cref="EOL_STYLE_CR">
        /// </seealso>
        /// <seealso cref="EOL_STYLE_CRLF">
        /// </seealso>
        /// <seealso cref="EOL_STYLE_LF">
        /// </seealso>
        /// <seealso cref="EOL_STYLE_NATIVE">
        /// </seealso>
        public static byte[] getEOLBytes(String eolType)
        {
            if (eolType == null)
            {
                return null;
            }
            else if (EOL_STYLE_NATIVE.Equals(eolType))
            {
                return EOL_NATIVE_BYTES;
            }
            else if (EOL_STYLE_CR.Equals(eolType))
            {
                return EOL_CR_BYTES;
            }
            else if (EOL_STYLE_CRLF.Equals(eolType))
            {
                return EOL_CRLF_BYTES;
            }
            return EOL_LF_BYTES;
        }

        /// <summary>
        /// Converts a string representation of a boolean value to boolean.
        /// Useful to convert values of the {@link #COPIED} property.
        /// </summary>
        /// <param name="text">a string to convert to a boolean value</param>
        /// <returns>
        /// 	<c>true</c> if and only if
        /// <code>text</code> is not <c>null</c>
        /// and is equal, ignoring case, to the string
        /// "true"
        /// </returns>
        public static bool booleanValue(String text)
        {
            return text == null ? false : Boolean.Parse(text.Trim());
        }

        /// <summary>
        /// Converts a string representation of a numeric value to a long value.
        /// Useful to convert revision numbers.
        /// </summary>
        /// <param name="text">a string to convert to a long value</param>
        /// <returns>
        /// a long representation of the given string;
        /// -1 is returned if the string can not be parsed
        /// </returns>
        public static long longValue(String text)
        {
            long longVal;
            bool couldParse= Int64.TryParse(text, out longVal);
            longVal = couldParse ? longVal : -1;
            return longVal;
        }

        /// <summary>
        /// Returns a short name for the given property name - that is
        /// a name without any prefixes.
        /// </summary>
        /// <param name="longName">a property name</param>
        /// <returns>a property short name</returns>
        public static String shortPropertyName(String longName)
        {
            if (longName == null)
            {
                return null;
            }
            if (longName.StartsWith(SVN_ENTRY_PREFIX))
            {
                return longName.Substring(SVN_ENTRY_PREFIX.Length);
            }
            else if (longName.StartsWith(SVN_WC_PREFIX))
            {
                return longName.Substring(SVN_WC_PREFIX.Length);
            }
            else if (longName.StartsWith(SVN_PREFIX))
            {
                return longName.Substring(SVN_PREFIX.Length);
            }
            return longName;
        }

        /// <summary>
        /// Returns the value for such boolean properties as
        /// "svn:executable", "svn:needs-lock"
        /// and "svn:special".
        /// Used by internals.
        /// </summary>
        /// <param name="propName">a property name</param>
        /// <returns>
        /// the property value "*", or
        /// <c>null</c> if the property is not boolean
        /// </returns>
        /// <seealso cref="isBooleanProperty(String)">
        /// </seealso>
        /// <since>           1.1
        /// </since>
        public static String getValueOfBooleanProperty(String propName)
        {
            if (EXECUTABLE.Equals(propName) || NEEDS_LOCK.Equals(propName) || SPECIAL.Equals(propName))
            {
                return "*";
            }
            return null;
        }

        /// <summary>
        /// Checks whether the property is boolean.
        /// </summary>
        /// <param name="propName">a property name</param>
        /// <returns>
        /// 	<c>true</c> if boolean,
        /// otherwise <c>false</c>
        /// </returns>
        /// <since>            1.1
        /// </since>
        public static bool isBooleanProperty(String propName)
        {
            return EXECUTABLE.Equals(propName) || SPECIAL.Equals(propName) || NEEDS_LOCK.Equals(propName);
        }
    }
}