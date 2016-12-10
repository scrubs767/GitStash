using GitStash.Common;
using GitStash.UI;
using GitStash.ViewModels;
using GitWrapper;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using SecondLanguage;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using Scrubs.TeamExplorer;

namespace GitStash.Sections
{
    [TeamExplorerSection(GitStashPackage.StashesSection, GitStashPackage.StashPage, 220)]
    public class StashesSection : TeamExplorerSectionBase
    {
        StashesViewModel vm;
        private IGitStashWrapper wrapper;
        Translator T;

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
