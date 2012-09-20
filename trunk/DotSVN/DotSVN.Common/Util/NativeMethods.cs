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

using System.Runtime.InteropServices;
using System.Text;

namespace DotSVN.Common.Util
{
    internal class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern uint GetTempFileName(string tmpPath, string prefix, uint uniqueIdOrZero,
                                                    StringBuilder tmpFileName);
    }
}