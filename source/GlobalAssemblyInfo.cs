using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany("The Framework Factory, LLC")]
[assembly: AssemblyCopyright("Copyright 2016 The Framework Factory, LLC")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

[assembly: NeutralResourcesLanguage( "en-US" )]

#if DEBUG
[assembly: AssemblyConfiguration( "Debug" )]
#else
[assembly: AssemblyConfiguration( "Release" )]
#endif

// Other Settings