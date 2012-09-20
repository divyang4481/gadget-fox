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

using System.Reflection;
using System.Runtime.InteropServices;
using log4net.Config;
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly : AssemblyTitle("DotSVN.Tests")]
[assembly : AssemblyDescription("")]
[assembly : AssemblyConfiguration("")]
[assembly : AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly : ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly : Guid("1db8ad6e-7e48-40e9-ad6c-523e3c249533")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly : AssemblyVersion("1.0.0.0")]
[assembly : AssemblyFileVersion("1.0.0.0")]


// Configure log4net using the .config file

[assembly : XmlConfigurator(Watch = false)]