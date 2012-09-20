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

using System.IO;
using DotSVN.Common.Util;

namespace DotSVN.Server.RepositoryAccess.File
{
    public class FSRepositoryUtil
    {
        public static void copy(Stream src, Stream dst)
        {
            try
            {
                byte[] buffer = new byte[102400];
                while (true)
                {
                    int length = src.Read(buffer, 0, buffer.Length);
                    if (length > 0)
                    {
                        dst.Write(buffer, 0, length);
                    }
                    if (length != 102400)
                    {
                        break;
                    }
                }
            }
            catch (IOException ioe)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.IO_ERROR, ioe.Message);
                SVNErrorManager.error(err, ioe);
            }
        }

    }
}