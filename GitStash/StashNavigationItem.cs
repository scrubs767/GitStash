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
using System.Windows.Media;

namespace GitStash
{
    [TeamExplorerNavigationItem(GitStashPackage.StashNavigationItem, 1500, TargetPageId = GitStashPackage.StashPage)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StashNavigationItem : TeamExplorerNavigationItemBase
    {
        private readonly ITeamExplorer teamExplorer;
        private readonly IGitExt gitService;
        Translator T;

        [ImportingConstructor]
        public StashNavigationItem([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            Image = Resources.StashIcon;
            ArgbColor = UI.Colors.NavigationItemGreen.ToInt32();
            IsEnabled = true;
            this.T = GetService<IGitStashTranslator>().Translator;
            Text = T["GitStash"];
            teamExplorer = GetService<ITeamExplorer>();
            gitService = (IGitExt)serviceProvider.GetService(typeof(IGitExt));
            teamExplorer.PropertyChanged += TeamExplorerOnPropertyChanged;
            Refresh();
        }

        private void TeamExplorerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Refresh();
        }

        private void Refresh()
        {
            IsVisible = false;
            if (gitService != null && gitService.ActiveRepositories.Any())
            {
                IsVisible = true;
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
