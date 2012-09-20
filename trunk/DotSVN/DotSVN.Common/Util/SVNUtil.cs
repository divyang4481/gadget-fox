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
using System.Security.Cryptography;
using System.Text;

namespace DotSVN.Common.Util
{
    public class SVNUtil
    {
        /// <summary>
        /// Reverses string values.
        /// </summary>
        /// <param name="text">The string object containing the string to be reversed.</param>
        /// <returns>The reversed string.</returns>
        public static string ReverseString(string text)
        {
            char[] charArray = text.ToString().ToCharArray();
            Array.Reverse(charArray);
            return new String(charArray);
        }

        /// <summary>
        /// Computes the hash for the given data using MD5.
        /// </summary>
        /// <param name="hashData">The data to HASH.</param>
        /// <returns></returns>
        public static string ComputeHash(string hashData)
        {
            return ComputeHash(Encoding.Default.GetBytes(hashData));
        }

        /// <summary>
        /// Computes the hash for the given data using MD5.
        /// </summary>
        /// <param name="hashData">The data to HASH.</param>
        /// <returns></returns>
        public static string ComputeHash(byte[] hashData)
        {
            MD5 md5Hasher = MD5.Create();

            // Convert the hashData to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(hashData);

            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string.
            for (int index = 0; index < data.Length; index++)
            {
                sBuilder.Append(data[index].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
