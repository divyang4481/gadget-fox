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
using System.Threading;
using DotSVN.Common;
using DotSVN.Common.Entities;
using DotSVN.Common.Interfaces;
using DotSVN.Common.Util;

namespace DotSVN.Server.RepositoryAccess
{
    /// <summary>
    /// The abstract class <c>SVNRepository</c> provides an interface for protocol
    /// specific drivers used for direct working with a Subversion repository.
    /// <c>SVNRepository</c> joins all low-level API methods needed for repository access operations.
    /// 
    /// <para>It is important to say that before using the library it must be configured
    /// according to implimentations to be used. That is if a repository is assumed
    /// to be accessed either via the <i>WebDAV</i> protocol (http:// or
    /// https://), or a custom <i>svn</i> one (svn:// or svn+ssh://&gt;)
    /// or immediately on the local machine (file:///) a user must initialize the library
    /// in a proper way. So, only after these setup steps the client can Create http | svn | file protocol
    /// implementations of the SVNRepository abstract class to access the repository.</para>
    /// 
    /// <para><c>SVNRepository</c> objects are not thread-safe, we're strongly recommend
    /// you not to use one <c>SVNRepository</c> object from within multiple threads.</para>
    /// Also methods of <c>SVNRepository</c> objects are not reenterable - that is,
    /// you can not call operation methods of an <c>SVNRepository</c> driver neither
    /// from within those handlers that are passed to some of the driver's methods, nor
    /// during committing with the help of a commit editor (until the editor's {@link ISVNEditor#closeEdit() closeEdit()}
    /// method is called).
    /// To authenticate a user over network <c>SVNRepository</c> drivers use <see cref="ISVNAuthenticationManager"/> auth drivers.
    /// </summary>
    public class SVNRepository : ISVNRepository
    {
        #region Private Fields

        private Object lockObject = new object();
        private Thread lockerThread = null;

        /// <summary>
        /// The current <see cref="ISVNSession"/>
        /// </summary>
        protected ISVNSession currentSession;

        /// <summary>
        /// The current location as a <see cref="SVNURL"/>
        /// </summary>
        private SVNURL location;

        /// <summary>
        /// The current Repository Root as a <see cref="SVNURL"/>
        /// </summary>
        protected SVNURL repositoryRoot;

        /// <summary>
        /// The UUID of this repository
        /// </summary>
        protected string repositoryUUID;

        /// <summary>
        /// The current <see cref="ISVNAuthenticationManager"/>
        /// </summary>
        protected ISVNAuthenticationManager SVNAuthenticationManager;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SVNRepository"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="session">The session.</param>
        public SVNRepository(SVNURL url, ISVNSession session)
        {
            location = url;
            currentSession = session;
        }

        #region ISVNRepository Members

        /// <summary>
        /// Get or Set the repository location to which this object is set.  It may be the location that was used to Create this object
        /// </summary>
        public virtual SVNURL Location
        {
            get { return location; }
            set { location = value; }
        }


        /// <summary>
        /// Gets or Sets an authentication driver for this object. The auth driver  may be implemented to retrieve cached credentials,
        /// to prompt a user for credentials or something else (actually, this is up to an implementor).
        /// </summary>
        public virtual ISVNAuthenticationManager AuthenticationManager
        {
            get { return SVNAuthenticationManager; }
            set { SVNAuthenticationManager = value; }
        }

        /// <summary>
        /// Gets the Universal Unique IDentifier (UUID) of the repository this driver is created for.
        /// </summary>
        /// <param name="forceConnection">if is true then forces
        /// this driver to test a connection</param>
        /// <returns>the UUID of a repository</returns>
        public virtual String GetRepositoryUUID(bool forceConnection)
        {
            if (forceConnection && repositoryUUID == null)
            {
                TestConnection();
            }
            return repositoryUUID;
        }



        /// <summary>
        /// Gets a repository's root directory location.
        /// </summary>
        /// <param name="ForceConnection">If true, forces this driver to open and test a connection</param>
        /// <returns>
        /// The repository root directory location url
        /// </returns>
        public virtual SVNURL GetRepositoryRoot(bool ForceConnection)
        {
            if (ForceConnection && repositoryRoot == null)
            {
                TestConnection();
            }
            return repositoryRoot;
        }

        /// <summary>
        /// Tries to access a repository. Used to check if there're no problems
        /// with accessing a repository and to cache a repository UUID and root directory location.
        /// </summary>
        public virtual void TestConnection()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens the current Repository
        /// </summary>
        public virtual void OpenRepository()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Closes the current Repository
        /// </summary>
        public virtual void CloseRepository()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the number of the latest revision of the repository this driver is working with.
        /// </summary>
        /// <returns>The latest revision number</returns>
        public virtual long GetLatestRevision()
        {
            throw new NotImplementedException();
        }

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
        public virtual long GetDatedRevision(DateTime date)
        {
            throw new NotImplementedException();
        }

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
        public virtual IDictionary<string, string> GetRevisionProperties(long revision,
                                                                         IDictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a revision property with the specified name to a new value.
        /// <para><b>NOTE:</b> revision properties are not versioned. So, the old values may be lost forever.</para>
        /// </summary>
        /// <param name="revision">The number of the revision which property is to be changed</param>
        /// <param name="propertyName">A revision property name</param>
        /// <param name="propertyValue">the value of the revision property</param>
        public virtual void SetRevisionPropertyValue(long revision, string propertyName, string propertyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value of an unversioned property.
        /// </summary>
        /// <param name="revision">a revision number</param>
        /// <param name="propertyName">a property name</param>
        /// <returns>
        /// a revision property value or null if there's no such revision property
        /// </returns>
        public virtual string GetRevisionPropertyValue(long revision, string propertyName)
        {
            throw new NotImplementedException();
        }

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
        public virtual SVNNodeKind CheckPath(string path, long revision)
        {
            throw new NotImplementedException();
        }

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
        public virtual long GetFile(string path, long revision, IDictionary<string, string> properties, 
                                        Stream contents)
        {
            throw new NotImplementedException();
        }

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
        public virtual ICollection<SVNDirEntry> GetDir(string path, long revision,
                                                       IDictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fetches the contents of a directory into the provided collection object and returns the directory entry itself.
        /// <para>If entries arg is not null it receives the directory entries.
        /// Information of each directory entry is represented by an <c>SVNDirEntry</c> object.</para>
        /// 	<para>The path arg can be both relative to the location of this driver and absolute to the repository root (starts with "/").
        /// </para>
        /// </summary>
        /// <param name="path">A directory path</param>
        /// <param name="revision">A revision number</param>
        /// <param name="includeCommitMessages">if True then <c>SVNDirEntry</c> objects will be supplied with commit log messages, otherwise not</param>
        /// <param name="entries">A collection that receives fetched dir entries</param>
        /// <returns>
        /// The directory entry itself which contents are fetched into entries
        /// </returns>
        public virtual SVNDirEntry GetDir(string path, long revision, bool includeCommitMessages, ICollection entries)
        {
            throw new NotImplementedException();
        }

        #endregion


        public static bool isValidRevision(long revision)
        {
            return revision >= 0;
        }

        public static bool isInvalidRevision(long revision)
        {
            return revision < 0;
        }

        protected internal void lockRepository()
        {
            try
            {
                Monitor.TryEnter(lockObject, 2000);
                if(lockerThread == Thread.CurrentThread)
                    throw new Exception("SVNRepository methods are not reenterable");
                lockerThread = Thread.CurrentThread;
            }
            catch
            {
                throw new Exception("Timeout occured while attemptinf to aquire write lock");
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
        }


        protected internal virtual void unlock()
        {
            try
            {
                Monitor.TryEnter(lockObject, 2000);
                lockerThread = null;
                Monitor.PulseAll(lockObject);
            }
            catch
            {
                throw new Exception("Timeout occured while attemptinf to release write lock");
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
        }


        /// <summary>
        /// Caches identification parameters (UUID, rood directory location)
        /// of the repository with which this driver is working.
        /// </summary>
        /// <param name="uuid">the repository's Universal Unique IDentifier
        /// (UUID)</param>
        /// <param name="rootURL">The root URL.</param>
        /// <seealso cref="GetRepositoryRoot">
        /// </seealso>
        /// <seealso cref="GetRepositoryUUID(bool)">
        /// </seealso>
        protected internal virtual void setRepositoryCredentials(String uuid, SVNURL rootURL)
        {
            if (uuid != null && rootURL != null)
            {
                repositoryUUID = uuid;
                repositoryRoot = rootURL;
            }
        }

    }
}