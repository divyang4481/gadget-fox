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

namespace DotSVN.Common.Interfaces
{
    /// <summary>
    /// The <c>ISVNAuthenticationManager</c> is implemented by manager classes used by <c>SVNRepository</c> drivers for
    /// user authentication purposes.
    /// <para>When an <c>SVNRepository</c> driver is created, you should provide an authentication manager via a call to:
    /// <code>repository.setAuthenticationManager(authManager)</code></para>
    /// </summary>
    public interface ISVNAuthenticationManager
    {
    }
}