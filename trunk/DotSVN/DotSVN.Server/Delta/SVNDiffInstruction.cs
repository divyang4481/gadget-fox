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
using System.IO;

namespace DotSVN.Server.Delta
{
    /// <summary>
    /// The <c>SVNDiffInstruction</c> class represents instructions used as delta
    /// applying rules.
    /// For now there are three types of copy instructions:
    /// <ul>
    /// <li><see cref="SVNDiffInstruction.COPY_FROM_SOURCE"/> that is when bytes are copied from
    /// a source view (for example, existing revision of a file) to the target one.</li>
    /// <li><see cref="SVNDiffInstruction.COPY_FROM_NEW_DATA"/> new data bytes (e.g. new
    /// text) are copied to the target view.</li>
    /// <li><see cref="SVNDiffInstruction.COPY_FROM_TARGET"/> that is, when a sequence of bytes in the
    /// target must be repeated.</li>
    /// </ul>
    /// These are three different ways how full text representation bytes are
    /// obtained.
    /// </summary>
    public class SVNDiffInstruction
    {
        /// <summary> A type of an instruction that says that data must be copied
        /// from the source view to the target one.
        /// </summary>
        public const int COPY_FROM_SOURCE = 0x00;

        /// <summary> A type of an instruction that says that data must be copied
        /// from the target view to the target itself.  
        /// </summary>
        public const int COPY_FROM_TARGET = 0x01;

        /// <summary> A type of an instruction that says that data must be copied
        /// from the new data to the target view.  
        /// </summary>
        public const int COPY_FROM_NEW_DATA = 0x02;

        /// <summary>
        /// Creates a particular type of a diff instruction.
        /// Instruction offsets are relative to the bounds of views, i.e.
        /// a source/target view is a window of bytes (specified in a concrete
        /// diff window) in the source/target stream (this can be a file, a buffer).
        /// </summary>
        /// <param name="t">a type of an instruction</param>
        /// <param name="l">a number of bytes to copy</param>
        /// <param name="o">an offset in the source (which may be a source or a target
        /// view, or a new data stream) from where
        /// the bytes are to be copied</param>
        /// <seealso cref="SVNDiffWindow">
        /// </seealso>
        public SVNDiffInstruction(int t, int l, int o)
        {
            type = t;
            length = l;
            offset = o;
        }

        /// <summary>
        /// Creates a new instruction object.
        /// It's the instruction for the empty contents file.
        /// </summary>
        public SVNDiffInstruction() : this(0, 0, 0)
        {
        }

        /// <summary> A type of this instruction.</summary>
        public int type;

        /// <summary> A length bytes to copy.    </summary>
        public int length;

        /// <summary> An offset in the source from where the bytes
        /// should be copied. Instruction offsets are relative to the bounds of 
        /// views, i.e. a source/target view is a window of bytes (specified in a concrete 
        /// diff window) in the source/target stream (this can be a file, a buffer). 
        /// </summary>
        public int offset;

        /// <summary>
        /// Gives a string representation of this object.
        /// </summary>
        /// <returns>a string representation of this object</returns>
        public override String ToString()
        {
            StringBuilder b = new StringBuilder();
            switch (type)
            {
                case COPY_FROM_SOURCE:
                    b.Append("S->");
                    break;

                case COPY_FROM_TARGET:
                    b.Append("T->");
                    break;

                case COPY_FROM_NEW_DATA:
                    b.Append("D->");
                    break;
            }
            if (type == 0 || type == 1)
            {
                b.Append(offset);
            }
            else
            {
                b.Append(offset);
            }
            b.Append(":");
            b.Append(length);
            return b.ToString();
        }

        /// <summary>
        /// Wirtes this instruction to a byte buffer.
        /// </summary>
        /// <param name="target">a byte buffer to write to</param>
        public virtual void writeTo(MemoryStream target)
        {
            sbyte first = (sbyte) (type << 6);
            if (length <= 0x3f && length > 0)
            {
                // single-byte lenght;
                first |= (sbyte) ((length & 0x3f));
                target.WriteByte((byte) (first & 0xff));
            }
            else
            {
                target.WriteByte((byte) (first & 0xff));
                writeInt(target, length);
            }
            if (type == 0 || type == 1)
            {
                writeInt(target, offset);
            }
        }

        /// <summary>
        /// Writes an integer to a byte buffer.
        /// </summary>
        /// <param name="os">a byte buffer to write to</param>
        /// <param name="i">an integer to write</param>
        public static void writeInt(MemoryStream os, int i)
        {
            if (i == 0)
            {
                os.WriteByte(0);
                return;
            }
            int count = 1;
            long v = i >> 7;
            while (v > 0)
            {
                v = v >> 7;
                count++;
            }
            while (--count >= 0)
            {
                sbyte b;
                b = (sbyte) ((count > 0 ? 0x1 : 0x0) << 7);
                int r;
                r = ((sbyte) ((i >> (7*count)) & 0x7f)) | b;
                os.WriteByte((byte) r);
            }
        }

        /// <summary>
        /// Writes a long to a byte buffer.
        /// </summary>
        /// <param name="os">a byte buffer to write to</param>
        /// <param name="i">a long number to write</param>
        public static void writeLong(MemoryStream os, long i)
        {
            if (i == 0)
            {
                os.WriteByte(0);
                return;
            }
            // how many bytes there are:
            int count = 1;
            long v = i >> 7;
            while (v > 0)
            {
                v = v >> 7;
                count++;
            }
            while (--count >= 0)
            {
                sbyte b;
                b = (sbyte) ((count > 0 ? 0x1 : 0x0) << 7);
                int r;
                r = ((sbyte) ((i >> (7*count)) & 0x7f)) | b;
                os.WriteByte((byte) r);
            }
        }
    }
}