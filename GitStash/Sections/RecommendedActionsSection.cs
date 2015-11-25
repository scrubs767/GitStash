using GitStash.UI;
using GitStash.ViewModels;
using GitWrapper;
using Microsoft.TeamFoundation.Controls;
using System.ComponentModel.Composition;
using TeamExplorer.Common;
using System;

namespace GitStash.Sections
{
    [TeamExplorerSection(GitStashPackage.RecommendedActionsSection, GitStashPackage.StashPage, 120)]
    public class RecommendedActionsSection : TeamExplorerBaseSection
    {
        private IGitStashWrapper wrapper;

        [ImportingConstructor]
        public RecommendedActionsSection(IGitStashWrapper wrapper)
        {
            this.wrapper = wrapper;
            wrapper.StashesChangedEvent += GitService_PropertyChanged;
        }

        private void GitService_PropertyChanged(object sender, StashesChangedEventArgs e)
        {
            if (wrapper.WorkingDirHasChanges())
                Title = "Create Stash";
            else
                Title = "Create Stash (Nothing to Stash)";
            Refresh();
        }

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            SectionContent = new RecommendedActionsControl(new RecommendedActionsViewModel(wrapper));
            Title = "Create Stash (Nothing to Stash)";
            IsVisible = true;
        }
        public override void Refresh()
        {
            var service = GetService<ITeamExplorerPage>();
            service.Refresh();
        }
    }
}
