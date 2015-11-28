using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Git Stash")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("0.1.*")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyDescription("Flavor=Debug")] 
#else
[assembly: AssemblyConfiguration("Retail")]
[assembly: AssemblyDescription("Flavor=Retail")] 
#endif
