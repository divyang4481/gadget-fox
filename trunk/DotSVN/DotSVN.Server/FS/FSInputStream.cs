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
using System.IO;
using System.Text.RegularExpressions;
using DotSVN.Common;
using DotSVN.Common.Exceptions;
using DotSVN.Common.Util;
using DotSVN.Server.Delta;

namespace DotSVN.Server.FS
{
    public class FSInputStream : Stream
    {
        #region Private Fields
        
        private int chunkIndex;
        private bool isChecksumFinalized;
        private String hexChecksum;
        private long streamLength;
        private long streamOffset;
        private MemoryStream myBuffer;
        private SVNDeltaCombiner deltaCombiner;
        private List<FSRepresentationState> repStateList;
        private static readonly string deltaPattern = @"^(?'header'\w+)\s(?'baseRevision'\d+)\s(?'baseOffset'\d+)\s(?'baseLength'\d+)$";
        private static readonly Regex deltaRegex = new Regex(deltaPattern, RegexOptions.Compiled | RegexOptions.Singleline);
        private MemoryStream hashData = new MemoryStream(1024 * 100);

        #endregion

        #region Stream Properties

        public override Boolean CanRead
        {
            get{ return true;}
        }

        public override Boolean CanSeek
        {
            get { throw new NotImplementedException(); }
        }

        public override Boolean CanWrite
        {
            get { return false; }
        }

        public override Int64 Length
        {
            get { return streamLength; }
        }

        public override Int64 Position
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        #endregion

        private FSInputStream(SVNDeltaCombiner combiner, FSRepresentation representation, FSFS owner)
        {
            repStateList = new List<FSRepresentationState>();
            deltaCombiner = combiner;
            chunkIndex = 0;
            isChecksumFinalized = false;
            hexChecksum = representation.HexDigest;
            streamOffset = 0;
            streamLength = representation.ExpandedSize;
            try
            {
                buildRepresentationList(representation, repStateList, owner);
            }
            catch (SVNException)
            {
                /* Something terrible has happened while building rep list, need to	close any files still opened */
                Close();
                throw;
            }
        }

        public static Stream createDeltaStream(SVNDeltaCombiner combiner, FSRevisionNode fileNode, FSFS owner)
        {
            if (fileNode == null)
            {
                return null;
            }
            else if (fileNode.Type != SVNNodeKind.file)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_NOT_FILE,
                                           "Attempted to get textual contents of a *non*-file node");
                SVNErrorManager.error(err);
            }
            FSRepresentation representation = fileNode.TextRepresentation;
            if (representation == null)
            {
                return null;
            }
            return new FSInputStream(combiner, representation, owner);
        }

        public static Stream createDeltaStream(SVNDeltaCombiner combiner, FSRepresentation fileRep, FSFS owner)
        {
            if (fileRep == null)
            {
                return null;
            }
            return new FSInputStream(combiner, fileRep, owner);
        }

        private int readContents(byte[] buf, int offset, int length)
        {
            length = getContents(buf, offset, length);
            if (!isChecksumFinalized)
            {
                hashData.Write(buf, offset, length);
                streamOffset += length;

                if (streamOffset == streamLength)
                {
                    isChecksumFinalized = true;
                    byte[] hashDataBuffer = new byte[hashData.Position];
                    Array.Copy(hashData.GetBuffer(), hashDataBuffer, hashData.Position);
                    String hexDigest = SVNUtil.ComputeHash(hashDataBuffer);
                    hashData.Seek(0, SeekOrigin.Begin);
                    if (!hexChecksum.Equals(hexDigest))
                    {
                        SVNErrorMessage err =
                            SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT,"Checksum mismatch while reading representation");
                        SVNErrorManager.error(err);
                    }
                }
            }

            return length;
        }

        private int getContents(byte[] buffer, int offset, int length)
        {
            int remaining = length;
            int targetPos = offset;

            while (remaining > 0)
            {
                
                if (myBuffer != null && myBuffer.Position < myBuffer.Length)
                {
                    int bytesRemaining = (int)(myBuffer.Length - myBuffer.Position);
                    int copyLength = Math.Min(bytesRemaining, remaining);
                    /* Actually copy the data. */
                    myBuffer.Read(buffer, targetPos, copyLength);
                    targetPos += copyLength;
                    remaining -= copyLength;
                }
                else
                {
                    FSRepresentationState resultState =  repStateList[0];
                    if (resultState.currentOffset == resultState.endOffset)
                    {
                        break;
                    }
                    deltaCombiner.reset();

                    foreach (FSRepresentationState curState in repStateList)
                    {
                        while (curState.chunkIndex < chunkIndex)
                        {
                            deltaCombiner.skipWindow(curState.fsFile);
                            curState.chunkIndex++;
                            curState.currentOffset = curState.fsFile.Position;
                            if (curState.currentOffset >= curState.endOffset)
                            {
                                SVNErrorMessage err =
                                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT,
                                                           "Reading one svndiff window read beyond the end of the representation");
                                SVNErrorManager.error(err);
                            }
                        }
                        SVNDiffWindow window = deltaCombiner.readWindow(curState.fsFile, curState.version);
                        MemoryStream target = deltaCombiner.addWindow(window);
                        curState.chunkIndex++;
                        curState.currentOffset = curState.fsFile.Position;
                        if (target != null)
                        {
                            myBuffer = target;
                            chunkIndex++;
                            break;
                        }
                    }
                }
            }
            return targetPos;
        }

        private FSRepresentationState buildRepresentationList(FSRepresentation firstRep, List<FSRepresentationState> result, FSFS owner)
        {
            FSFile file = null;
            FSRepresentation rep = new FSRepresentation(firstRep);
            
            try
            {
                while (true)
                {
                    file = owner.openAndSeekRepresentation(rep);
                    FSRepresentationState repState = readRepresentationLine(file);
                    repState.fsFile = file;
                    repState.startOffset = file.Position;
                    repState.currentOffset = repState.startOffset;
                    repState.endOffset = repState.startOffset + rep.Size;
                    if (!repState.isDelta)
                    {
                        return repState;
                    }
                    byte[] header = new byte[4];
                    int r = file.Read(ref header);

                    if (!(header[0] == 'S' && header[1] == 'V' && header[2] == 'N' && r == 4))
                    {
                        SVNErrorMessage err =
                            SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed svndiff data in representation");
                        SVNErrorManager.error(err);
                    }
                    repState.version = header[3];
                    repState.chunkIndex = 0;
                    repState.currentOffset += 4;
                    /*
					* Push this rep onto the list. If it's self-compressed, we're
					* done.
					*/
                    result.Insert(result.Count, repState);
                    if (repState.isDeltaVsEmpty)
                    {
                        return null;
                    }
                    rep.Revision = repState.baseRevision;
                    rep.Offset = repState.baseOffset;
                    rep.Size = repState.baseLength;
                    rep.TransactionId = null;
                }
            }
            catch (IOException ioe)
            {
                if (file != null) 
                    file.Close();
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.IO_ERROR, ioe.Message);
                SVNErrorManager.error(err, ioe);
            }
            catch (SVNException)
            {
                if (file != null) 
                    file.Close();
                throw;
            }
            return null;
        }

        /// <summary>
        /// Reads the representation line from the revision file
        /// A representation begins with a line containing either "PLAIN\n" or
        /// "DELTA\n" or "DELTA <rev> <offset> <length>\n", where <rev>, <offset>,
        /// and <length> give the location of the delta base of the representation
        /// and the amount of data it contains (not counting the header or
        /// trailer).  If no base location is given for a delta, the base is the
        /// empty stream.  After the initial line comes raw svndiff data, followed
        /// by a cosmetic trailer "ENDREP\n".
        /// </summary>
        /// <param name="file">Revision file</param>
        /// <returns>The representation state</returns>
        private static FSRepresentationState readRepresentationLine(FSFile file)
        {
            FSRepresentationState repState = new FSRepresentationState();
            repState.isDelta = false;
            
            try
            {
                String line = file.ReadLine(160);
                if (FSRepresentation.REP_PLAIN.Equals(line))
                {
                    // This is a plain representation
                    return repState;
                }
                if (FSRepresentation.REP_DELTA.Equals(line))
                {
                    // This is a delta against the empty stream.
                    repState.isDelta = true;
                    repState.isDeltaVsEmpty = true;
                    return repState;
                }

                // We have hopefully a DELTA vs. a non-empty base revision
                //  Get the revision, offset and length of the base delta
                repState.isDelta = true;
                repState.isDeltaVsEmpty = false;
                
                bool fileIsCorrupt = true;
                long baseRevision = 0;
                long baseOffset = 0;
                long baseLength = 0;

                // Look for the pattern 'DELTA <rev> <offset> <length>\n'
                Match match = deltaRegex.Match(line);
                if(match.Success)
                {
                    string header = match.Groups["header"].Value;
                    if ( FSRepresentation.REP_DELTA.Equals(header)
                        && ( Int64.TryParse(match.Groups["baseRevision"].Value, out baseRevision) )
                        && ( Int64.TryParse(match.Groups["baseOffset"].Value, out baseOffset) )
                        && ( Int64.TryParse(match.Groups["baseLength"].Value, out baseLength) ) )
                    {
                        fileIsCorrupt = false;
                    }
                }

                //If the file is corrupt, report error; else update the representation state
                if (fileIsCorrupt)
                {
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Malformed representation header");
                    SVNErrorManager.error(err);
                }
                else
                {
                    repState.baseRevision = baseRevision;
                    repState.baseOffset = baseOffset;
                    repState.baseLength = baseLength;
                }
            }
            catch (SVNException)
            {
                file.Close();
                throw;
            }
            return repState;
        }

        #region Stream overrides

        public override int Read(byte[] buf, int offset, int length)
        {
            try
            {
                return readContents(buf, offset, length);
            }
            catch (SVNException svne)
            {
                throw new IOException(svne.Message);
            }
        }

        public override int ReadByte()
        {
            byte[] buf = new byte[1];
            int r = Read(buf, 0, 1);
            if (r <= 0)
            {
                return - 1;
            }
            return buf[0];
        }

        public override void Close()
        {
            foreach (FSRepresentationState state in repStateList)
            {
                if (state.fsFile != null)
                {
                    state.fsFile.Close();
                }
            }
            repStateList.Clear();
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override Int64 Seek(Int64 offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(Int64 value)
        {
            throw new NotImplementedException();
        }

        public override void Write(Byte[] buffer, Int32 offset, Int32 count)
        {
            throw new NotImplementedException();
        }

        #endregion

        private class FSRepresentationState
        {
            internal FSFile fsFile;

            /// <summary>
            /// The starting offset for the raw svndiff/plaintext data minus header.
            /// </summary>
            internal long startOffset;

            /// <summary>
            /// The current offset into the file
            /// </summary>
            internal long currentOffset;

            /// <summary>
            /// The end offset of the raw data.
            /// </summary>
            internal long endOffset;

            /// <summary>
            /// If a delta, what svndiff version?
            /// </summary>
            internal int version;

            internal int chunkIndex;
            internal bool isDelta;
            internal bool isDeltaVsEmpty;
            internal long baseRevision;
            internal long baseOffset;
            internal long baseLength;
        }
    }
}