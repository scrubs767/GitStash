using System;

using Microsoft.TeamFoundation.Controls;
using Scrubs.TeamExplorer;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using GitStash.UI;
using System.Windows.Threading;
using GitStash.Common;
using GitWrapper;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using GitStash.ViewModels;
using SecondLanguage;

namespace GitStash
{


    [TeamExplorerPage(GitStashPackage.StashPage, Undockable = true)]
    public class StashPage : TeamExplorerPageBase, INavigateable
    {
        private static ITeamExplorer teamExplorer;
        Translator T;
        private IGitStashWrapper gitWrapper;

        public override void Initialize(object sender, PageInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            teamExplorer = (ITeamExplorer)ServiceProvider.GetService(typeof(ITeamExplorer));
            gitWrapper = GetService<IGitStashWrapper>();
            T = GetService<IGitStashTranslator>().Translator;
            Title = T["Git Stash"];
            PageContent = new PageControl(new PageViewModel(this, gitWrapper, T));            
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

