//------------------------------------------------------------------------------
// <copyright file="GitStashPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using GitWrapper;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Interop;
using System.Linq;
using GitStash.Common;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;

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
    [ProvideService(typeof(IGitStashWrapper))]
    [ProvideService(typeof(IGitStashProjectEvents))]
    //[ProvideOptionPage(typeof(Options.StashOptionsPage),"Git Stash", "General", 0, 0, true)]
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
        private IGitStashWrapper wrapper = null;
        private IGitStashProjectEvents events = null;
        private IGitExt gitService = null;
        private GitStashProjects projects;

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

        private void TraceWriteLine(string msg)
        {
            System.Diagnostics.Trace.WriteLine("**********" + msg);
        }
        private object CreateGitWrapperService(IServiceContainer container, Type serviceType)
        {
            TraceWriteLine("Service Requested: " + serviceType.FullName);
            if (typeof(IGitStashWrapper) == serviceType)
            {
                if (wrapper == null)
                {
                    IVsOutputWindowPane outputWindow;
                    var outWindow = GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
                    var customGuid = new Guid("D9B93453-B887-407F-99EC-66C6FD5CA84C");
                    outWindow.CreatePane(ref customGuid, "Git Stash", 1, 1);
                    outWindow.GetPane(ref customGuid, out outputWindow);
                    if (gitService.ActiveRepositories.FirstOrDefault() != null)
                    {                        
                        string path = gitService.ActiveRepositories.FirstOrDefault().RepositoryPath;
                        TraceWriteLine("Creating Wrapper service with path: " + path);
                        wrapper = new GitStashWrapper(path, events, new OutputWindowLogger(outputWindow), projects);
                    }
                    else
                    {
                        TraceWriteLine("Creating Wrapper service.");
                        wrapper = new GitStashWrapper(events, new OutputWindowLogger(outputWindow), projects);
                    }
                }
                return wrapper;
            }

            if (typeof(IGitStashProjectEvents) == serviceType)
            {
                return events;
            }

            return null;
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            TraceWriteLine("Package Initialization: Starting");
            IServiceContainer serviceContainer = this as IServiceContainer;
            ServiceCreatorCallback callback =
               new ServiceCreatorCallback(CreateGitWrapperService);
            serviceContainer.AddService(typeof(IGitStashWrapper), callback, true);
            serviceContainer.AddService(typeof(IGitStashProjectEvents), callback, true);
            gitService = (IGitExt)GetService(typeof(IGitExt));
            gitService.PropertyChanged += GitService_PropertyChanged;
            this.projects = new GitStashProjects(this);
            this.events = new GitStashFileWatcher();
            if(gitService.ActiveRepositories.FirstOrDefault() != null)
            {
                string path = gitService.ActiveRepositories.FirstOrDefault().RepositoryPath;
                TraceWriteLine("Setting directory: " + path);
                events.ChangeDirectory(path);
            }
            TraceWriteLine("Package Initialization: Done");
        }

        private void GitService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (gitService.ActiveRepositories.FirstOrDefault() != null)
            {
                string path = gitService.ActiveRepositories.FirstOrDefault().RepositoryPath;
                TraceWriteLine("Changing directory: " + path);
                events.ChangeDirectory(path);
            }
        }


        #endregion
    }
}
