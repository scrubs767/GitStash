using GitStash.Common;
using GitStash.UI;
using GitStash.ViewModels;
using GitWrapper;
using Microsoft.TeamFoundation.Controls;
using SecondLanguage;
using System;
using Scrubs.TeamExplorer;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;


namespace GitStash.Sections
{
    [TeamExplorerSection(GitStashPackage.RecommendedActionsSection, GitStashPackage.StashPage, 120)]
    public class RecommendedActionsSection : TeamExplorerSectionBase
    {
        private IGitStashWrapper wrapper;

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            this.wrapper = GetService<IGitStashWrapper>();            
            SectionContent = new RecommendedActionsControl(new RecommendedActionsViewModel(wrapper,this));
            Translator T = GetService<IGitStashTranslator>().Translator;
            Title = T["Create Stash"];
            IsVisible = true;
        }
        public override void Refresh()
        {
            var service = GetService<ITeamExplorerPage>();
            service.Refresh();
        }
    }
}
