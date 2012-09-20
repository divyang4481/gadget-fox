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
using System.Collections.Generic;
using System.IO;
using DotSVN.Common;
using DotSVN.Common.Entities;
using DotSVN.Common.Interfaces;
using DotSVN.Common.Util;

namespace DotSVN.Server.RepositoryAccess
{
    /// <summary>
    /// Interface for SVNRepository. For more details see <see cref="SVNRepository"/>
    /// </summary>
    /// <seealso cref="SVNRepository"/>
    public interface ISVNRepository
    {
        /// <summary>
        /// Gets and Sets the location of this repository.
        /// </summary>
        /// <value>The location.</value>
        SVNURL Location { get; set; }

        /// <summary>
        /// Sets or Gets an authentication driver for this object. The auth driver
        /// may be implemented to retrieve cached credentials, to prompt
        /// a user for credentials or something else (actually, this is up
        /// to an implementor).
        /// </summary>
        /// <value>The authentication manager.</value>
        ISVNAuthenticationManager AuthenticationManager { get; set; }

        /// <summary>
        /// Gets the Universal Unique IDentifier (UUID) of the repository this  driver is created for.
        /// </summary>
        /// <param name="ForceConnection">If true, forces this driver to open and test a connection</param>
        /// <returns>UUID of the repository</returns>
        string GetRepositoryUUID(bool ForceConnection);

        /// <summary>
        /// Gets a repository's root directory location.
        /// </summary>
        /// <param name="ForceConnection">If true, forces this driver to open and test a connection</param>
        /// <returns>
        /// The repository root directory location url
        /// </returns>
        SVNURL GetRepositoryRoot(bool ForceConnection);

        /// <summary>
        /// Tries to access a repository. Used to check if there're no problems
        /// with accessing a repository and to cache a repository UUID and root directory location.
        /// </summary>
        void TestConnection();

        /// <summary>
        /// Opens the current Repository
        /// </summary>
        void OpenRepository();

        /// <summary>
        /// Closes the current Repository
        /// </summary>
        void CloseRepository();

        /// <summary>
        /// Returns the number of the latest revision of the repository this driver is working with.
        /// </summary>
        /// <returns>The latest revision number</returns>
        long GetLatestRevision();


        /// <summary>
        /// Returns the recent repository revision number for the particular moment
        /// in time - the closest one before or at the specified datestamp.
        /// <para> Example: if you specify a single date without specifying a time of the day
        /// (e.g. 2002-11-27) the timestamp is assumed to 00:00:00 and the method won't
        /// return any revisions for the day you have specified but for the day just before it.</para>
        /// </summary>
        /// <param name="date">A datestamp for defining the needed moment in time</param>
        /// <returns>
        /// the revision of the repository for that time
        /// </returns>
        long GetDatedRevision(DateTime date);


        /// <summary>
        /// Returns unversioned revision properties for a particular revision.
        /// Property names (keys) are mapped to their values. You may use <c>SVNRevisionProperty</c>
        /// constants to retrieve property values from the map.
        /// </summary>
        /// <param name="revision">A revision number</param>
        /// <param name="properties">If not null then properties will be placed in this map, otherwise a new map will be created.</param>
        /// <returns>
        /// A map containing unversioned revision properties
        /// </returns>
        IDictionary<string, string> GetRevisionProperties(long revision, IDictionary<string, string> properties);


        /// <summary>
        /// Sets a revision property with the specified name to a new value.
        /// <para><b>NOTE:</b> revision properties are not versioned. So, the old values may be lost forever.</para>
        /// </summary>
        /// <param name="revision">The number of the revision which property is to be changed</param>
        /// <param name="propertyName">A revision property name</param>
        /// <param name="propertyValue">the value of the revision property</param>
        void SetRevisionPropertyValue(long revision, string propertyName, String propertyValue);

        /// <summary>
        /// Gets the value of an unversioned property.
        /// </summary>
        /// <param name="revision">a revision number</param>
        /// <param name="propertyName">a property name</param>
        /// <returns>
        /// a revision property value or null if there's no such revision property
        /// </returns>
        string GetRevisionPropertyValue(long revision, string propertyName);

        /// <summary>
        /// Returns the kind of an item located at the specified path in a particular revision.
        /// If the path does not exist under the specified revision, SVNNodeKind.none will be returned.
        /// <para>The path arg can be both relative to the location of this driver
        /// and absolute to the repository root (starts with "/").</para>
        /// </summary>
        /// <param name="path">An Item's path</param>
        /// <param name="revision">A revision number</param>
        /// <returns>
        /// The node kind for the given path at the given revision
        /// </returns>
        SVNNodeKind CheckPath(string path, long revision);

        /// <summary>
        /// Fetches the contents and or properties of a file located at the specified path
        /// in a particular revision. If contents arg is not null it will be written with file contents.
        /// If properties arg is not null it will receive the properties of the file.
        /// This includes all properties: not just ones controlled by a user and stored in the repository filesystem,
        /// but also non-tweakable ones (e.g. 'wcprops', 'entryprops', etc.). Property names (keys) are mapped to property values.
        /// <para>The path arg can be both relative to the location of this driver and absolute to the repository root
        /// (starts with "/").</para>
        /// <para>If revision is invalid (negative), HEAD revision will be used.</para>
        /// </summary>
        /// <param name="path">a file path</param>
        /// <param name="revision">File revision</param>
        /// <param name="properties">File properties receiver map</param>
        /// <param name="contents">An output stream to write the file contents to</param>
        /// <returns>The revision the file has been taken at</returns>
        long GetFile(string path, long revision, IDictionary<string, string> properties, Stream contents);

        /*

        /// <summary> Fetches the contents and/or properties of a directory located at the specified path
        /// in a particular revision.
        /// <para>If handler arg is not null  it will be dispatched with information of each directory entry represented by an 
        /// <c>SVNDirEntry</c> object.</para>
        /// 
        /// <para>If properties arg is not null  it will receive the properties of the file.  
        /// This includes all properties: not just ones controlled by a user and stored in the repository filesystem, but also non-tweakable 
        /// ones (e.g. 'wcprops', 'entryprops', etc.). Property names (keys) are mapped to property values.</para>
        /// 
        /// <para>The path arg can be both relative to the location of this driver and absolute to the repository root 
        /// (starts with "/").</para>
        /// 
        /// <para>If revision is invalid (negative), HEAD revision will be used.  
        /// <b>NOTE:</b> you may not invoke operation methods of this <c>SVNRepository</c> object from within the provided handler.</summary>
        /// <param name="path">A directory path</param>
        /// <param name="revision">A directory revision</param>
        /// <param name="properties">A directory properties receiver map</param>
        /// <param name="handler">A handler to process directory entries</param>
        /// <returns>The revision of the directory</returns>
        long GetDir(String path, long revision, System.Collections.IDictionary properties, ISVNDirEntryHandler handler);

        /// <summary>Retrieves interesting file revisions for the specified file. 
        /// 
        /// <para>A file revision is represented by an <c>SVNFileRevision</c> object. Each
        /// file revision is handled by the file revision handler provided. Only those 
        /// revisions will be retrieved in which the file was changed.
        /// The iteration will begin at the first such revision starting from the 
        /// startRevision and so on - up to the endRevision.
        /// If the method succeeds, the provided handler will have
        /// been invoked at least once.</para> 
        /// 
        /// <para>For the first interesting revision the file contents  will be provided to the handler 
        /// as a text delta against an empty file. For the following revisions, the delta will be against the fulltext contents of the 
        /// previous revision.</para> 
        /// 
        /// <para> The path arg can be both relative to the location of 
        /// this driver and absolute to the repository root (starts with "/").</para>
        /// 
        /// <para><b>NOTES:</b> 
        /// <ul>
        /// <li>you may not invoke methods of this <c>SVNRepository</c>
        /// object from within the provided handler</li>
        /// <li>this functionality is not available in pre-1.1 servers</li>
        /// </ul></para></summary>
        /// <param name="path">A file path </param>
        /// <param name="startRevision">The revision to start from</param>
        /// <param name="endRevision">The revision to stop at</param>
        /// <param name="handler">A handler that processes file revisions passed</param>
        /// <returns>The number of retrieved file revisions</returns>
        long GetFileRevisions(String path, long startRevision, long endRevision, ISVNFileRevisionHandler handler);

        /// <summary> Traverses revisions history. In other words, collects per revision
        /// information that includes the revision number, author, datestamp, 
        /// log message and maybe a list of changed paths (optional). For each
        /// revision this information is represented by an <c>SVNLogEntry</c> 
        /// object. Such objects are passed to the provided handler. 
        /// 
        /// <para>This method invokes handler on each log entry from
        /// startRevision toendRevision.
        /// startRevision may be greater or less than
        /// endRevision; this just controls whether the log messages are
        /// processed in descending or ascending revision number order.</para>
        /// 
        /// <para>If startRevision or endRevision is invalid, it
        /// defaults to the youngest.</para>
        /// 
        /// <para>If targetPaths has one or more elements, then
        /// only those revisions are processed in which at least one of targetPaths was
        /// changed (i.e., if a file text or properties changed; if dir properties
        /// changed or an entry was added or deleted). Each path is relative 
        /// to the repository location that this object is set to.</para>
        /// 
        /// <para>If changedPath is true , then each 
        /// <c>SVNLogEntry</c> passed to the handler will contain info about all 
        /// paths changed in that revision it represents. To get them call 
        /// getChangedPaths() that returns a map,
        /// which keys are the changed paths and the values are <c>SVNLogEntryPath</c> objects.
        /// If changedPath is false , changed paths info will not be provided.</para>
        /// 
        /// <para>If strictNode is true , copy history will 
        /// not be traversed (if any exists) when harvesting the revision logs for each path.</para>
        /// 
        /// <para>Target paths can be both relative to the location of 
        /// this driver and absolute to the repository root (starts with "/").</para>
        /// 
        /// <para><b>NOTE:</b> you may not invoke methods of this <c>SVNRepository</c>
        /// object from within the provided handler.</para></summary>
        /// 
        /// <param name="targetPaths">Paths that mean only those revisions at which they were changed</param>
        /// <param name="startRevision">A revision to start from</param>
        /// <param name="endRevision">A revision to end at</param>
        /// <param name="changedPath">If true then revision information will also include all changed paths per revision, otherwise not</param>
        /// <param name="strictNode">If true  then copy history (if any) is not to be traversed</param>
        /// <param name="handler">Handler that will be dispatched log entry objects</param>
        /// <returns>The number of revisions traversed</returns>
        long Log(String[] targetPaths, long startRevision, long endRevision, bool changedPath, bool strictNode, ISVNLogEntryHandler handler);


        /// <summary> Traverses revisions history. In other words, collects per revision
        /// information that includes the revision number, author, datestamp,log message and maybe a list of changed paths (optional). For each
        /// revision this information is represented by an <c>SVNLogEntry</c>object. Such objects are passed to the provided handler. 
        /// 
        /// <para>This method invokes handler on each log entry from startRevision to endRevision.
        /// startRevision may be greater or less than endRevision; this just controls whether the log messages are
        /// processed in descending or ascending revision number order.</para>
        /// 
        /// <para>If startRevision or endRevision is invalid, it defaults to the youngest.</para>
        ///  
        /// <para>If targetPaths has one or more elements, then only those revisions are processed in which at least 
        /// one of targetPaths was changed (i.e., if a file text or properties changed; if dir properties
        /// changed or an entry was added or deleted). Each path is relative to the repository location that this object is set to.</para>
        /// 
        /// <para>If changedPath is true , then each <c>SVNLogEntry</c> passed to the handler will contain info about all 
        /// paths changed in that revision it represents. To get them call getChangedPaths() that returns a map, which keys are the 
        /// changed paths and the values are <c>SVNLogEntryPath</c> objects. If changedPath is false , changed paths
        /// info will not be provided.</para> 
        /// 
        /// <para>If strictNode is true , copy history will not be traversed (if any exists) 
        /// when harvesting the revision logs for each path.</para>
        /// 
        /// <para>If limit is > 0 then only the first limit log entries
        /// will be handled. Otherwise (i.e. if limit is 0) this number is ignored.</para>
        /// 
        /// <para>Target paths can be both relative to the location of this driver and absolute to the repository root (starts with "/").</para>
        /// 
        /// <para><b>NOTE:</b> you may not invoke methods of this <c>SVNRepository</c> object from within the provided handler.</para>
        /// </summary>
        /// <param name="targetPaths">Paths that mean only those revisions at which they were changed</param>
        /// <param name="startRevision">Arevision to start from</param>
        /// <param name="endRevision">The revision to end at</param>
        /// <param name="changedPath">If true  then revision information will also include all changed paths per revision, otherwise not</param>
        /// <param name="strictNode">If true  then copy history (if any) is not to be traversed </param>
        /// <param name="limit">the maximum number of log entries to process</param>
        /// <param name="handler">Handler that will be dispatched log entry objects</param>
        /// <returns>The number of revisions traversed</returns>
        long Log(System.String[] targetPaths, long startRevision, long endRevision, bool changedPath, bool strictNode, long limit, ISVNLogEntryHandler handler);

        /// <summary> Gets entry locations in time. The location of an entry in a repository
        /// may change from revision to revision. This method allows to trace entry locations 
        /// in different revisions. 
        /// 
        /// <para>
        /// For each interesting revision (taken from revisions) an entry location
        /// is represented by an <c>SVNLocationEntry</c> object which is passed to the provided
        /// handler. Each <c>SVNLocationEntry</c> object represents a repository path 
        /// in a definite revision.
        /// 
        /// <para>
        /// The path arg can be both relative to the location of 
        /// this driver and absolute to the repository root (starts with "/").
        /// 
        /// <para>
        /// <b>NOTES:</b> 
        /// <ul>
        /// <li>you may not invoke methods of this <c>SVNRepository</c>
        /// object from within the provided handler
        /// <li>this functionality is not available in pre-1.1 servers
        /// </ul>
        /// 
        /// </summary>
        /// <param name="path			an">item's path 
        /// </param>
        /// <param name="pegRevision">	a revision in which path is first 
        /// looked up 
        /// </param>
        /// <param name="revisions">	an array of numbers of interesting revisions in which
        /// locations are looked up. If path 
        /// doesn't exist in an interesting revision, that revision 
        /// will be ignored. 
        /// </param>
        /// <param name="handler">		a location entry handler that will handle all found entry
        /// locations
        /// </param>
        /// <returns> 				the number of the entry locations found 
        /// </returns>
        /// <throws>  SVNException in the following cases: </throws>
        /// <summary>                      <ul>
        /// <li>path not found in the specified pegRevision
        /// <li>pegRevision is not valid
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed 
        /// </ul>
        /// </summary>
        /// <seealso cref="getLocations(String, Collection, long, long[])">
        /// </seealso>
        /// <seealso cref="getLocations(String, Map, long, long[])">
        /// </seealso>
        /// <seealso cref="ISVNLocationEntryHandler">
        /// </seealso>
        /// <seealso cref="SVNLocationEntry">
        /// </seealso>
        /// <since>               SVN 1.1
        /// </since>
        public abstract int getLocations(System.String path, long pegRevision, long[] revisions, ISVNLocationEntryHandler handler);

        /// <summary> Retrieves and returns interesting file revisions for the specified file. 
        /// 
        /// <para>
        /// A file revision is represented by an <c>SVNFileRevision</c> object. 
        /// Only those revisions will be retrieved in which the file was changed.
        /// The iteration will begin at the first such revision starting from the 
        /// startRevision and so on - up to the endRevision.
        /// 
        /// <para>
        /// The path arg can be both relative to the location of 
        /// this driver and absolute to the repository root (starts with "/").
        /// 
        /// <para>
        /// <b>NOTE:</b> this functionality is not available in pre-1.1 servers
        /// 
        /// </summary>
        /// <param name="path">        a file path 
        /// </param>
        /// <param name="revisions">	if not null  this collection
        /// will receive all the fetched file revisions   
        /// </param>
        /// <param name="sRevision">	a revision to start from
        /// </param>
        /// <param name="eRevision">	a revision to stop at
        /// </param>
        /// <returns> 				a collection that keeps	file revisions - {@link SVNFileRevision} instances 
        /// </returns>
        /// <throws>  SVNException if a failure occured while connecting to a repository  </throws>
        /// <summary>                      or the user's authentication failed (see 
        /// </summary>
        /// <seealso cref="getFileRevisions(String, long, long, ISVNFileRevisionHandler)">
        /// </seealso>
        /// <seealso cref="SVNFileRevision">
        /// </seealso>
        /// <since>               SVN 1.1
        /// </since>
        public virtual System.Collections.ICollection getFileRevisions(System.String path, System.Collections.ICollection revisions, long sRevision, long eRevision)
        {
            System.Collections.ICollection result = revisions != null ? revisions : new System.Collections.ArrayList();
            ISVNFileRevisionHandler handler = new AnonymousClassISVNFileRevisionHandler(result, this);
            getFileRevisions(path, sRevision, eRevision, handler);
            return result;
        }
        
        */

        /// <summary>
        /// Fetches the contents and properties of a directory located at the specified path
        /// in a particular revision. Information of each directory entry is represented by a single <c>SVNDirEntry</c> object.
        /// <para>The path arg can be both relative to the location of
        /// this driver and absolute to the repository root (starts with "/").</para>
        /// </summary>
        /// <param name="path">A directory path</param>
        /// <param name="revision">Arevision number</param>
        /// <param name="properties">If not null then all directory properties (including non-tweakable ones)
        /// will be put into this map (where keys are property names and mappings are property values)</param>
        /// <returns>
        /// A collection containing fetched directory entries <c>SVNDirEntry</c> objects
        /// </returns>
        ICollection<SVNDirEntry> GetDir(string path, long revision, IDictionary<string, string> properties);


        /// <summary>
        /// Fetches the contents of a directory into the provided collection object and returns the directory entry itself.
        /// <para>If entries arg is not null it receives the directory entries.
        /// Information of each directory entry is represented by an <c>SVNDirEntry</c> object.</para>
        /// The path arg can be both relative to the location of this driver and absolute to the repository root (starts with "/").
        /// </summary>
        /// <param name="path">A directory path</param>
        /// <param name="revision">A revision number</param>
        /// <param name="includeCommitMessages">if True then <c>SVNDirEntry</c> objects will be supplied with commit log messages, otherwise not</param>
        /// <param name="entries">A collection that receives fetched dir entries</param>
        /// <returns>
        /// The directory entry itself which contents are fetched into entries
        /// </returns>
        SVNDirEntry GetDir(string path, long revision, bool includeCommitMessages, ICollection entries);

        /*
        /// <summary> Traverses revisions history and returns a collection of log entries. In 
        /// other words, collects per revision information that includes the revision number, 
        /// author, datestamp, log message and maybe a list of changed paths (optional). For each
        /// revision this information is represented by an <c>SVNLogEntry</c>. 
        /// object. 
        /// 
        /// <para>
        /// startRevision may be greater or less than
        /// endRevision; this just controls whether the log messages are
        /// processed in descending or ascending revision number order.
        /// 
        /// <para>
        /// If startRevision</code> or <code>endRevision is invalid, it
        /// defaults to the youngest.
        /// 
        /// <para>
        /// If targetPaths has one or more elements, then
        /// only those revisions are processed in which at least one of targetPaths was
        /// changed (i.e., if a file text or properties changed; if dir properties
        /// changed or an entry was added or deleted). Each path is relative 
        /// to the repository location that this object is set to.
        /// 
        /// <para>
        /// If changedPath is true , then each 
        /// <c>SVNLogEntry</c> object is supplied with info about all 
        /// paths changed in that revision it represents. To get them call 
        /// getChangedPaths() that returns a map,
        /// which keys are the changed paths and the mappings are <c>SVNLogEntryPath</c> objects.
        /// If changedPath is false , changed paths
        /// info will not be provided. 
        /// 
        /// <para>
        /// If strictNode is true , copy history will 
        /// not be traversed (if any exists) when harvesting the revision logs for each path.
        /// 
        /// <para>
        /// Target paths can be both relative to the location of 
        /// this driver and absolute to the repository root (starts with "/").
        /// 
        /// </summary>
        /// <param name="targetPaths">     paths that mean only those revisions at which they were 
        /// changed
        /// </param>
        /// <param name="entries">			if not null  then this collection
        /// will receive log entries
        /// </param>
        /// <param name="startRevision">   a revision to start from
        /// </param>
        /// <param name="endRevision">     a revision to end at 
        /// </param>
        /// <param name="changedPath">     if true  then
        /// revision information will also include all changed paths per 
        /// revision, otherwise not
        /// </param>
        /// <param name="strictNode">      if true  then copy history (if any) is not 
        /// to be traversed
        /// </param>
        /// <returns> 					a collection with log entries
        /// </returns>
        /// <throws>  SVNException     if a failure occured while connecting to a repository  </throws>
        /// <summary>                          or the user's authentication failed 
        /// </summary>
        /// <seealso cref="log(String[], long, long, boolean, boolean, ISVNLogEntryHandler)">
        /// </seealso>
        /// <seealso cref="log(String[], long, long, boolean, boolean, long, ISVNLogEntryHandler)">
        /// </seealso>
        public virtual System.Collections.ICollection log(System.String[] targetPaths, System.Collections.ICollection entries, long startRevision, long endRevision, bool changedPath, bool strictNode)
        {
            System.Collections.ICollection result = entries != null ? entries : new System.Collections.ArrayList();
            log(targetPaths, startRevision, endRevision, changedPath, strictNode, new AnonymousClassISVNLogEntryHandler(result, this));
            return result;
        }

        /// <summary> Gets entry locations in time. The location of an entry in a repository
        /// may change from revision to revision. This method allows to trace entry locations 
        /// in different revisions. 
        /// 
        /// <para>
        /// For each interesting revision (taken from revisions) an entry location
        /// is represented by an <c>SVNLocationEntry</c> object. Each <c>SVNLocationEntry</c> 
        /// object represents a repository path in a definite revision.
        /// 
        /// <para>
        /// The path arg can be both relative to the location of 
        /// this driver and absolute to the repository root (starts with "/").
        /// 
        /// <para>
        /// <b>NOTE:</b> this functionality is not available in pre-1.1 servers
        /// 
        /// </summary>
        /// <param name="path">        an item's path
        /// </param>
        /// <param name="entries">		if not null  then this 
        /// collection object receives entry locations
        /// </param>
        /// <param name="pegRevision"> a revision in which path is first 
        /// looked up 
        /// </param>
        /// <param name="revisions">   an array of numbers of interesting revisions in which
        /// locations are looked up. If path 
        /// doesn't exist in an interesting revision, that revision 
        /// will be ignored. 
        /// </param>
        /// <returns> 				a collection with retrieved entry locations
        /// </returns>
        /// <throws>  SVNException in the following cases: </throws>
        /// <summary>                      <ul>
        /// <li>path</code> not found in the specified <code>pegRevision
        /// <li>pegRevision is not valid
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed 
        /// SVNAuthenticationException
        /// </ul>
        /// </summary>
        /// <seealso cref="getLocations(String, long, long[], ISVNLocationEntryHandler)">
        /// </seealso>
        /// <seealso cref="getLocations(String, Map, long, long[])">
        /// </seealso>
        /// <seealso cref="SVNLocationEntry">
        /// </seealso>
        /// <seealso cref="ISVNLocationEntryHandler">
        /// </seealso>
        /// <since>               SVN 1.1
        /// </since>
        public virtual System.Collections.ICollection getLocations(System.String path, System.Collections.ICollection entries, long pegRevision, long[] revisions)
        {
            System.Collections.ICollection result = entries != null ? entries : new System.Collections.ArrayList();
            getLocations(path, pegRevision, revisions, new AnonymousClassISVNLocationEntryHandler(result, this));
            return result;
        }

        /// <summary> Gets entry locations in time. The location of an entry in a repository
        /// may change from revision to revision. This method allows to trace entry locations 
        /// in different revisions. 
        /// 
        /// <para>
        /// For each interesting revision (taken from revisions) an entry location
        /// is represented by an <c>SVNLocationEntry</c> object. Each <c>SVNLocationEntry</c> 
        /// object represents a repository path in a definite revision.
        /// 
        /// <para>
        /// The path arg can be both relative to the location of 
        /// this driver and absolute to the repository root (starts with "/").
        /// 
        /// <para>
        /// <b>NOTE:</b> this functionality is not available in pre-1.1 servers
        /// 
        /// </summary>
        /// <param name="path">        an item's path
        /// </param>
        /// <param name="entries">     if not null  then this 
        /// map object receives entry locations (which keys are revision
        /// numbers as Longs and mappings are entry locations objects)
        /// </param>
        /// <param name="pegRevision"> a revision in which path is first 
        /// looked up 
        /// </param>
        /// <param name="revisions">   an array of numbers of interesting revisions in which
        /// locations are looked up. If path 
        /// doesn't exist in an interesting revision, that revision 
        /// will be ignored. 
        /// </param>
        /// <returns>              a map (which keys are revision numbers as Longs and mappings 
        /// are entry locations objects) with collected entry locations            
        /// </returns>
        /// <throws>  SVNException in the following cases: </throws>
        /// <summary>                      <ul>
        /// <li>path</code> not found in the specified <code>pegRevision
        /// <li>pegRevision is not valid
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed 
        /// SVNAuthenticationException
        /// </ul>
        /// </summary>
        /// <seealso cref="getLocations(String, long, long[], ISVNLocationEntryHandler)">
        /// </seealso>
        /// <seealso cref="getLocations(String, Collection, long, long[])">
        /// </seealso>
        /// <seealso cref="SVNLocationEntry">
        /// </seealso>
        /// <seealso cref="ISVNLocationEntryHandler">
        /// </seealso>
        /// <since>               SVN 1.1
        /// </since>
        public virtual System.Collections.IDictionary getLocations(System.String path, System.Collections.IDictionary entries, long pegRevision, long[] revisions)
        {
            System.Collections.IDictionary result = entries != null ? entries : new System.Collections.Hashtable();
            getLocations(path, pegRevision, revisions, new AnonymousClassISVNLocationEntryHandler1(result, this));
            return result;
        }

        /// <summary> Calculates the differences between two items. 
        /// 
        /// <para>
        /// target is the name (one-level path component) of an entry that will restrict
        /// the scope of the diff operation to this entry. In other words target is a child entry of the 
        /// directory represented by the repository location to which this object is set. For
        /// example, if we have something like "/dirA/dirB" in a repository, then
        /// this object's repository location may be set to "svn://host:port/path/to/repos/dirA",
        /// and target</code> may be <code>"dirB".
        /// 
        /// <para>
        /// If target</code> is null  or empty (<code>"")
        /// then the scope of the diff operation is the repository location to which
        /// this object is set.
        /// 
        /// <para>
        /// The reporter is used to describe the state of the target item(s) (i.e. 
        /// items' revision numbers). All the paths described by the reporter
        /// should be relative to the repository location to which this object is set. 
        /// 
        /// <para>
        /// After that the editor is used to carry out all the work on 
        /// evaluating differences against url</code>. This <code>editor contains 
        /// knowledge of where the change will begin (when {@link ISVNEditor#openRoot(long) ISVNEditor.openRoot()} 
        /// is called).
        /// 
        /// <para>
        /// If ignoreAncestry is false  then
        /// the ancestry of the paths being diffed is taken into consideration - they
        /// are treated as related. In this case, for example, if calculating differences between 
        /// two files with identical contents but different ancestry, 
        /// the entire contents of the target file is considered as having been removed and 
        /// added again. 
        /// 
        /// <para>
        /// If ignoreAncestry is true 
        /// then the two paths are merely compared ignoring the ancestry.
        /// 
        /// <para>
        /// <b>NOTE:</b> you may not invoke methods of this <c>SVNRepository</c>
        /// object from within the provided reporter</code> and <code>editor.
        /// 
        /// </summary>
        /// <param name="url">				a repository location of the entry against which 
        /// differences are calculated 
        /// </param>
        /// <param name="targetRevision">  a revision number of the entry located at the 
        /// specified url; defaults to the
        /// latest revision (HEAD) if this arg is invalid
        /// </param>
        /// <param name="revision">        a revision number of the repository location to which 
        /// this driver object is set
        /// </param>
        /// <param name="target">			a target entry name (optional)
        /// </param>
        /// <param name="ignoreAncestry">	if true  then
        /// the ancestry of the two entries to be diffed is 
        /// ignored, otherwise not 
        /// </param>
        /// <param name="recursive">       if true  and the diff scope
        /// is a directory, descends recursively, otherwise not 
        /// </param>
        /// <param name="getContents">     if false  contents (diff windows) will not be sent ot 
        /// the editor. 
        /// </param>
        /// <param name="reporter">		a caller's reporter
        /// </param>
        /// <param name="editor">			a caller's editor
        /// </param>
        /// <throws>  SVNException     in the following cases: </throws>
        /// <summary>                          <ul>
        /// <li>url not found neither in the specified 
        /// revision nor in the HEAD revision
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed 
        /// SVNAuthenticationException
        /// </ul>
        /// </summary>
        /// <seealso cref="ISVNReporterBaton">
        /// </seealso>
        /// <seealso cref="ISVNReporter">
        /// </seealso>
        /// <seealso cref="ISVNEditor">
        /// </seealso>
        public abstract void diff(SVNURL url, long targetRevision, long revision, System.String target, bool ignoreAncestry, bool recursive, bool getContents, ISVNReporterBaton reporter, ISVNEditor editor);

        /// <deprecated>
        /// </deprecated>
        public abstract void diff(SVNURL url, long targetRevision, long revision, System.String target, bool ignoreAncestry, bool recursive, ISVNReporterBaton reporter, ISVNEditor editor);


        /// <summary> Calculates the differences between two items. 
        /// 
        /// <para>
        /// target is the name (one-level path component) of an entry that will restrict
        /// the scope of the diff operation to this entry. In other words target is a child entry of the 
        /// directory represented by the repository location to which this object is set. For
        /// example, if we have something like "/dirA/dirB" in a repository, then
        /// this object's repository location may be set to "svn://host:port/path/to/repos/dirA",
        /// and target</code> may be <code>"dirB".
        /// 
        /// <para>
        /// If target</code> is null  or empty (<code>"")
        /// then the scope of the diff operation is the repository location to which
        /// this object is set.
        /// 
        /// <para>
        /// The reporter is used to describe the state of the target item(s) (i.e. 
        /// items' revision numbers). All the paths described by the reporter
        /// should be relative to the repository location to which this object is set. 
        /// 
        /// <para>
        /// After that the editor is used to carry out all the work on 
        /// evaluating differences against url</code>. This <code>editor contains 
        /// knowledge of where the change will begin (when {@link ISVNEditor#openRoot(long) ISVNEditor.openRoot()} 
        /// is called).
        /// 
        /// <para>
        /// If ignoreAncestry is false  then
        /// the ancestry of the paths being diffed is taken into consideration - they
        /// are treated as related. In this case, for example, if calculating differences between 
        /// two files with identical contents but different ancestry, 
        /// the entire contents of the target file is considered as having been removed and 
        /// added again. 
        /// 
        /// <para>
        /// If ignoreAncestry is true 
        /// then the two paths are merely compared ignoring the ancestry.
        /// 
        /// <para>
        /// <b>NOTE:</b> you may not invoke methods of this <c>SVNRepository</c>
        /// object from within the provided reporter</code> and <code>editor.
        /// 
        /// </summary>
        /// <param name="url">             a repository location of the entry against which 
        /// differences are calculated 
        /// </param>
        /// <param name="revision">        a revision number of the repository location to which 
        /// this driver object is set
        /// </param>
        /// <param name="target">          a target entry name (optional)
        /// </param>
        /// <param name="ignoreAncestry">  if true  then
        /// the ancestry of the two entries to be diffed is 
        /// ignored, otherwise not 
        /// </param>
        /// <param name="recursive">       if true  and the diff scope
        /// is a directory, descends recursively, otherwise not 
        /// </param>
        /// <param name="reporter">        a caller's reporter
        /// </param>
        /// <param name="editor">          a caller's editor
        /// </param>
        /// <throws>  SVNException     in the following cases: </throws>
        /// <summary>                          <ul>
        /// <li>url not found neither in the specified 
        /// revision nor in the HEAD revision
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed 
        /// SVNAuthenticationException
        /// </ul>
        /// </summary>
        /// <deprecated>              use {@link #diff(SVNURL, long, long, String, boolean, boolean, ISVNReporterBaton, ISVNEditor)} instead 
        /// </deprecated>
        /// <seealso cref="ISVNReporterBaton">
        /// </seealso>
        /// <seealso cref="ISVNReporter">
        /// </seealso>
        /// <seealso cref="ISVNEditor">
        /// </seealso>
        public abstract void diff(SVNURL url, long revision, System.String target, bool ignoreAncestry, bool recursive, ISVNReporterBaton reporter, ISVNEditor editor);

        /// <summary> Updates a path receiving changes from a repository.
        /// 
        /// <para>
        /// target is the name (one-level path component) of an entry that will 
        /// restrict the scope of the update to this entry. In other words target is a child entry of the 
        /// directory represented by the repository location to which this object is set. For
        /// example, if we have something like "/dirA/dirB" in a repository, then
        /// this object's repository location may be set to "svn://host:port/path/to/repos/dirA",
        /// and target</code> may be <code>"dirB".
        /// 
        /// <para>
        /// If target</code> is null  or empty (<code>"")
        /// then the scope of the update operation is the repository location to which
        /// this object is set.
        /// 
        /// <para>
        /// The reporter is used to describe the state of the local item(s) (i.e. 
        /// items' revision numbers, deleted, switched items). All the paths described by the 
        /// reporter should be relative to the repository location to which this 
        /// object is set. 
        /// 
        /// <para>
        /// After that the editor is used to carry out all the work on 
        /// updating. This editor contains 
        /// knowledge of where the change will begin (when {@link ISVNEditor#openRoot(long) ISVNEditor.openRoot()} 
        /// is called).
        /// 
        /// <para>
        /// <b>NOTE:</b> you may not invoke methods of this <c>SVNRepository</c>
        /// object from within the provided reporter</code> and <code>editor.
        /// 
        /// </summary>
        /// <param name="revision">		a desired revision to make update to; defaults to
        /// the latest revision (HEAD)
        /// </param>
        /// <param name="target">			an entry name (optional)  
        /// </param>
        /// <param name="recursive">       if true  and the update scope
        /// is a directory, descends recursively, otherwise not 
        /// </param>
        /// <param name="reporter">        a caller's reporter
        /// </param>
        /// <param name="editor">          a caller's editor
        /// </param>
        /// <throws>  SVNException     in the following cases: </throws>
        /// <summary>                          <ul>
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed SVNAuthenticationException
        /// </ul>
        /// </summary>
        /// <seealso cref="update(SVNURL, long, String, boolean, ISVNReporterBaton, ISVNEditor)">
        /// </seealso>
        /// <seealso cref="ISVNReporterBaton">
        /// </seealso>
        /// <seealso cref="ISVNReporter">
        /// </seealso>
        /// <seealso cref="ISVNEditor">
        /// </seealso>
        /// <seealso cref="<a href="http://svnkit.com/kb/dev-guide-update-operation.html">Using ISVNReporter/ISVNEditor in update-related operations</a>">
        /// </seealso>
        public abstract void update(long revision, System.String target, bool recursive, ISVNReporterBaton reporter, ISVNEditor editor);

        /// <summary> Gets status of a path.
        /// 
        /// <para>
        /// target is the name (one-level path component) of an entry that will 
        /// restrict the scope of the status to this entry. In other words target is a child entry of the 
        /// directory represented by the repository location to which this object is set. For
        /// example, if we have something like "/dirA/dirB" in a repository, then
        /// this object's repository location may be set to "svn://host:port/path/to/repos/dirA",
        /// and target</code> may be <code>"dirB".
        /// 
        /// <para>
        /// If target</code> is null  or empty (<code>"")
        /// then the scope of the update operation is the repository location to which
        /// this object is set.
        /// 
        /// <para>
        /// The reporter is used to describe the state of the local item(s) (i.e. 
        /// items' revision numbers, deleted, switched items). All the paths described by the 
        /// reporter should be relative to the repository location to which this 
        /// object is set. 
        /// 
        /// <para>
        /// After that the editor is used to carry out all the work on 
        /// performing status. This editor contains 
        /// knowledge of where the change will begin (when {@link ISVNEditor#openRoot(long) ISVNEditor.openRoot()} 
        /// is called).
        /// 
        /// <para>
        /// <b>NOTE:</b> you may not invoke methods of this <c>SVNRepository</c>
        /// object from within the provided reporter</code> and <code>editor.
        /// 
        /// </summary>
        /// <param name="revision">        a desired revision to get status against; defaults to
        /// the latest revision (HEAD)
        /// </param>
        /// <param name="target">          an entry name (optional)  
        /// </param>
        /// <param name="recursive">       if true  and the status scope
        /// is a directory, descends recursively, otherwise not 
        /// </param>
        /// <param name="reporter">		a client's reporter-baton
        /// </param>
        /// <param name="editor">			a client's status editor
        /// </param>
        /// <throws>  SVNException     in the following cases: </throws>
        /// <summary>                          <ul>
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed SVNAuthenticationException
        /// </ul>
        /// </summary>
        /// <seealso cref="ISVNReporterBaton">
        /// </seealso>
        /// <seealso cref="ISVNEditor">
        /// </seealso>
        public abstract void status(long revision, System.String target, bool recursive, ISVNReporterBaton reporter, ISVNEditor editor);

        /// <summary> Updates a path switching it to a new repository location.  
        /// 
        /// <para>
        /// Updates a path as it's described for the {@link #update(long, String, boolean, ISVNReporterBaton, ISVNEditor) update()}
        /// method using the provided reporter</code> and <code>editor, and switching
        /// it to a new repository location. 
        /// 
        /// <para>
        /// <b>NOTE:</b> you may not invoke methods of this <c>SVNRepository</c>
        /// object from within the provided reporter</code> and <code>editor.
        /// 
        /// </summary>
        /// <param name="url">				a new location in the repository to switch to
        /// </param>
        /// <param name="revision">        a desired revision to make update to; defaults
        /// to the latest revision (HEAD)
        /// </param>
        /// <param name="target">          an entry name (optional)  
        /// </param>
        /// <param name="recursive">       if true  and the switch scope
        /// is a directory, descends recursively, otherwise not 
        /// </param>
        /// <param name="reporter">        a caller's reporter
        /// </param>
        /// <param name="editor">          a caller's editor
        /// </param>
        /// <throws>  SVNException     in the following cases: </throws>
        /// <summary>                          <ul>
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed SVNAuthenticationException
        /// </ul>
        /// </summary>
        /// <seealso cref="update(long, String, boolean, ISVNReporterBaton, ISVNEditor)">
        /// </seealso>
        /// <seealso cref="ISVNReporterBaton">
        /// </seealso>
        /// <seealso cref="ISVNReporter">
        /// </seealso>
        /// <seealso cref="ISVNEditor">
        /// </seealso>
        /// <seealso cref="<a href="http://svnkit.com/kb/dev-guide-update-operation.html">Using ISVNReporter/ISVNEditor in update-related operations</a>">
        /// </seealso>
        public abstract void update(SVNURL url, long revision, System.String target, bool recursive, ISVNReporterBaton reporter, ISVNEditor editor);

        /// <summary> Checks out a directory from a repository .
        /// 
        /// <para>
        /// target is the name (one-level path component) of an entry that will 
        /// restrict the scope of the checkout to this entry. In other words target is a child entry of the 
        /// directory represented by the repository location to which this object is set. For
        /// example, if we have something like "/dirA/dirB" in a repository, then
        /// this object's repository location may be set to "svn://host:port/path/to/repos/dirA",
        /// and target</code> may be <code>"dirB".
        /// 
        /// <para>
        /// If target</code> is null  or empty (<code>"")
        /// then the scope of the checkout operation is the repository location to which
        /// this object is set.
        /// 
        /// <para>
        /// The provided editor is used to carry out all the work on 
        /// building a local tree of dirs and files being checked out. 
        /// 
        /// <para>
        /// <b>NOTE:</b> you may not invoke methods of this <c>SVNRepository</c>
        /// object from within the provided editor.
        /// 
        /// </summary>
        /// <param name="revision">    a desired revision of a dir to check out; defaults
        /// to the latest revision (HEAD)
        /// </param>
        /// <param name="target">      an entry name (optional)  
        /// </param>
        /// <param name="recursive">   if true  and the checkout 
        /// scope is a directory, descends recursively, otherwise not 
        /// </param>
        /// <param name="editor">		a caller's checkout editor
        /// </param>
        /// <throws>  SVNException	in the following cases: </throws>
        /// <summary>                      <ul>
        /// <li>the checkout scope is not a directory (only dirs can
        /// be checked out)
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed SVNAuthenticationException
        /// </ul>
        /// </summary>
        /// <seealso cref="update(long, String, boolean, ISVNReporterBaton, ISVNEditor)">
        /// </seealso>
        /// <seealso cref="ISVNEditor">
        /// 
        /// </seealso>
        public virtual void checkout(long revision, System.String target, bool recursive, ISVNEditor editor)
        {
            long lastRev = revision >= 0 ? revision : LatestRevision;
            // check path?
            SVNNodeKind nodeKind = checkPath("", revision);
            if (nodeKind == SVNNodeKind.FILE)
            {
                SVNErrorMessage err = SVNErrorMessage.Create(SVNErrorCode.RA_ILLEGAL_URL, "URL ''{0}'' refers to a file, not a directory", getLocation());
                SVNErrorManager.error(err);
            }
            else if (nodeKind == SVNNodeKind.NONE)
            {
                SVNErrorMessage err = SVNErrorMessage.Create(SVNErrorCode.RA_ILLEGAL_URL, "URL ''{0}'' doesn't exist", getLocation());
                SVNErrorManager.error(err);
            }
            update(revision, target, recursive, new AnonymousClassISVNReporterBaton(lastRev, this), editor);
        }

        /// <summary> Replays the changes from the specified revision through the given editor.
        /// 
        /// <para>
        /// Changes will be limited to those that occur under a session's URL, and
        /// the server will assume that the client has no knowledge of revisions
        /// prior to a lowRevision.  These two limiting factors define the portion
        /// of the tree that the server will assume the client already has knowledge of,
        /// and thus any copies of data from outside that part of the tree will be
        /// sent in their entirety, not as simple copies or deltas against a previous
        /// version.
        /// 
        /// <para>
        /// If sendDeltas is null , the actual text 
        /// and property changes in the revision will be sent, otherwise no text deltas and 
        /// null  property changes will be sent instead.
        /// 
        /// <para>
        /// If lowRevision is invalid, it defaults to 0.
        /// 
        /// </summary>
        /// <param name="lowRevision">    a low revision point beyond which a client has no
        /// knowledge of paths history        
        /// </param>
        /// <param name="revision">       a revision to replay
        /// </param>
        /// <param name="sendDeltas">     controls whether text and property changes are to be
        /// sent
        /// </param>
        /// <param name="editor">         a commit editor to receive changes 
        /// </param>
        /// <throws>  SVNException </throws>
        /// <since>  1.1, new in SVN 1.4
        /// </since>
        public abstract void replay(long lowRevision, long revision, bool sendDeltas, ISVNEditor editor);

        
        /// <summary> Gets an editor for committing changes to a repository. Having got the editor
        /// traverse a local tree of dirs and/or files to be committed, handling them    
        /// with corresponding methods of the editor. 
        /// 
        /// <para>
        /// mediator is used for temporary delta data storage allocations. 
        /// 
        /// <para>
        /// The root path of the commit is the current repository location to which
        /// this object is set.
        /// 
        /// <para>
        /// After the commit has succeeded {@link ISVNEditor#closeEdit() 
        /// ISVNEditor.closeEdit()} returns an <c>SVNCommitInfo</c> object
        /// that contains a new revision number, the commit date, commit author.
        /// 
        /// <para>
        /// This method should be rather used with pre-1.2 repositories.
        /// 
        /// <para>
        /// <b>NOTE:</b> you may not invoke methods of this <c>SVNRepository</c>
        /// object from within the returned commit editor.
        /// 
        /// </summary>
        /// <param name="logMessage">      a commit log message
        /// </param>
        /// <param name="mediator">        temp delta storage provider; used also to cache
        /// wcprops while committing  
        /// </param>
        /// <returns>                  an editor to commit a local tree of dirs and/or files 
        /// </returns>
        /// <throws>  SVNException     in the following cases: </throws>
        /// <summary>                          <ul>
        /// <li>the repository location this object is set to is not a 
        /// directory
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed SVNAuthenticationException
        /// </ul>
        /// </summary>
        /// <seealso cref="ISVNEditor">
        /// </seealso>
        /// <seealso cref="ISVNWorkspaceMediator">
        /// </seealso>
        /// <seealso cref="<a href="http://svnkit.com/kb/dev-guide-commit-operation.html">Using ISVNEditor in commit operations</a>">
        /// </seealso>
        public virtual ISVNEditor getCommitEditor(System.String logMessage, ISVNWorkspaceMediator mediator)
        {
            return getCommitEditor(logMessage, null, false, mediator);
        }

        /// <summary> Gives information about an entry located at the specified path in a particular 
        /// revision. 
        /// 
        /// <para>
        /// The path arg can be both relative to the location of 
        /// this driver and absolute to the repository root (starts with "/").
        /// 
        /// </summary>
        /// <param name="path			an">item's path
        /// </param>
        /// <param name="revision		a">revision of the entry; defaults to the latest 
        /// revision (HEAD)  
        /// </param>
        /// <returns>				an <c>SVNDirEntry</c> containing information about
        /// the entry or null  if 
        /// there's no entry with at the specified path 
        /// under the specified revision
        /// </returns>
        /// <throws>  SVNException in the following cases: </throws>
        /// <summary>                      <ul>
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed SVNAuthenticationException
        /// </ul>
        /// </summary>
        public abstract SVNDirEntry info(System.String path, long revision);

        /// <summary> Gets an editor for committing changes to a repository. Having got the editor
        /// traverse a local tree of dirs and/or files to be committed, handling them    
        /// with corresponding methods of the editor. 
        /// 
        /// <para>
        /// locks is a map used to provide lock tokens on the locked paths. 
        /// Keys are locked paths in a local tree, and each value for a key is a lock 
        /// token. locks must live during the whole commit operation.
        /// 
        /// <para>
        /// If keepLocks is true , then the locked 
        /// paths won't be unlocked after a successful commit. Otherwise, if 
        /// false , locks will be automatically released.
        /// 
        /// <para>
        /// mediator is used for temporary delta data storage allocations. 
        /// 
        /// <para>
        /// The root path of the commit is the current repository location to which
        /// this object is set.
        /// 
        /// <para>
        /// After the commit has succeeded {@link ISVNEditor#closeEdit() 
        /// ISVNEditor.closeEdit()} returns an <c>SVNCommitInfo</c> object
        /// that contains a new revision number, the commit date, commit author.
        /// 
        /// <para>
        /// <b>NOTE:</b> you may not invoke methods of this <c>SVNRepository</c>
        /// object from within the returned commit editor.
        /// 
        /// </summary>
        /// <param name="logMessage		a">commit log message
        /// </param>
        /// <param name="locks			a">map containing locked paths mapped to lock 
        /// tokens
        /// </param>
        /// <param name="keepLocks		<span">class="javakeyword">true  to keep 
        /// existing locks;	false  
        /// to release locks after the commit
        /// </param>
        /// <param name="mediator			temp">delta storage provider; used also to cache
        /// wcprops while committing  
        /// </param>
        /// <returns>					an editor to commit a local tree of dirs and/or files 
        /// </returns>
        /// <throws>  SVNException     in the following cases: </throws>
        /// <summary>                          <ul>
        /// <li>the repository location this object is set to is not a 
        /// directory
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed SVNAuthenticationException
        /// </ul>
        /// </summary>
        /// <seealso cref="getCommitEditor(String, ISVNWorkspaceMediator)">
        /// </seealso>
        /// <seealso cref="<a href="http://svnkit.com/kb/dev-guide-commit-operation.html">Using ISVNEditor in commit operations</a>">
        /// </seealso>
        public abstract ISVNEditor getCommitEditor(System.String logMessage, System.Collections.IDictionary locks, bool keepLocks, ISVNWorkspaceMediator mediator);

        /// <summary> Gets the lock for the file located at the specified path.
        /// If the file has no lock the method returns null .
        /// 
        /// <para>
        /// The path arg can be both relative to the location of 
        /// this driver and absolute to the repository root (starts with "/").
        /// 
        /// </summary>
        /// <param name="path		">    a file path  
        /// </param>
        /// <returns>			     an <c>SVNLock</c> instance (representing the lock) or 
        /// null  if there's no lock
        /// </returns>
        /// <throws>  SVNException  in the following cases: </throws>
        /// <summary>                       <ul>
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed SVNAuthenticationException
        /// </ul>
        /// </summary>
        /// <seealso cref="lock(Map, String, boolean, ISVNLockHandler)">
        /// </seealso>
        /// <seealso cref="unlock(Map, boolean, ISVNLockHandler)">
        /// </seealso>
        /// <seealso cref="getLocks(String)">
        /// </seealso>
        public abstract SVNLock getLock(System.String path);

        /// <summary> Gets all locks on or below the path, that is if the repository 
        /// entry (located at the path) is a directory then the method 
        /// returns locks of all locked files (if any) in it.
        /// 
        /// <para>
        /// The path arg can be both relative to the location of 
        /// this driver and absolute to the repository root (starts with "/").
        /// 
        /// </summary>
        /// <param name="path">		a path under which locks are to be retrieved
        /// </param>
        /// <returns> 				an array of <c>SVNLock</c> objects (representing locks)
        /// </returns>
        /// <throws>  SVNException in the following cases: </throws>
        /// <summary>                      <ul>
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed SVNAuthenticationException
        /// </ul>
        /// </summary>
        /// <seealso cref="lock(Map, String, boolean, ISVNLockHandler)">
        /// </seealso>
        /// <seealso cref="unlock(Map, boolean, ISVNLockHandler)">
        /// </seealso>
        /// <seealso cref="getLock(String)">
        /// </seealso>
        public abstract SVNLock[] getLocks(System.String path);

        /// <summary> Locks path(s) at definite revision(s).
        /// 
        /// <para>
        /// Note that locking is never anonymous, so any server implementing
        /// this function will have to "pull" a username from the client, if
        /// it hasn't done so already.
        /// 
        /// <para>
        /// Each path to be locked is handled with the provided handler.
        /// If a path was successfully locked, the handler's 
        /// {@link ISVNLockHandler#handleLock(String, SVNLock, SVNErrorMessage) handleLock()}
        /// is called that receives the path and either a lock object (representing the lock
        /// that was set on the path) or an error exception, if locking failed for that path.
        /// 
        /// <para>
        /// If any path is already locked by a different user and the
        /// force flag is false , then this call fails
        /// with throwing an <c>SVNException</c>. But if force is
        /// true , then the existing lock(s) will be "stolen" anyway,
        /// even if the user name does not match the current lock's owner.
        /// 
        /// <para>
        /// Paths can be both relative to the location of this driver and absolute to 
        /// the repository root (starting with "/").
        /// 
        /// </summary>
        /// <param name="pathsToRevisions		a">map which keys are paths and values are 
        /// revision numbers (as Longs); paths are strings and revision 
        /// numbers are Long objects 
        /// </param>
        /// <param name="comment				a">comment string for the lock (optional)
        /// </param>
        /// <param name="force				<span">class="javakeyword">true  if the file is to be 
        /// locked in any way (even if it's already locked by someone else)
        /// </param>
        /// <param name="handler">             if not null , the lock
        /// handler is invoked on each path to be locked  
        /// </param>
        /// <throws>  SVNException         in the following cases: </throws>
        /// <summary>                              <ul>
        /// <li>force is false 
        /// and a path is already locked by someone else
        /// <li>a revision of a path is less than its last changed revision
        /// <li>a path does not exist in the latest revision 
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed SVNAuthenticationException
        /// </ul>
        /// </summary>
        /// <seealso cref="unlock(Map, boolean, ISVNLockHandler)">
        /// </seealso>
        /// <seealso cref="getLocks(String)">
        /// </seealso>
        /// <seealso cref="getLock(String)">
        /// </seealso>
        public abstract void lock_Renamed(System.Collections.IDictionary pathsToRevisions, System.String comment, bool force, ISVNLockHandler handler);

        /// <summary> Removes lock(s) from the file(s). 
        /// 
        /// <para>
        /// Note that unlocking is never anonymous, so any server
        /// implementing this function will have to "pull" a username from
        /// the client, if it hasn't done so already.
        /// 
        /// <para>
        /// Each path to be unlocked is handled with the provided handler.
        /// If a path was successfully unlocked, the handler's 
        /// {@link ISVNLockHandler#handleUnlock(String, SVNLock, SVNErrorMessage) handleUnlock()}
        /// is called that receives the path and either a lock object (representing the lock
        /// that was removed from the path) or an error exception, if unlocking failed for 
        /// that path.
        /// 
        /// <para>
        /// If the username doesn't match the lock's owner and force is 
        /// false , this method call fails with
        /// throwing an <c>SVNException</c>.  But if the force
        /// flag is true , the lock will be "broken" 
        /// by the current user.
        /// 
        /// <para>
        /// Also if the lock token is incorrect or null 
        /// and force is false , the method 
        /// fails with throwing a <c>SVNException</c>. However, if force is 
        /// true  the lock will be removed anyway.
        /// 
        /// <para>
        /// Paths can be both relative to the location of this driver and absolute to 
        /// the repository root (starting with "/").
        /// 
        /// </summary>
        /// <param name="pathToTokens	a">map which keys are file paths and values are file lock
        /// tokens (both keys and values are strings)
        /// </param>
        /// <param name="force		<span">class="javakeyword">true  to remove the 
        /// lock in any case - i.e. to "break" the lock
        /// </param>
        /// <param name="handler">     if not null , the lock
        /// handler is invoked on each path to be unlocked
        /// </param>
        /// <throws>  SVNException in the following cases: </throws>
        /// <summary>                      <ul>
        /// <li>force is false 
        /// and the name of the user who tries to unlock a path does not match
        /// the lock owner
        /// <li>a lock token is incorrect for a path
        /// <li>a failure occured while connecting to a repository 
        /// <li>the user authentication failed SVNAuthenticationException
        /// </ul>
        /// </summary>
        /// <seealso cref="lock(Map, String, boolean, ISVNLockHandler)">
        /// </seealso>
        /// <seealso cref="getLocks(String)">
        /// </seealso>
        /// <seealso cref="getLock(String)">
        /// </seealso>
        public abstract void unlock(System.Collections.IDictionary pathToTokens, bool force, ISVNLockHandler handler);

        /// <summary> Closes the current session closing a socket connection used by
        /// this object.    
        /// If this driver object keeps a single connection for 
        /// all the data i/o, this method helps to reset the connection.
        /// 
        /// </summary>
        /// <throws>  SVNException  if some i/o error has occurred </throws>
        public abstract void closeSession();



        */
    }
}