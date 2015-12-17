using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using GitWrapper;
using TeamExplorer.Common;
using GitStash.Common;
using SecondLanguage;

namespace GitStash.ViewModels
{
    public class StashesViewModel : INotifyPropertyChanged
    {
        IGitStashWrapper git;
        List<StashViewModel> stashes;
        private ITeamExplorerBase page;
        Translator T;
        public StashesViewModel(IGitStashWrapper git, ITeamExplorerBase page, Translator T)
        {
            this.page = page;
            this.git = git;
            this.T = T;
            git.StashesChangedEvent += Git_StashesChangedEvent;
        }

        private void Git_StashesChangedEvent(object sender, StashesChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Stashes"));
            }
        }

        public IList<StashViewModel> Stashes
        {
            get
            {
                if(stashes != null) stashes.ForEach(s => s.AfterDelete -= AfterDeleteStash);
                stashes = git.Stashes.Select(s => new StashViewModel(git, s, page, T)).ToList();
                stashes.ForEach(s => s.AfterDelete += AfterDeleteStash);
                return stashes;
            }
        }

        private void AfterDeleteStash(object source, AfterDeleteStashEventArgs e)
        {
            StashViewModel vm = source as StashViewModel;
            if (source == null)
                return;
            vm.AfterDelete -= AfterDeleteStash;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Stashes"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        
    }
}
