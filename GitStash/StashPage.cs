using System;
using System.ComponentModel;

using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using TeamExplorer.Common;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using GitStash.UI;
using System.Windows.Threading;
using GitStash.Common;
using GitWrapper;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using GitStash.ViewModels;

namespace GitStash
{
    

    [TeamExplorerPage(GitStashPackage.StashPage, Undockable = true)]
    public class StashPage : TeamExplorerBasePage, INavigateable
    {
        private static ITeamExplorer teamExplorer;
        private static IVsOutputWindowPane outputWindow;
        private IGitStashWrapper gitWrapper;
        private IGitExt gitService;

        [ImportingConstructor]
        public StashPage([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
           Title = "Git Stash";
            teamExplorer = (ITeamExplorer)serviceProvider.GetService(typeof(ITeamExplorer));
            gitService = (IGitExt)serviceProvider.GetService(typeof(IGitExt));                        
            var outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            var customGuid = new Guid("D9B93453-B887-407F-99EC-66C6FD5CA84C");
            outWindow.CreatePane(ref customGuid, "Git Stash", 1, 1);
            outWindow.GetPane(ref customGuid, out outputWindow);
            gitWrapper = new GitStashWrapper(serviceProvider, new OutputWindowLogger(outputWindow));
        }

        public override void Initialize(object sender, PageInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            gitWrapper = GetService<IGitStashWrapper>();
            PageContent = new PageControl(new PageViewModel(this, gitWrapper));            
        }

        public override object GetExtensibilityService(Type serviceType)
        {
            if (serviceType == typeof(IGitStashWrapper))
                return gitWrapper;
            return base.GetExtensibilityService(serviceType);
        }
        public void ShowPage(string page)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() =>
                    teamExplorer.NavigateToPage(new Guid(page), null)));

        }
    }
}

