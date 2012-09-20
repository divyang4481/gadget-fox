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
using System.Text;
using System.Text.RegularExpressions;
using DotSVN.Common.Exceptions;
using DotSVN.Common.Util;

namespace DotSVN.Server.FS
{
    /// <summary>
    /// Encapsulates a File Repository File
    /// </summary>
    public class FSFile
    {
        #region Private Fields
        private const int MAX_HEADER_LINE_LENGTH = 1024;
        private const int MAX_LINE_LENGTH = 80;
        private const int MAX_PROP_LINE_LENGTH = 160;

        private long currentPosition = 0;
        private FileInfo fileInfo;
        private StreamReader streamReader;
        private static readonly string nameValuePattern = @"^(?'name'[^:]+):\s(?'value'.*)$";
        private static readonly Regex nameValueRegex = new Regex(nameValuePattern, RegexOptions.Compiled | RegexOptions.Singleline);
        #endregion

        #region Properties

        /// <summary>
        /// Gets the current file position.
        /// </summary>
        /// <returns>current file position</returns>
        public long Position
        {
            get { return currentPosition; }
        }

        /// <summary>
        /// Gets the size of the current file.
        /// </summary>
        /// <returns>The size of the current file in bytes</returns>
        public long Size
        {
            get { return fileInfo.Length; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FSFile"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        public FSFile(FileInfo file)
        {
            fileInfo = file;
            currentPosition = 0;
            streamReader = fileInfo.OpenText();
        }

        /// <summary>
        /// Seeks to the specified position from the begining of the file.
        /// </summary>
        /// <param name="position">The position to seek to.</param>
        public void Seek(long position)
        {
            streamReader.BaseStream.Seek(position, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();
            currentPosition = position;
        }

        /// <summary>
        /// Closes this file stream.
        /// </summary>
        public void Close()
        {
            streamReader.Close();
            streamReader = null;
            currentPosition = 0;
        }

        /// <summary>
        /// Reads the integer from current line.
        /// </summary>
        /// <returns>Integer in the current line, else -1</returns>
        public int ReadInt()
        {
            String line = ReadLine(MAX_LINE_LENGTH);

            int intValue = -1;
            // convert the string into a number
            try
            {
                intValue = Int32.Parse(line);
            }
            catch
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.MALFORMED_FILE);
                SVNErrorManager.error(err);
            }

            return intValue;
        }


        /// <summary>
        /// Reads the current line till the specified limit.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns>String in the current line</returns>
        public String ReadLine(int limit)
        {
            // Performance:
            // Max size is already known, it is moreefficent to use char array instead of StringBuilder
            char[] text = new char[limit];
            int charIndex = 0;
            try
            {
                int textIndex = 0;
                while (textIndex < limit)
                {
                    int character = streamReader.Read();
                    currentPosition++;
                    if (character < 0)
                    {
                        SVNErrorMessage err = SVNErrorMessage.create(
                            SVNErrorCode.STREAM_UNEXPECTED_EOF,
                            string.Format("Can't read length line from file {0}", fileInfo.FullName));
                        SVNErrorManager.error(err);
                    }
                    else if (character == '\n')
                    {
                        // Reached end of line; so break out of loop without appending \n or increasing textIndex
                        break;
                    }
                    text[charIndex++] = ((char)character);
                    textIndex++;
                }
            }
            catch (IOException exception)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Can''t read length line from file {0}: {1}",
                                           new Object[] {fileInfo.FullName, exception.Message});
                SVNErrorManager.error(err, exception);
            }
            return new string(text, 0, charIndex);
        }


        /// <summary>
        /// Reads till the specified limit (including '\n').
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns>String that is read</returns>
        public String ReadLimit(int limit)
        {
            int bytesToRead = limit - 1;
            char[] text = new char[bytesToRead];
            try
            {
                int bytesRead = streamReader.ReadBlock(text, 0, bytesToRead);
                currentPosition += bytesRead;
                if (bytesRead  < bytesToRead)
                {
                    SVNErrorMessage err = SVNErrorMessage.create(
                        SVNErrorCode.STREAM_UNEXPECTED_EOF,
                        string.Format("Can't read length line from file {0}", fileInfo.FullName));
                    SVNErrorManager.error(err);
                }

                int EOLChar = streamReader.Read();
                currentPosition++;
                if(EOLChar != '\n')
                {
                    SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.MALFORMED_FILE, "Missing EOL char while reading property.");
                    SVNErrorManager.error(err);
                }
            }
            catch (IOException exception)
            {
                SVNErrorMessage err =
                    SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Can''t read length line from file {0}: {1}",
                                           new Object[] { fileInfo.FullName, exception.Message });
                SVNErrorManager.error(err, exception);
            }
            return new string(text);
        }

        /// <summary>
        /// Reads the properties.
        /// The format of a dumped hash table is:
        /// K [nlength]
        /// name (a string of [nlength] bytes, followed by a newline)
        /// V [vlength]
        /// val (a string of [vlength] bytes, followed by a newline)
        /// [... etc, etc ...]
        /// END
        /// (Yes, there is a newline after END.)
        /// For example:
        /// K 5
        /// color
        /// V 3
        /// red
        /// K 11
        /// wine review
        /// V 376
        /// A forthright entrance, yet coquettish on the tongue, its deceptively
        /// fruity exterior hides the warm mahagony undercurrent that is the
        /// hallmark of Chateau Fraisant-Pitre.  Connoisseurs of the region will
        /// be pleased to note the familiar, subtle hints of mulberries and
        /// carburator fluid.  Its confident finish is marred only by a barely
        /// detectable suggestion of rancid squid ink.
        /// K 5
        /// price
        /// V 8
        /// US $6.50
        /// END
        /// </summary>
        /// <param name="allowEOF">if set to <c>true</c>, allow End of File.</param>
        /// <param name="digest">The digest is computed and returned in this string</param>
        /// <returns>
        /// The properties as an <c>IDictionary</c> of strings
        /// </returns>
        public IDictionary<string, string> ReadProperties(bool allowEOF, out string digest)
        {
            return ReadProperties(allowEOF, true, out digest);
        }

        /// <summary>
        /// Reads the properties.
        /// 
        /// The format of a dumped hash table is:
        /// 
        ///   K [nlength]
        ///   name (a string of [nlength] bytes, followed by a newline)
        ///   V [vlength]
        ///   val (a string of [vlength] bytes, followed by a newline)
        ///   [... etc, etc ...]
        ///   END
        /// 
        /// 
        /// (Yes, there is a newline after END.)
        /// 
        /// For example:
        /// 
        ///   K 5
        ///   color
        ///   V 3
        ///   red
        ///   K 11
        ///   wine review
        ///   V 376
        ///   A forthright entrance, yet coquettish on the tongue, its deceptively
        ///   fruity exterior hides the warm mahagony undercurrent that is the
        ///   hallmark of Chateau Fraisant-Pitre.  Connoisseurs of the region will
        ///   be pleased to note the familiar, subtle hints of mulberries and
        ///   carburator fluid.  Its confident finish is marred only by a barely
        ///   detectable suggestion of rancid squid ink.
        ///   K 5 
        ///   price
        ///   V 8
        ///   US $6.50
        ///   END
        /// </summary>
        /// <param name="allowEOF">if set to <c>true</c>, allow End of File.</param>
        /// <returns>The properties as an <c>IDictionary</c> of strings</returns>
        public IDictionary<string, string> ReadProperties(bool allowEOF)
        {
            string dummyString;
            return ReadProperties(allowEOF, false, out dummyString);
        }

        
        private IDictionary<string, string> ReadProperties(bool allowEOF, bool computeDigest, out string digest)
        {
            IDictionary<string, string> propCollection = new Dictionary<string, string>();
            digest = string.Empty;
            String line = null;
            StringBuilder hashData = new StringBuilder(MAX_PROP_LINE_LENGTH);
            try
            {
                while (true)
                {
                    // Read all key value pairs until we reach 'END' or end-of-file
                    // TODO: Replace the following parsing with Regex matching (System.Text.RegularExpressions)

                    // 1. Read the key; first read the length of the key and then read the key text
                    try
                    {
                        line = ReadLine(MAX_PROP_LINE_LENGTH); // K length or END, there may be EOF.
                        if (computeDigest)
                            hashData.Append(line + (char)'\n');
                    }
                    catch (SVNException svnEx)
                    {
                        if (allowEOF && (svnEx.ErrorMessage.ErrorCode == SVNErrorCode.STREAM_UNEXPECTED_EOF))
                        {
                            break;
                        }
                        SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.MALFORMED_FILE);
                        SVNErrorManager.error(err, svnEx);
                    }

                    // Break out of the loop if the line is empty or END (bcoz it means we finished reading)
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }
                    else if (!allowEOF && "END".Equals(line))
                    {
                        break;
                    }

                    // 1.1 Read the key length line (Should be in the form 'K <length>' or 'D' )
                    char keyChar = line[0];
                    if ((keyChar != 'K' && keyChar != 'D') || line.Length < 3 || line[1] != ' ')
                    {
                        SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.MALFORMED_FILE);
                        SVNErrorManager.error(err);
                    }

                    // 1.2 Extract the key length from the key length line
                    int keyLength;
                    bool parseSuccess = Int32.TryParse(line.Substring(2), out keyLength);
                    if (!parseSuccess || (keyLength < 0))
                    {
                        SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.MALFORMED_FILE);
                        SVNErrorManager.error(err);
                    }

                    // 1.3 Read the key text based on the key length obtained above + the EOL
                    string keyText = ReadLimit(keyLength + 1);
                    if (computeDigest)
                        hashData.Append(keyText + (char)'\n');

                    if (keyChar == 'D')
                    {
                        propCollection[keyText] = null;
                        continue;
                    }

                    // 2. Now let's read the value; first read the length of value, then read the value text

                    // 2.1 Read the value length line (Should be non-empty and in proper format 'V <length>' )
                    line = ReadLine(MAX_PROP_LINE_LENGTH);
                    if (computeDigest)
                        hashData.Append(line + (char)'\n');

                    char valueChar = line[0];
                    if (string.IsNullOrEmpty(line) || (valueChar != 'V') || (line.Length < 3) || (line[1] != ' '))
                    {
                        SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.MALFORMED_FILE);
                        SVNErrorManager.error(err);
                    }

                    // 2.2 Extract the value length from the line
                    int valueLength;
                    parseSuccess = Int32.TryParse(line.Substring(2), out valueLength);
                    if (!parseSuccess || (valueLength < 0))
                    {
                        SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.MALFORMED_FILE);
                        SVNErrorManager.error(err);
                    }

                    // 2.3 Now read the value text based on the value length obtained above + EOL
                    string valueText = ReadLimit(valueLength + 1);
                    if (computeDigest)
                        hashData.Append(valueText + (char)'\n');

                    // Put the key value pair into the properties collection
                    propCollection[keyText] = valueText;
                }

                // Compute the MD5 hash of the read data
                if (computeDigest)
                {
                    digest = SVNUtil.ComputeHash(hashData.ToString());
                }
            }
            catch (IOException e)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.MALFORMED_FILE);
                SVNErrorManager.error(err, e);
            }
            return propCollection;
        }

        /// <summary>
        /// Reads the header text and returns the header properties as name-value pairs.
        /// Each line in the header area is of the form '[name]: [value]'
        /// The header area ends where there is an empty line
        /// </summary>
        /// <returns>A collection of name-value pairs each of which represent a header id and its value</returns>
        public IDictionary<string, string> ReadHeader()
        {
            IDictionary<string, string> headerProps = new Dictionary<string, string>();
            while (true)
            {
                // Get the header lines one by one. 
                string headerLine = ReadLine(MAX_HEADER_LINE_LENGTH);

                // When we find an empty line, break out of the loop (this is the end of header area)
                if (string.IsNullOrEmpty(headerLine))
                {
                    break;
                }

                // Locate the colon character in the line; Each header line is of the form '[name]: [value]'
                // Match the current line
                Match match = nameValueRegex.Match(headerLine);
                if (match.Success)
                {
                    string nameText = match.Groups["name"].Value;
                    string valueText = match.Groups["value"].Value;

                    // Add the name value pair to header proeprties collection
                    headerProps[nameText] = valueText;
                }
                else
                {
                    // The regex match failed which means that header line is malformed; report error
                    SVNErrorMessage err =
                        SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, "Found malformed header in revision file");
                    SVNErrorManager.error(err);
                }
            }
            return headerProps;
        }

        /// <summary>
        /// Reads the root offsets and changed path offsets from the revision file
        ///  SVN Structure: At the very end of a rev file is a pair of lines containing
        ///     "\n[root-offset] [cp-offset]\n", 
        ///     where [root-offset] is the offset of the root directory node revision
        ///     and [cp-offset] is the offset of the changed-path data.
        /// </summary>
        /// <returns>A string that contains both the offsets separated by a white space</returns>
        public string ReadOffsets()
        {
            string offsetLine = String.Empty;

            // Seek till the 64th character from the end of the file
            Seek(Size - 64);
            string finalLine = streamReader.ReadToEnd();
            currentPosition = streamReader.BaseStream.Position;
            bool foundEOL = false;

            int charIndex = finalLine.Length - 1;
            // The last character in this line should be an EOL. If not, we should raise error
            if ( !String.IsNullOrEmpty(finalLine) &&  finalLine.EndsWith("\n"))
            {
                // The last character is a an EOL character (\n); hence skip it
                //  We need to find the '\n' before copy-offset & root-offset
                charIndex--;

                // Loop backwards through the string until we find another EOL
                for (; charIndex > 0; charIndex--)
                {
                    if (finalLine[charIndex] == '\n')
                    {
                        foundEOL = true;
                        break;
                    }
                }
            }

            // If we have found the correct offset line, extract it, else report error
            if (foundEOL)
            {
                // Extract the offset line after removing the leading and trailing EOL characters
                offsetLine = finalLine.Substring(charIndex).Trim();
            }
            else
            {
                // If we could not find any EOL, then the file is corrupt. Report error
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.FS_CORRUPT, 
                            "Malformed offset line in revision file: Final line lacks trailing End-Of-Line or is more than 64 characters in length");
                SVNErrorManager.error(err);
            }

            return offsetLine;
        }

        public int Read(MemoryStream buffer)
        {
            if (buffer == null)
            {
                return 0;
            }
            byte[] data = new byte[(int)(buffer.Length - buffer.Position)];
            streamReader.BaseStream.Position = currentPosition;
            int bytesRead = streamReader.BaseStream.Read(data,
                                                    (int)buffer.Position, data.Length);
            currentPosition += bytesRead;
            buffer.Write(data, 0, bytesRead);
            streamReader.DiscardBufferedData();

            //char[] readBuffer = new char[buffer.Capacity - buffer.Position];
            //int bytesRead = streamReader.ReadBlock(readBuffer, 0, readBuffer.Length);
            //currentPosition += bytesRead;
            //buffer.Write(Encoding.Default.GetBytes(readBuffer), (int)buffer.Position, bytesRead);

            return bytesRead;
        }

        public int Read(ref byte[] buffer)
        {
            if (buffer == null)
                return 0;
            char[] readBuffer = new char[buffer.Length];
            int bytesRead = streamReader.ReadBlock(readBuffer, 0, buffer.Length);
            currentPosition += bytesRead;
            buffer = Encoding.Default.GetBytes(readBuffer);
            return bytesRead;
        }
    }

}