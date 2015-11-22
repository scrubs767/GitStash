using GitStash.UI;
using GitStash.ViewModels;
using Microsoft.TeamFoundation.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamExplorer.Common;

namespace GitStash.Sections
{
    [TeamExplorerSection(GitStashPackage.RecommendedActionsSection, GitStashPackage.StashPage, 120)]
    public class RecommendedActionsSection : TeamExplorerBaseSection
    {
        public RecommendedActionsSection()
        {
            Title = "Recommended Actions";
            IsVisible = true;
            //UpdateVisibleState();
            SectionContent = new RecommendedActionsControl(new RecommendedActionsViewModel(this));
        }

        public override void Refresh()
        {
            var service = GetService<ITeamExplorerPage>();
            service.Refresh();
        }
    }
}
