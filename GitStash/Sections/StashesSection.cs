using GitStash.UI;
using GitStash.ViewModels;
using GitWrapper;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamExplorer.Common;

namespace GitStash.Sections
{
    [TeamExplorerSection(GitStashPackage.StashesSection, GitStashPackage.StashPage, 220)]
    public class StashesSection : TeamExplorerBaseSection
    {
        StashesViewModel vm;
        IGitStashWrapper wrapper;

        [ImportingConstructor]
        public StashesSection(IGitStashWrapper wrapper)
        {
            vm = new StashesViewModel(wrapper);
            vm.PropertyChanged += StashesPropertyChanged;
            Title = String.Format("Stashes({0})",vm.Stashes.Count());
            IsVisible = true;
            SectionContent = new StashesControl(vm);
        }

        private void StashesPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Title = String.Format("Stashes({0})", vm.Stashes.Count());
            Refresh();
        }

        public override void Refresh()
        {
            var service = GetService<ITeamExplorerPage>();
            if(service != null)
                service.Refresh();
        }
    }
}
