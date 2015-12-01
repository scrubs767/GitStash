using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using GitStash.Properties;
using TeamExplorer.Common;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using SecondLanguage;

namespace GitStash
{
    [TeamExplorerNavigationItem(GitStashPackage.StashNavigationItem, 1500)]
    public class StashNavigationItem : TeamExplorerBaseNavigationItem
    {

        private readonly ITeamExplorer teamExplorer;
        private readonly IGitExt gitService;
        Translator T = Translator.Default;
        [ImportingConstructor]
        public StashNavigationItem([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.Image = Resources.StashIcon;
            this.IsVisible = false;
            T.RegisterTranslationsByCulture(@"po\*.po");
            this.Text = T["Git Stash"];
            teamExplorer = GetService<ITeamExplorer>();
            gitService = (IGitExt)serviceProvider.GetService(typeof(IGitExt));
            teamExplorer.PropertyChanged += TeamExplorerOnPropertyChanged;
        }

        private void TeamExplorerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            UpdateVisible();
        }

        private void UpdateVisible()
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
