﻿//------------------------------------------------------------------------------
// <copyright file="GitStashPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System.Windows.Forms;
using GitStash;
using System.Windows;

namespace GitStash
{
    //public class OptionPageGrid : DialogPage
    //{
    //    private int optionInt = 256;

    //    [Category("GitStash")]
    //    [DisplayName("Git Location")]
    //    [Description("The Directory where Git is located.")]
    //    public string GitLocation { get; set; }
    //}

    
    

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>

    [ProvideOptionPage(typeof(Options.StashOptionsPage),"GitStash", "General", 0, 0, true)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(GitStashPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class GitStashPackage : Package
    {
        /// <summary>
        /// GitStashPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "afd07cd2-58ae-47f6-a16d-fea9a25da808";
        public const string StashNavigationItem = "8C35B3DF-D7CC-45BC-B958-BFAE3E157A21";
        public const string StashPage = "D40EAA8D-8CA8-47A2-9DF6-E3ABD9FD4FE7";
        public const string StashOptionsPage = "35836859-8302-47BA-AE29-F3825D82C179";


        public const string StashesSection = "E162E6A9-66E3-409D-B825-7BB31C3B60C4";
        public const string RecommendedActionsSection = "{9CC25864-F7AA-45C2-9F78-0B1DDE2547A9}";
        /// <summary>
        /// Initializes a new instance of the <see cref="GitStashPackage"/> class.
        /// </summary>
        public GitStashPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        #endregion
    }
}