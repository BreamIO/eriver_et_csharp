﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ETServer")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Bream Team")]
[assembly: AssemblyProduct("ETServer")]
[assembly: AssemblyCopyright("Copyright ©  2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("7d6ce9b9-5776-4fff-b60a-30f9261dc789")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.5.*")]
[assembly: AssemblyFileVersion("0.5.0.0")]

// Configure log4net using the .config file
//[assembly: log4net.Config.XmlConfigurator(ConfigFile = "ETServer_logging", ConfigFileExtension = ".xml", Watch = true)]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "logging.config", Watch = true)]
// This will cause log4net to look for a configuration file
// called logging.xml in the application base
// directory (i.e. the directory containing TestApp.exe)
// The config file will be watched for changes.
