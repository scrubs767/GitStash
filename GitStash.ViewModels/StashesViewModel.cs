using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using GitWrapper;

namespace GitStash.ViewModels
{
    public class StashesViewModel : INotifyPropertyChanged
    {
        IGitStashWrapper git;

        public StashesViewModel(IGitStashWrapper git)
        {
            this.git = git;
            // I think one of these are redundant
            git.StashesChangedEvent += Git_StashesChangedEvent;
        }

        private void Git_StashesChangedEvent(object sender, StashesChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Stashes"));
            }
        }

        List<StashViewModel> stashes;

        public IList<StashViewModel> Stashes
        {
            get
            {
                if(stashes != null) stashes.ForEach(s => s.AfterDelete -= AfterDeleteStash);
                stashes = git.Stashes.Select(s => new StashViewModel(git, s)).ToList();
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
