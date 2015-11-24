using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitStash.Sections;
using System.ComponentModel;
using GitWrapper;

namespace GitStash.ViewModels
{
    public class StashesViewModel : INotifyPropertyChanged
    {
        GitStashWrapper git;

        public StashesViewModel(GitStashWrapper git)
        {
            this.git = git;
            git.PropertyChanged += Git_PropertyChanged;

        }

        private void Git_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
