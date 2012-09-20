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
using System.IO.Compression;
using DotSVN.Common.Util;
using DotSVN.Server.FS;

namespace DotSVN.Server.Delta
{
    public class SVNDeltaCombiner
    {
        private SVNDiffWindow myWindow;

        private MemoryStream myWindowData;
        private MemoryStream myNextWindowInstructions;
        private MemoryStream myNextWindowData;
        private MemoryStream myTarget;
        private MemoryStream myRealTarget;
        private MemoryStream myReadWindowBuffer;

        private SVNRangeTree myRangeTree;
        private SVNOffsetsIndex myOffsetsIndex;
        private SVNDiffInstruction[] myWindowInstructions;
        private SVNDiffInstruction myInstructionTemplate;

        public SVNDeltaCombiner()
        {
            myRangeTree = new SVNRangeTree();
            myWindowInstructions = new SVNDiffInstruction[10];
            myInstructionTemplate = new SVNDiffInstruction(0, 0, 0);
            myOffsetsIndex = new SVNOffsetsIndex();

            myNextWindowData = new MemoryStream(1024 * 20);
        }

        public virtual void reset()
        {
            myWindow = null;
            myWindowData = null;
            myReadWindowBuffer = null;
            myNextWindowData = clearBuffer(myNextWindowData);
            myNextWindowInstructions = null;
            myTarget = null;
            myRealTarget = null;

            myRangeTree.dispose();
        }

        public virtual SVNDiffWindow readWindow(FSFile file, int version)
        {
            myReadWindowBuffer = clearBuffer(myReadWindowBuffer);
            myReadWindowBuffer = ensureBufferSize(myReadWindowBuffer, 4096);
            long position = 0;
            try
            {
                position = file.Position;
                file.Read(myReadWindowBuffer);
            }
            catch (IOException e)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.SVNDIFF_CORRUPT_WINDOW);
                SVNErrorManager.error(err, e);
            }
            myReadWindowBuffer.Position = 0;
            long sourceOffset = readLongOffset(myReadWindowBuffer);
            int sourceLength = readOffset(myReadWindowBuffer);
            int targetLength = readOffset(myReadWindowBuffer);
            int instructionsLength = readOffset(myReadWindowBuffer);
            int dataLength = readOffset(myReadWindowBuffer);
            if (sourceOffset < 0 || sourceLength < 0 || targetLength < 0 || instructionsLength < 0 || dataLength < 0)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.SVNDIFF_CORRUPT_WINDOW);
                SVNErrorManager.error(err);
            }
            position += myReadWindowBuffer.Position;
            file.Seek(position);

            myReadWindowBuffer = clearBuffer(myReadWindowBuffer);
            myReadWindowBuffer = ensureBufferSize(myReadWindowBuffer, instructionsLength + dataLength);
            myReadWindowBuffer.SetLength(instructionsLength + dataLength);
            try
            {
                file.Read(myReadWindowBuffer);
            }
            catch (IOException e)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.SVNDIFF_CORRUPT_WINDOW);
                SVNErrorManager.error(err, e);
            }
            myReadWindowBuffer.Position = 0;
            myReadWindowBuffer.SetLength(myReadWindowBuffer.Capacity);
            if (version == 1)
            {
                // decompress instructions and new data, put back to the buffer.
                try
                {
                    int[] lenghts = decompress(instructionsLength, dataLength);
                    instructionsLength = lenghts[0];
                    dataLength = lenghts[1];
                }
                catch (IOException e)
                {
                    SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.SVNDIFF_CORRUPT_WINDOW);
                    SVNErrorManager.error(err, e);
                }
            }
            SVNDiffWindow window =
                new SVNDiffWindow(sourceOffset, sourceLength, targetLength, instructionsLength, dataLength);
            window.Data = myReadWindowBuffer;
            return window;
        }

        public virtual void skipWindow(FSFile file)
        {
            myReadWindowBuffer = clearBuffer(myReadWindowBuffer);
            myReadWindowBuffer = ensureBufferSize(myReadWindowBuffer, 4096);
            long position = 0;
            try
            {
                position = file.Position;
                file.Read(myReadWindowBuffer);
            }
            catch (IOException e)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.SVNDIFF_CORRUPT_WINDOW);
                SVNErrorManager.error(err, e);
            }
            myReadWindowBuffer.Position = 0;
            if (readLongOffset(myReadWindowBuffer) < 0)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.SVNDIFF_CORRUPT_WINDOW);
                SVNErrorManager.error(err);
            }
            if (readOffset(myReadWindowBuffer) < 0)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.SVNDIFF_CORRUPT_WINDOW);
                SVNErrorManager.error(err);
            }
            if (readOffset(myReadWindowBuffer) < 0)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.SVNDIFF_CORRUPT_WINDOW);
                SVNErrorManager.error(err);
            }
            int instructionsLength = readOffset(myReadWindowBuffer);
            int dataLength = readOffset(myReadWindowBuffer);
            if (instructionsLength < 0 || dataLength < 0)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.SVNDIFF_CORRUPT_WINDOW);
                SVNErrorManager.error(err);
            }
            position += myReadWindowBuffer.Position;
            file.Seek(position + dataLength + instructionsLength);
            myReadWindowBuffer = clearBuffer(myReadWindowBuffer);
        }

        // when true, there is a target and my window.
        public virtual MemoryStream addWindow(SVNDiffWindow window)
        {
            // 1.
            // if window doesn't has cpfrom source apply it to empty file and save results to the target.
            // and we're done.
            if (window.SourceViewLength == 0 || !window.hasCopyFromSourceInstructions())
            {
                // apply window, make sure target not less then getTargetViewLength.
                myTarget = clearBuffer(myTarget);
                myTarget = ensureBufferSize(myTarget, window.TargetViewLength);
                int dataLength = window.apply(new byte[0], myTarget.GetBuffer());

                // and then apply myWindow if any.
                MemoryStream result;
                if (myWindow != null)
                {
                    myRealTarget = clearBuffer(myRealTarget);
                    myRealTarget = ensureBufferSize(myRealTarget, myWindow.TargetViewLength);
                    myWindow.apply(myTarget.GetBuffer(), myRealTarget.GetBuffer());
                    result = myRealTarget;
                }
                else
                {
                    result = myTarget;
                }
                result.Position = 0;
                int tLength = myWindow != null ? myWindow.TargetViewLength : window.TargetViewLength;
                result.SetLength(tLength);
                return result;
            }

            // 2.
            // otherwise combine window with myWindow, and save it in place of myWindow.
            // we're not done.
            if (myWindow != null)
            {
                myWindow = combineWindows(window);
                return null;
            }

            // 3. 
            // if we do not have myWindow yet, just save window as myWindow.
            // make sure window is 'copied', so that its full data goes to our buffer.
            // and also make sure that myWindowData has enough free space for that window.
            // we're not done.
            myWindowData = clearBuffer(myWindowData);
            myWindowData = ensureBufferSize(myWindowData, window.DataLength);
            myWindow = window.clone(myWindowData);
            return null;
        }

        private int[] decompress(int instructionsLength, int dataLength)
        {
            long originalPosition = myReadWindowBuffer.Position;
            int realInstructionsLength = readOffset(myReadWindowBuffer);
            byte[] instructionsData = new byte[realInstructionsLength];
            byte[] data = null;
            int realDataLength = 0;
            long  compressedLength = instructionsLength - (myReadWindowBuffer.Position - originalPosition);
            if (realInstructionsLength == compressedLength)
            {
                Array.Copy(myReadWindowBuffer.GetBuffer(), myReadWindowBuffer.Position,
                                         instructionsData, 0, realInstructionsLength);
                myReadWindowBuffer.Position = (myReadWindowBuffer.Position + realInstructionsLength);
            }
            else
            {
                byte[] compressedData = new byte[compressedLength - 2];
                Array.Copy(myReadWindowBuffer.GetBuffer(),myReadWindowBuffer.Position + 2,
                                         compressedData, 0, compressedLength - 2);
                myReadWindowBuffer.Position = (myReadWindowBuffer.Position + compressedLength);
                DeflateStream inputStream =
                    new DeflateStream(new MemoryStream(compressedData), CompressionMode.Decompress);
                int read = 0;
                while (read < realInstructionsLength)
                {
                    read += inputStream.Read(instructionsData, read, realInstructionsLength - read);
                }
            }
            if (dataLength > 0)
            {
                originalPosition = myReadWindowBuffer.Position;
                realDataLength = readOffset(myReadWindowBuffer);
                compressedLength = dataLength - (myReadWindowBuffer.Position - originalPosition);
                data = new byte[realDataLength];
                if (compressedLength == realDataLength)
                {
                    Array.Copy(myReadWindowBuffer.GetBuffer(), myReadWindowBuffer.Position, 
                                data, 0, realDataLength);
                    myReadWindowBuffer.Position = (myReadWindowBuffer.Position + realDataLength);
                }
                else
                {
//                    byte[] compressedData = new byte[compressedLength];
//                    Array.Copy(myReadWindowBuffer.GetBuffer(), myReadWindowBuffer.Position,
//                                             compressedData, 0, compressedLength);
//                    myReadWindowBuffer.Position = (myReadWindowBuffer.Position + compressedLength);
//                    DeflateStream deflateStream =
//                        new DeflateStream(new MemoryStream(compressedData), CompressionMode.Decompress);
//                    int read = 0;
//                    while (read < realDataLength)
//                    {
//                        read += deflateStream.Read(data, read, realDataLength - read);
//                    }
                    byte[] compressedData = new byte[compressedLength - 2];
                    Array.Copy(myReadWindowBuffer.GetBuffer(), myReadWindowBuffer.Position + 2,
                                             compressedData, 0, compressedLength - 2);
                    myReadWindowBuffer.Position = (myReadWindowBuffer.Position + compressedLength);
                    DeflateStream deflateStream =
                        new DeflateStream(new MemoryStream(compressedData), CompressionMode.Decompress);
                    int read = 0;
                    while (read < realDataLength)
                    {
                        read += deflateStream.Read(data, read, realDataLength - read);
                    }

                }
            }
            myReadWindowBuffer = clearBuffer(myReadWindowBuffer);
            myReadWindowBuffer = ensureBufferSize(myReadWindowBuffer, realInstructionsLength + realDataLength);
            myReadWindowBuffer.Write(instructionsData, 0, instructionsData.Length);
            if (data != null)
            {
                myReadWindowBuffer.Write(data, 0, data.Length);
            }
            myReadWindowBuffer.Position = 0;
            myReadWindowBuffer.SetLength(myReadWindowBuffer.Capacity);
            return new int[] {realInstructionsLength, realDataLength};
        }

        private SVNDiffWindow combineWindows(SVNDiffWindow window)
        {
            myNextWindowInstructions = clearBuffer(myNextWindowInstructions);
            myNextWindowData = clearBuffer(myNextWindowData);

            int targetOffset = 0;
            myWindowInstructions = window.loadDiffInstructions(myWindowInstructions);
            createOffsetsIndex(myWindowInstructions, window.InstructionsCount);

            SVNRangeTree rangeIndexTree = myRangeTree;
            rangeIndexTree.dispose();

            for (IEnumerator instructions = myWindow.instructions(true); instructions.MoveNext();)
            {
                SVNDiffInstruction instruction = (SVNDiffInstruction) instructions.Current;
                if (instruction.type != SVNDiffInstruction.COPY_FROM_SOURCE)
                {
                    myNextWindowInstructions = ensureBufferSize(myNextWindowInstructions, 10);
                    instruction.writeTo(myNextWindowInstructions);
                    if (instruction.type == SVNDiffInstruction.COPY_FROM_NEW_DATA)
                    {
                        myNextWindowData = ensureBufferSize(myNextWindowData, instruction.length);
                        myWindow.writeNewData(myNextWindowData, instruction.offset, instruction.length);
                    }
                }
                else
                {
                    int offset = instruction.offset;
                    int limit = instruction.offset + instruction.length;
                    int tgt_off = targetOffset;
                    rangeIndexTree.splay(offset);
                    SVNRangeTree.SVNRangeListNode listTail = rangeIndexTree.buildRangeList(offset, limit);
                    SVNRangeTree.SVNRangeListNode listHead = listTail.head;
                    for (SVNRangeTree.SVNRangeListNode range = listHead; range != null; range = range.next)
                    {
                        if (range.kind == SVNRangeTree.SVNRangeListNode.FROM_TARGET)
                        {
                            myInstructionTemplate.type = SVNDiffInstruction.COPY_FROM_TARGET;
                            myInstructionTemplate.length = range.limit - range.offset;
                            myInstructionTemplate.offset = range.targetOffset;
                            myNextWindowInstructions = ensureBufferSize(myNextWindowInstructions, 10);
                            myInstructionTemplate.writeTo(myNextWindowInstructions);
                        }
                        else
                        {
                            copySourceInstructions(range.offset, range.limit, tgt_off, window, myWindowInstructions);
                        }
                        tgt_off += range.limit - range.offset;
                    }
                    assertCondition(tgt_off == targetOffset + instruction.length, "assert #1");
                    rangeIndexTree.insert(offset, limit, targetOffset);
                    rangeIndexTree.disposeList(listHead);
                }
                targetOffset += instruction.length;
            }
            // build window from 'next' buffers and replace myWindow with the new one.
            myNextWindowData.Position = 0;
            myNextWindowInstructions.Position = 0;
            int instrLength = (int)myNextWindowInstructions.Length;
            int newDataLength = (int)myNextWindowData.Length;
            myWindowData = clearBuffer(myWindowData);
            myWindowData = ensureBufferSize(myWindowData, instrLength + newDataLength);
            myWindowData.Write(myNextWindowInstructions.GetBuffer(), 0, (int)myNextWindowInstructions.Length );
            myWindowData.Write(myNextWindowData.GetBuffer(), 0, (int)myNextWindowData.Length);
            myWindowData.Position = 0; // no need to set 'limit'...

            myWindow =
                new SVNDiffWindow(window.SourceViewOffset, window.SourceViewLength, myWindow.TargetViewLength,
                                  instrLength, newDataLength);
            myWindow.Data = myWindowData;

            myNextWindowInstructions = clearBuffer(myNextWindowInstructions);
            myNextWindowData = clearBuffer(myNextWindowData);
            return myWindow;
        }

        private void copySourceInstructions(int offset, int limit, int targetOffset, SVNDiffWindow window,
                                            SVNDiffInstruction[] windowInsructions)
        {
            int firstInstuctionIndex = findInstructionIndex(myOffsetsIndex, offset);
            int lastInstuctionIndex = findInstructionIndex(myOffsetsIndex, limit - 1);

            for (int i = firstInstuctionIndex; i <= lastInstuctionIndex; i++)
            {
                SVNDiffInstruction instruction = windowInsructions[i];
                int off0 = myOffsetsIndex.offsets[i];
                int off1 = myOffsetsIndex.offsets[i + 1];

                int fix_offset = offset > off0 ? offset - off0 : 0;
                int fix_limit = off1 > limit ? off1 - limit : 0;
                assertCondition(fix_offset + fix_limit < instruction.length, "assert #7");

                if (instruction.type != SVNDiffInstruction.COPY_FROM_TARGET)
                {
                    int oldOffset = instruction.offset;
                    int oldLength = instruction.length;

                    instruction.offset += fix_offset;
                    instruction.length = oldLength - fix_offset - fix_limit;

                    myNextWindowInstructions = ensureBufferSize(myNextWindowInstructions, 10);
                    instruction.writeTo(myNextWindowInstructions);
                    if (instruction.type == SVNDiffInstruction.COPY_FROM_NEW_DATA)
                    {
                        myNextWindowData = ensureBufferSize(myNextWindowData, instruction.length);
                        window.writeNewData(myNextWindowData, instruction.offset, instruction.length);
                    }
                    instruction.offset = oldOffset;
                    instruction.length = oldLength;
                }
                else
                {
                    assertCondition(instruction.offset < off0, "assert #8");
                    if (instruction.offset + instruction.length - fix_limit <= off0)
                    {
                        copySourceInstructions(instruction.offset + fix_offset,
                                               instruction.offset + instruction.length - fix_limit, targetOffset, window,
                                               windowInsructions);
                    }
                    else
                    {
                        int patternLength = off0 - instruction.offset;
                        int patternOverlap = fix_offset%patternLength;
                        assertCondition(patternLength > patternOverlap, "assert #9");
                        int fix_off = fix_offset;
                        int tgt_off = targetOffset;

                        if (patternOverlap >= 0)
                        {
                            int length =
                                Math.Min(instruction.length - fix_offset - fix_limit, patternLength - patternOverlap);
                            copySourceInstructions(instruction.offset + patternOverlap,
                                                   instruction.offset + patternOverlap + length, tgt_off, window,
                                                   windowInsructions);
                            tgt_off += length;
                            fix_off += length;
                        }
                        assertCondition(fix_off + fix_limit <= instruction.length, "assert #A");
                        if (patternOverlap > 0 && fix_off + fix_limit < instruction.length)
                        {
                            int length = Math.Min(instruction.length - fix_offset - fix_limit, patternOverlap);
                            copySourceInstructions(instruction.offset, instruction.offset + length, tgt_off, window,
                                                   windowInsructions);
                            tgt_off += length;
                            fix_off += length;
                        }
                        assertCondition(fix_off + fix_limit <= instruction.length, "assert #B");
                        if (fix_off + fix_limit < instruction.length)
                        {
                            myInstructionTemplate.type = SVNDiffInstruction.COPY_FROM_TARGET;
                            myInstructionTemplate.length = instruction.length - fix_off - fix_limit;
                            myInstructionTemplate.offset = tgt_off - patternLength;
                            myNextWindowInstructions = ensureBufferSize(myNextWindowInstructions, 10);
                            myInstructionTemplate.writeTo(myNextWindowInstructions);
                        }
                    }
                }
                targetOffset += instruction.length - fix_offset - fix_limit;
            }
        }

        private void createOffsetsIndex(SVNDiffInstruction[] instructions, int length)
        {
            if (myOffsetsIndex == null)
            {
                myOffsetsIndex = new SVNOffsetsIndex();
            }
            myOffsetsIndex.clear();
            int offset = 0;
            for (int i = 0; i < length; i++)
            {
                SVNDiffInstruction instruction = instructions[i];
                myOffsetsIndex.addOffset(offset);
                offset += instruction.length;
            }
            myOffsetsIndex.addOffset(offset);
        }

        private int findInstructionIndex(SVNOffsetsIndex offsets, int offset)
        {
            int lo = 0;
            int hi = offsets.length - 1;
            int op = (lo + hi)/2;

            assertCondition(offset < offsets.offsets[offsets.length - 1], "assert #2");

            for (; lo < hi; op = (lo + hi)/2)
            {
                int thisOffset = offsets.offsets[op];
                int nextOffset = offsets.offsets[op + 1];
                if (offset < thisOffset)
                {
                    hi = op;
                }
                else if (offset > nextOffset)
                {
                    lo = op;
                }
                else
                {
                    if (offset == nextOffset)
                    {
                        op++;
                    }
                    break;
                }
            }
            assertCondition(offsets.offsets[op] <= offset && offset < offsets.offsets[op + 1], "assert #3");
            return op;
        }

        private MemoryStream clearBuffer(MemoryStream memoryStream)
        {
            if (memoryStream != null)
            {
                memoryStream.Position = 0;
            }
            return memoryStream;
        }

        private MemoryStream ensureBufferSize(MemoryStream buffer, int dataLength)
        {
            if (buffer == null || buffer.Length < dataLength)
            {
                if (buffer != null)
                {
                    buffer.Capacity = (dataLength * 3 / 2);
                    buffer.Position = 0;
                }
                else
                {
                    buffer = new MemoryStream();
                    buffer.SetLength(dataLength * 3 / 2);
                }
            }
            return buffer;
        }

        private int readOffset(MemoryStream buffer)
        {
            long savedPosition = buffer.Position;
            int offset = 0;
            while (buffer.Position < buffer.Length)
            {
                sbyte byteRead = (sbyte)buffer.ReadByte();
                offset = (offset << 7) | (byteRead & 0x7F);
                if ((byteRead & 0x80) != 0)
                {
                    continue;
                }
                return offset;
            }
            buffer.Position = savedPosition;
            return - 1;
        }

        private long readLongOffset(MemoryStream buffer)
        {
            long savedPosition = buffer.Position;
            long offset = 0;
            while (buffer.Position < buffer.Length)
            {
                sbyte readByte;
                readByte = (sbyte)buffer.ReadByte();
                offset = (offset << 7) | (byte)(readByte & 0x7F);
                if ((readByte & 0x80) != 0)
                {
                    continue;
                }
                return offset;
            }
            buffer.Position = savedPosition;
            return - 1;
        }

        /// <summary>
        /// Asserts the condition.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="message">The message.</param>
        internal static void assertCondition(bool condition, String message)
        {
            if (!condition)
            {
                SVNErrorManager.LogError(message);
                SVNErrorManager.LogError(new Exception(message));
            }
        }

        private class SVNOffsetsIndex
        {
            public int length;
            public int[] offsets;

            public SVNOffsetsIndex()
            {
                offsets = new int[10];
            }

            public virtual void clear()
            {
                length = 0;
            }

            public virtual void addOffset(int offset)
            {
                if (length >= offsets.Length)
                {
                    int[] newOffsets = new int[length*3/2];
                    Array.Copy(offsets, 0, newOffsets, 0, length);
                    offsets = newOffsets;
                }
                offsets[length] = offset;
                length++;
            }
        }
    }
}