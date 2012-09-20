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
using System.IO;
using System.Text;
using DotSVN.Common.Util;
using System.IO.Compression;

namespace DotSVN.Server.Delta
{
    /// <summary>
    /// The <b>SVNDiffWindow</b> class represents a diff window that
    /// contains instructions and new data of a delta to apply to a file.
    /// <para>Instructions are not immediately contained in a window. A diff window
    /// provides an iterator that reads and constructs one <see cref="SVNDiffInstruction"/>
    /// from provided raw bytes per one iteration. There is even an ability to
    /// use a single <b>SVNDiffInstruction</b> object for read and decoded instructions:
    /// for subsequent iterations an iterator simply uses the same instruction object
    /// to return as a newly read and decoded instruction.</para>
    /// </summary>
    /// <seealso cref="SVNDiffInstruction">
    /// </seealso>
    public class SVNDiffWindow
    {
        private long mySourceViewOffset;
        private int mySourceViewLength;
        private int myTargetViewLength;
        private int myNewDataLength;
        private int myInstructionsLength;

        private SVNDiffInstruction myTemplateInstruction = new SVNDiffInstruction(0, 0, 0);
        private SVNDiffInstruction myTemplateNextInstruction = new SVNDiffInstruction(0, 0, 0);

        private byte[] myData;
        private int myDataOffset;
        private int myInstructionsCount;

        /// <summary> Bytes of the delta header of an uncompressed diff window. </summary>
        public static readonly byte[] SVN_HEADER =
            new byte[] {(byte) 'S', (byte) 'V', (byte) 'N', (byte) '\x0000'};

        /// <summary> Bytes of the delta header of a compressed diff window.</summary>
        public static readonly byte[] SVN1_HEADER =
            new byte[] {(byte) 'S', (byte) 'V', (byte) 'N', (byte) '\x0001'};

        /// <summary> An empty window (in particular, its instructions length = 0). Corresponds 
        /// to the case of an empty delta, so, it's passed to a delta consumer to 
        /// create an empty file. 
        /// </summary>
        public static readonly SVNDiffWindow EMPTY = new SVNDiffWindow(0, 0, 0, 0, 0);

        /// <summary>
        /// Returns the length of instructions in bytes.
        /// </summary>
        /// <value>The length of the instructions.</value>
        /// <returns> a number of instructions bytes
        /// </returns>
        public virtual int InstructionsLength
        {
            get { return myInstructionsLength; }
        }

        /// <summary>
        /// Returns the source view offset.
        /// </summary>
        /// <value>The source view offset.</value>
        /// <returns> an offset in the source from where the source bytes
        /// must be copied
        /// </returns>
        public virtual long SourceViewOffset
        {
            get { return mySourceViewOffset; }
        }

        /// <summary>
        /// Returns the number of bytes to copy from the source view to the target one.
        /// </summary>
        /// <value>The length of the source view.</value>
        /// <returns> a number of source bytes to copy
        /// </returns>
        public virtual int SourceViewLength
        {
            get { return mySourceViewLength; }
        }

        /// <summary>
        /// Returns the length in bytes of the target view. The length of the target
        /// view is actually the number of bytes that should be totally copied by all the
        /// instructions of this window.
        /// </summary>
        /// <value>The length of the target view.</value>
        /// <returns> a length in bytes of the target view
        /// </returns>
        public virtual int TargetViewLength
        {
            get { return myTargetViewLength; }
        }

        /// <summary>
        /// Returns the number of new data bytes to copy to the target view.
        /// </summary>
        /// <value>The new length of the data.</value>
        /// <returns> a number of new data bytes
        /// </returns>
        public virtual int NewDataLength
        {
            get { return myNewDataLength; }
        }

        /// <summary>
        /// Sets a byte buffer containing instruction and new data bytes
        /// of this window.
        /// Instructions will go before new data within the buffer and should start
        /// at <code>buffer.position() + buffer.arrayOffset()</code>.
        /// Applying a diff window prior to setting instruction and new data bytes
        /// may cause a NPE.
        /// </summary>
        /// <value>The data.</value>
        public virtual MemoryStream Data
        {
            set
            {
                myData = value.GetBuffer();
                myDataOffset = (int)value.Position;
            }
        }

        /// <summary>
        /// Returns the total amount of new data and instruction bytes.
        /// </summary>
        /// <value>The length of the data.</value>
        /// <returns> new data length + instructions length
        /// </returns>
        public virtual int DataLength
        {
            get { return myNewDataLength + myInstructionsLength; }
        }

        /// <summary>
        /// Returns the amount of instructions of this window object.
        /// </summary>
        /// <value>The instructions count.</value>
        /// <returns> a total number of instructions
        /// </returns>
        public virtual int InstructionsCount
        {
            get { return myInstructionsCount; }
        }

        /// <summary>
        /// Constructs an <c>SVNDiffWindow</c> object. This constructor is
        /// used when bytes of instructions are not decoded and converted to
        /// <c>SVNDiffInstruction</c> objects yet, but are kept elsewhere
        /// along with new data.
        /// </summary>
        /// <param name="sourceViewOffset">an offset in the source view</param>
        /// <param name="sourceViewLength">a number of bytes to read from the
        /// source view</param>
        /// <param name="targetViewLength">a length in bytes of the target view
        /// it must have after copying bytes</param>
        /// <param name="instructionsLength">a number of instructions bytes</param>
        /// <param name="newDataLength">a number of bytes of new data</param>
        /// <seealso cref="SVNDiffInstruction">
        /// </seealso>
        public SVNDiffWindow(long sourceViewOffset, int sourceViewLength, int targetViewLength, int instructionsLength,
                             int newDataLength)
        {
            mySourceViewOffset = sourceViewOffset;
            mySourceViewLength = sourceViewLength;
            myTargetViewLength = targetViewLength;
            myInstructionsLength = instructionsLength;
            myNewDataLength = newDataLength;
        }

        /// <summary>
        /// Returns an iterator to read instructions in series.
        /// Objects returned by an iterator's <code>next()</code> method
        /// are separate <b>SVNDiffInstruction</b> objects.
        /// Instructions as well as new data are read from a byte
        /// buffer that is passed to this window object via the
        /// {@link #setData(ByteBuffer) setData()} method.
        /// A call to this routine is equivalent to a call
        /// <code>instructions(false)</code>.
        /// </summary>
        /// <returns>an instructions iterator</returns>
        /// <seealso cref="instructions(bool)">
        /// </seealso>
        /// <seealso cref="SVNDiffInstruction">
        /// </seealso>
        public virtual IEnumerator instructions()
        {
            return instructions(false);
        }

        /// <summary>
        /// Returns an iterator to read instructions in series.
        /// If <code>template</code> is <span class="javakeyword">true</span>
        /// then each instruction returned by the iterator is actually the
        /// same <b>SVNDiffInstruction</b> object, but with proper options.
        /// This prevents from allocating new memory.
        /// On the other hand, if <code>template</code> is <span class="javakeyword">false</span>
        /// then the iterator returns a new allocated <b>SVNDiffInstruction</b> object per
        /// each instruction read and decoded.
        /// Instructions as well as new data are read from a byte buffer that is
        /// passed to this window object via the
        /// {@link #setData(ByteBuffer) setData()} method.
        /// </summary>
        /// <param name="template">to use a single/multiple instruction objects</param>
        /// <returns>an instructions iterator</returns>
        /// <seealso cref="instructions()">
        /// </seealso>
        /// <seealso cref="SVNDiffInstruction">
        /// </seealso>
        public virtual IEnumerator instructions(bool template)
        {
            return new InstructionsIterator(this, template);
        }

        /// <summary> Applies this window's instructions. The source and target streams
        /// are provided by <code>applyBaton</code>. 
        /// 
        /// If this window has got any {@link SVNDiffInstruction#COPY_FROM_SOURCE} instructions, then: 
        /// <ol>
        /// <li>At first copies a source view from the source stream 
        /// of <code>applyBaton</code> to the baton's inner source buffer.  
        /// {@link SVNDiffInstruction#COPY_FROM_SOURCE} instructions of this window are 
        /// relative to the bounds of that source buffer (source view, in other words).</li>
        /// <li>Second, according to instructions, copies source bytes from the source buffer
        /// to the baton's target buffer (or target view, in other words).</li> 
        /// <li>Then, if <code>applyBaton</code> is supplied with an MD5 digest, updates it with those bytes
        /// in the target buffer. So, after instructions applying completes, it will be the checksum for
        /// the full text expanded.</li>
        /// <li>The last step - appends the target buffer bytes to the baton's 
        /// target stream.</li>        
        /// </ol> 
        /// 
        /// {@link SVNDiffInstruction#COPY_FROM_NEW_DATA} instructions rule to copy bytes from 
        /// the instructions & new data buffer provided to this window object via a call to the 
        /// {@link #setData(ByteBuffer) setData()} method.
        /// 
        /// {@link SVNDiffInstruction#COPY_FROM_TARGET} instructions are relative to the bounds of
        /// the target buffer. 
        /// 
        /// </summary>
        /// <param name="applyBaton">   a baton that provides the source and target 
        /// views as well as holds the source and targed 
        /// streams 
        /// </param>
        public virtual void apply(SVNDiffWindowApplyBaton applyBaton)
        {
            // here we have streams and buffer from the previous calls (or nulls).

            // 1. buffer for target.
            if (applyBaton.myTargetBuffer == null || applyBaton.myTargetViewSize < TargetViewLength)
            {
                applyBaton.myTargetBuffer = new byte[TargetViewLength];
            }
            applyBaton.myTargetViewSize = TargetViewLength;

            // 2. buffer for source.
            int length = 0;
            if (SourceViewOffset != applyBaton.mySourceViewOffset || SourceViewLength > applyBaton.mySourceViewLength)
            {
                byte[] oldSourceBuffer = applyBaton.mySourceBuffer;
                // create a new buffer
                applyBaton.mySourceBuffer = new byte[SourceViewLength];
                // copy from the old buffer.
                if (applyBaton.mySourceViewOffset + applyBaton.mySourceViewLength > SourceViewOffset)
                {
                    // copy overlapping part to the new buffer
                    int start = (int) (SourceViewOffset - applyBaton.mySourceViewOffset);
                    Array.Copy(oldSourceBuffer, start, applyBaton.mySourceBuffer, 0,
                               (applyBaton.mySourceViewLength - start));
                    length = (applyBaton.mySourceViewLength - start);
                }
            }
            if (length < SourceViewLength)
            {
                // fill what remains.
                try
                {
                    int toSkip =
                        (int) (SourceViewOffset - (applyBaton.mySourceViewOffset + applyBaton.mySourceViewLength));
                    if (toSkip > 0)
                    {
                        applyBaton.mySourceStream.Seek(toSkip, SeekOrigin.Current);
                    }
                    applyBaton.mySourceStream.Read(applyBaton.mySourceBuffer, length,
                                                   applyBaton.mySourceBuffer.Length - length);
                }
                catch (IOException e)
                {
                    SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.IO_ERROR, e.Message);
                    SVNErrorManager.error(err, e);
                }
            }
            // update offsets in baton.
            applyBaton.mySourceViewLength = SourceViewLength;
            applyBaton.mySourceViewOffset = SourceViewOffset;

            // apply instructions.
            int tpos = 0;
            int npos = myInstructionsLength;
            try
            {
                for (IEnumerator instructionIterator = instructions(true); instructionIterator.MoveNext();)
                {
                    SVNDiffInstruction instruction = (SVNDiffInstruction) instructionIterator.Current;
                    int iLength = instruction.length < TargetViewLength - tpos
                                      ? instruction.length
                                      : TargetViewLength - tpos;
                    switch (instruction.type)
                    {
                        case SVNDiffInstruction.COPY_FROM_NEW_DATA:
                            Array.Copy(myData, myDataOffset + npos, applyBaton.myTargetBuffer, tpos, iLength);
                            npos += iLength;
                            break;

                        case SVNDiffInstruction.COPY_FROM_TARGET:
                            int start = instruction.offset;
                            int end = instruction.offset + iLength;
                            int tIndex = tpos;
                            for (int j = start; j < end; j++)
                            {
                                applyBaton.myTargetBuffer[tIndex] = applyBaton.myTargetBuffer[j];
                                tIndex++;
                            }
                            break;

                        case SVNDiffInstruction.COPY_FROM_SOURCE:
                            Array.Copy(applyBaton.mySourceBuffer, instruction.offset, applyBaton.myTargetBuffer, tpos,
                                       iLength);
                            break;

                        default:
                            break;
                    }
                    tpos += instruction.length;
                    if (tpos >= TargetViewLength)
                    {
                        break;
                    }
                }
                // save buffer.
                if (applyBaton.myDigest != null)
                {
                    Array.Copy(applyBaton.myTargetBuffer, applyBaton.myDigest, TargetViewLength);
                }
                applyBaton.myTargetStream.Write(applyBaton.myTargetBuffer, 0, TargetViewLength);
            }
            catch (IOException e)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.IO_ERROR, e.Message);
                SVNErrorManager.error(err, e);
            }
        }

        /// <summary> Applies this window's instructions provided source and target view buffers. 
        /// If this window has got any {@link SVNDiffInstruction#COPY_FROM_SOURCE} instructions, then 
        /// appropriate bytes described by such an instruction are copied from the <code>sourceBuffer</code> 
        /// to the <code>targetBuffer</code>.
        /// 
        /// {@link SVNDiffInstruction#COPY_FROM_NEW_DATA} instructions rule to copy bytes from 
        /// the instructions & new data buffer provided to this window object via a call to the 
        /// {@link #setData(ByteBuffer) setData()} method.
        /// 
        /// {@link SVNDiffInstruction#COPY_FROM_TARGET} instructions are relative to the bounds of
        /// the <code>targetBuffer</code> itself. 
        /// </summary>
        /// <param name="sourceBuffer"> a buffer containing a source view
        /// </param>
        /// <param name="targetBuffer"> a buffer to get a target view
        /// </param>
        /// <returns>the size of the resultant target view
        /// </returns>
        public virtual int apply(byte[] sourceBuffer, byte[] targetBuffer)
        {
            int dataOffset = myInstructionsLength;
            int tpos = 0;
            for (IEnumerator instructionIterator = instructions(true); instructionIterator.MoveNext();)
            {
                SVNDiffInstruction instruction = (SVNDiffInstruction) instructionIterator.Current;
                int iLength = instruction.length < TargetViewLength - tpos
                                  ? instruction.length
                                  : TargetViewLength - tpos;
                switch (instruction.type)
                {
                    case SVNDiffInstruction.COPY_FROM_NEW_DATA:
                        Array.Copy(myData, myDataOffset + dataOffset, targetBuffer, tpos, iLength);
                        dataOffset += iLength;
                        break;

                    case SVNDiffInstruction.COPY_FROM_TARGET:
                        int start = instruction.offset;
                        int end = instruction.offset + iLength;
                        int tIndex = tpos;
                        for (int j = start; j < end; j++)
                        {
                            targetBuffer[tIndex] = targetBuffer[j];
                            tIndex++;
                        }
                        break;

                    case SVNDiffInstruction.COPY_FROM_SOURCE:
                        Array.Copy(sourceBuffer, instruction.offset, targetBuffer, tpos, iLength);
                        break;

                    default:
                        break;
                }
                tpos += instruction.length;
                if (tpos >= TargetViewLength)
                {
                    break;
                }
            }
            return TargetViewLength;
        }

        /// <summary>
        /// Tells if this window is not empty, i.e. has got any instructions.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if has instructions,<c>false</c> if has not
        /// </returns>
        public virtual bool hasInstructions()
        {
            return myInstructionsLength > 0;
        }

        /// <summary>
        /// Writes this window object to the provided stream.
        /// <para>
        /// If <c>writeHeader</c> is <c>true</c> then writes SVN_HEADER bytes also.</para>
        /// </summary>
        /// <param name="os">an output stream to write to</param>
        /// <param name="writeHeader">controls whether the header should be written or not</param>
        public virtual void writeTo(Stream os, bool writeHeader)
        {
            writeTo(os, writeHeader, false);
        }

        /// <summary>
        /// Formats and writes this window bytes to the specified output stream.
        /// </summary>
        /// <param name="os">An output stream to write the window to.</param>
        /// <param name="writeHeader">if <c>true</c> a window header will be also written</param>
        /// <param name="compress">if <c>true</c> writes compressed window bytes using SVN1_HEADER otherwise
        /// non-compressed window is written with SVN_HEADER
        public virtual void writeTo(Stream os, bool writeHeader, bool compress)
        {
            if (writeHeader)
            {
                byte[] header = compress ? SVN1_HEADER : SVN_HEADER;
                os.Write(header, 0, header.Length);
            }
            if (!hasInstructions())
            {
                return;
            }
            MemoryStream offsets = new MemoryStream(100);
            SVNDiffInstruction.writeLong(offsets, mySourceViewOffset);
            SVNDiffInstruction.writeInt(offsets, mySourceViewLength);
            SVNDiffInstruction.writeInt(offsets, myTargetViewLength);

            MemoryStream instructions = null;
            MemoryStream newData = null;
            if (compress)
            {
                instructions = inflate(myData, myDataOffset, myInstructionsLength);
                int instLength = (int)(instructions.Position - instructions.Length);
                newData = inflate(myData, myDataOffset + myInstructionsLength, myNewDataLength);
                int dataLength = (int)(newData.Position - newData.Length);
                SVNDiffInstruction.writeInt(offsets, instLength);
                SVNDiffInstruction.writeInt(offsets, dataLength);
            }
            else
            {
                SVNDiffInstruction.writeInt(offsets, myInstructionsLength);
                SVNDiffInstruction.writeInt(offsets, myNewDataLength);
            }
            os.Write(offsets.GetBuffer(), 0, (int)offsets.Position);
            if (compress)
            {
                os.Write(instructions.GetBuffer(), 0, (int)(instructions.Position - instructions.Length));
                os.Write(newData.GetBuffer(), 0, (int)(newData.Position - newData.Length));
            }
            else
            {
                os.Write(myData, myDataOffset, myInstructionsLength);
                if (myNewDataLength > 0)
                {
                    os.Write(myData, myDataOffset + myInstructionsLength, myNewDataLength);
                }
            }
        }

        /// <summary>
        /// Tells whether this window contains any copy-from-source
        /// instructions.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this window has got at least one {@link SVNDiffInstruction#COPY_FROM_SOURCE}
        /// instruction
        /// </returns>
        public virtual bool hasCopyFromSourceInstructions()
        {
            for (IEnumerator instrs = instructions(true); instrs.MoveNext();)
            {
                SVNDiffInstruction instruction = (SVNDiffInstruction) instrs.Current;
                if (instruction.type == SVNDiffInstruction.COPY_FROM_SOURCE)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates an exact copy of this window object.
        /// <code>targetData</code> is written instruction and new data bytes and
        /// then is set to a new window object via a call to its setData()
        /// method.
        /// </summary>
        /// <param name="targetData">a byte buffer to receive a copy of this wondow data</param>
        /// <returns>
        /// a new window object that is an exact copy of this one
        /// </returns>
        public virtual SVNDiffWindow clone(MemoryStream targetData)
        {
            int targetOffset = (int)targetData.Position ;
            long position = targetData.Position;
            targetData.Write(myData, myDataOffset, myInstructionsLength + myNewDataLength);
            targetData.Position = position;
            SVNDiffWindow clone =
                new SVNDiffWindow(SourceViewOffset, SourceViewLength, TargetViewLength, InstructionsLength,
                                  NewDataLength);
            clone.Data = targetData;
            clone.myDataOffset = targetOffset;
            return clone;
        }

        private static MemoryStream inflate(byte[] src, int offset, int length)
        {
            MemoryStream buffer = new MemoryStream(length*2 + 2);
            SVNDiffInstruction.writeInt(buffer, length);
            if (length < 512)
            {
                buffer.Write(src, offset, length);
            }
            else
            {
                DeflateStream outStream = new DeflateStream(buffer, CompressionMode.Compress);
                outStream.Write(src, offset, length);
                outStream.Close();
                if (buffer.Position >= length)
                {
                    buffer.Position = 0;
                    SVNDiffInstruction.writeInt(buffer, length);
                    buffer.Write(src, offset, length);
                }
            }
            buffer.Position = 0;
            return buffer;
        }

        /// <summary>
        /// Gives a string representation of this object.
        /// </summary>
        /// <returns>a string representation of this object</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(SourceViewOffset);
            sb.Append(":");
            sb.Append(SourceViewLength);
            sb.Append(":");
            sb.Append(TargetViewLength);
            sb.Append(":");
            sb.Append(InstructionsLength);
            sb.Append(":");
            sb.Append(NewDataLength);
            sb.Append(":");
            sb.Append(DataLength);
            sb.Append(":");
            sb.Append(myDataOffset);
            return sb.ToString();
        }

        /// <summary>
        /// Returns an array of instructions of this window.
        /// <para>
        /// If <code>target</code> is large enough to receive all instruction
        /// objects, then it's simply filled up to the end of instructions.
        /// However if it's not, it will be expanded to receive all instructions.</para>
        /// </summary>
        /// <param name="target">an instructions receiver</param>
        /// <returns>an array  containing all instructions</returns>
        public virtual SVNDiffInstruction[] loadDiffInstructions(SVNDiffInstruction[] target)
        {
            int index = 0;
            for (IEnumerator instructionIterator = instructions(); instructionIterator.MoveNext();)
            {
                if (index >= target.Length)
                {
                    SVNDiffInstruction[] newTarget = new SVNDiffInstruction[index*3/2];
                    Array.Copy(target, 0, newTarget, 0, index);
                    target = newTarget;
                }
                target[index] = (SVNDiffInstruction) instructionIterator.Current;
                index++;
            }
            myInstructionsCount = index;
            return target;
        }

        /// <summary>
        /// Fills a target buffer with the specified number of new data bytes
        /// of this window object taken at the specified offset.
        /// </summary>
        /// <param name="target">a buffer to copy to</param>
        /// <param name="offset">an offset relative to the position of the first
        /// new data byte of this window object</param>
        /// <param name="length">a number of new data bytes to copy</param>
        public virtual void writeNewData(MemoryStream target, int offset, int length)
        {
            offset += myDataOffset + myInstructionsLength;
            target.Write(myData, offset, length);
        }

        private class InstructionsIterator : IEnumerator
        {
            private SVNDiffWindow enclosingInstance;

            private SVNDiffInstruction nextInsruction;
            private int myOffset;
            private int newDataOffset;
            private bool isTemplate;

            public SVNDiffWindow Enclosing_Instance
            {
                get { return enclosingInstance; }
            }

            public virtual Object Current
            {
                get
                {
                    if (nextInsruction == null)
                    {
                        return null;
                    }

                    if (isTemplate)
                    {
                        Enclosing_Instance.myTemplateNextInstruction.type = nextInsruction.type;
                        Enclosing_Instance.myTemplateNextInstruction.length = nextInsruction.length;
                        Enclosing_Instance.myTemplateNextInstruction.offset = nextInsruction.offset;
                        nextInsruction = readNextInstruction();
                        return Enclosing_Instance.myTemplateNextInstruction;
                    }
                    Object next = nextInsruction;
                    nextInsruction = readNextInstruction();
                    return next;
                }
            }

            public virtual bool MoveNext()
            {
                return nextInsruction != null;
            }

            public virtual void Reset()
            {
            }

            public InstructionsIterator(SVNDiffWindow enclosingInstance, bool useTemplate)
            {
                InitBlock(enclosingInstance);
                isTemplate = useTemplate;
                nextInsruction = readNextInstruction();
            }

            private void InitBlock(SVNDiffWindow instance)
            {
                enclosingInstance = instance;
            }

            private SVNDiffInstruction readNextInstruction()
            {
                if (Enclosing_Instance.myData == null || myOffset >= Enclosing_Instance.myInstructionsLength)
                {
                    return null;
                }
                SVNDiffInstruction instruction = isTemplate
                                                     ? Enclosing_Instance.myTemplateInstruction
                                                     : new SVNDiffInstruction();
                instruction.type = (Enclosing_Instance.myData[Enclosing_Instance.myDataOffset + myOffset] & 0xC0) >> 6;
                instruction.length = Enclosing_Instance.myData[Enclosing_Instance.myDataOffset + myOffset] & 0x3f;
                myOffset++;
                if (instruction.length == 0)
                {
                    // read length from next byte                
                    instruction.length = readInt();
                }
                if (instruction.type == 0 || instruction.type == 1)
                {
                    // read offset from next byte (no offset without length).
                    instruction.offset = readInt();
                }
                else
                {
                    // set offset to offset in newdata.
                    instruction.offset = newDataOffset;
                    newDataOffset += instruction.length;
                }
                return instruction;
            }

            private int readInt()
            {
                int result = 0;
                while (true)
                {
                    byte b = Enclosing_Instance.myData[Enclosing_Instance.myDataOffset + myOffset];
                    result = result << 7;
                    result = result | (b & 0x7f);
                    if ((b & 0x80) != 0)
                    {
                        myOffset++;
                        if (myOffset >= Enclosing_Instance.myInstructionsLength)
                        {
                            return - 1;
                        }
                        continue;
                    }
                    myOffset++;
                    return result;
                }
            }
        }
    }
}