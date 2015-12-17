using GitStash.Common;
using GitStash.UI;
using GitStash.ViewModels;
using GitWrapper;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using SecondLanguage;
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
        private IGitStashWrapper wrapper;
        Translator T;
        public StashesSection()
        {
            
        }
        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            T = GetService<IGitStashTranslator>().Translator;
            this.wrapper = GetService<IGitStashWrapper>();
            vm = new StashesViewModel(wrapper, this, T);
            vm.PropertyChanged += StashesPropertyChanged;
            Title = T["Stashes({0})", vm.Stashes.Count()];
            IsVisible = true;
            SectionContent = new StashesControl(vm);
        }

        private void StashesPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Title = Title = T["Stashes({0})", vm.Stashes.Count()];
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
