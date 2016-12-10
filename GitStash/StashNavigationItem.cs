using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using GitStash.Properties;
using Scrubs.TeamExplorer;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using SecondLanguage;
using GitStash.Common;

namespace GitStash
{
    [TeamExplorerNavigationItem(GitStashPackage.StashNavigationItem, 1500, TargetPageId = GitStashPackage.StashPage)]
    public class StashNavigationItem : TeamExplorerNavigationItemBase
    {

        private readonly ITeamExplorer teamExplorer;
        private readonly IGitExt gitService;
        Translator T;
        [ImportingConstructor]
        public StashNavigationItem([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            this.Image = Resources.StashIcon;
            this.IsVisible = false;
            this.T = GetService<IGitStashTranslator>().Translator;
            this.Text = T["Git Stash"];
            teamExplorer = GetService<ITeamExplorer>();
            gitService = (IGitExt)serviceProvider.GetService(typeof(IGitExt));
            teamExplorer.PropertyChanged += TeamExplorerOnPropertyChanged;
            IsVisible = true;
        }

        private void TeamExplorerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            UpdateVisible();
        }

        private void UpdateVisible()
        {
            IsEnabled = false;
            if (gitService != null && gitService.ActiveRepositories.Any())
            {
                IsEnabled = true;
            }
        }

        public override void Execute()
        {
            var service = this.GetService<ITeamExplorer>();
            if (service == null)
            {
                return;
            }
            service.NavigateToPage(new Guid(GitStashPackage.StashPage), null);
        }

    }
}
