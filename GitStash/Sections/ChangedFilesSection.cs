using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Controls;
using TeamExplorer.Common;
using GitStash.UI;
using GitStash.ViewModels;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using EnvDTE;

namespace GitStash.Sections
{
    [TeamExplorerSection(GitStashPackage.ChangesSection, GitStashPackage.StashPage, 200)]
    public class ChangedFilesSection : TeamExplorerBaseSection
    {
        public ChangedFilesSection()
        {
            Title = "Changes";
            IsVisible = false;
            //model = new FeaturesViewModel(this);
            //UpdateVisibleState();
            var service = GetService<IGitExt>();
            DTE dte = GetService<DTE>();
            string solutionDir = System.IO.Path.GetDirectoryName(dte.Solution.FullName);
            SectionContent = new ChangedFilesControl(new ChangedFilesViewModel(service, solutionDir));
        }

        public override void Refresh()
        {
            var service = GetService<ITeamExplorerPage>();
            service.Refresh();
        }
    }
}
