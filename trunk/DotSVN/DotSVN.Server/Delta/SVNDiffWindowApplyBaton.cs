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
using System.IO;
using DotSVN.Common.Util;

namespace DotSVN.Server.Delta
{
    /// <summary>
    /// The <c>SVNDiffWindowApplyBaton</c> class is used to provide the source
    /// and target streams during applying diff windows. Also an instance of
    /// <c>SVNDiffWindowApplyBaton</c> may be supplied with an MD5 digest object
    /// for on-the-fly updating it with the bytes of the target view. So that when
    /// a diff window's instructions are applied, the digest will be the checksum
    /// for the full expanded text written to the target stream during delta application.
    /// </summary>
    public class SVNDiffWindowApplyBaton
    {
        internal Stream mySourceStream;
        internal Stream myTargetStream;

        internal long mySourceViewOffset;
        internal int mySourceViewLength;
        internal int myTargetViewSize;

        internal byte[] mySourceBuffer;
        internal byte[] myTargetBuffer;
        internal byte[] myDigest;

        /// <summary>
        /// Creates a diff window apply baton whith source and target streams
        /// represented by files.
        /// </summary>
        /// <param name="source">A source file (from where the source views would
        /// be taken)</param>
        /// <param name="target">A target file where the full text is written</param>
        /// <param name="digest">An MD5 checksum for the full text that would be
        /// updated after each instruction applying</param>
        /// <returns>
        /// A new <c>SVNDiffWindowApplyBaton</c> object
        /// </returns>
        public static SVNDiffWindowApplyBaton create(FileInfo source, FileInfo target,
                                                     byte[] digest)
        {
            SVNDiffWindowApplyBaton baton = new SVNDiffWindowApplyBaton();
            if (source.Exists)
            {
                baton.mySourceStream = SVNFileUtil.openFileForReading(source);
            }
            else
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Create baton failed");
                SVNErrorManager.error(err);
            }
            
            baton.myTargetStream = SVNFileUtil.openFileForWriting(target, true);
            baton.mySourceBuffer = new byte[0];
            baton.mySourceViewLength = 0;
            baton.mySourceViewOffset = 0;
            baton.myDigest = digest;
            return baton;
        }

        /// <summary>
        /// Creates a diff window apply baton whith initial source and target streams.
        /// </summary>
        /// <param name="source">A source input stream (from where the source
        /// views would be taken)</param>
        /// <param name="target">A target output stream where the full text is written</param>
        /// <param name="digest">an MD5 checksum for the full text that would be
        /// updated after each instruction applying</param>
        /// <returns>
        /// A new <c>SVNDiffWindowApplyBaton</c> object
        /// </returns>
        public static SVNDiffWindowApplyBaton create(Stream source, Stream target,
                                                     byte[] digest)
        {
            SVNDiffWindowApplyBaton baton = new SVNDiffWindowApplyBaton();
            baton.mySourceStream = source;
            baton.myTargetStream = target;
            baton.mySourceBuffer = new byte[0];
            baton.mySourceViewLength = 0;
            baton.mySourceViewOffset = 0;
            baton.myDigest = digest;
            return baton;
        }

        private SVNDiffWindowApplyBaton()
        {
        }

        /// <summary>
        /// Closes the source and target streams, finalizes
        /// the checksum computation and returns it in a hex representation.
        /// </summary>
        /// <returns>An MD5 checksum in a hex representation.</returns>
        public virtual String close()
        {
            SVNFileUtil.closeFile(mySourceStream);
            mySourceStream = null;
            SVNFileUtil.closeFile(myTargetStream);
            myTargetStream = null;
            if (myDigest != null)
            {
                return SVNUtil.ComputeHash(myDigest);
            }
            return null;
        }
    }
}