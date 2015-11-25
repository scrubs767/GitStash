using GitWrapper;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GitStash.ViewModels
{
    public class RecommendedActionsViewModel : INotifyPropertyChanged
    {
       // private static IGitExt gitService;
        private IGitStashWrapper wrapper;
        private string _newStashMessage = "";
        bool stashAll = false;
        bool stashUntracked = false;
        bool stashIgnored = false;

        public RecommendedActionsViewModel(IGitStashWrapper wrapper)
        {
            this.wrapper = wrapper;
            wrapper.StashesChangedEvent += GitService_PropertyChanged;
            CreateStashButtonCommand = new RelayCommand(p => OnClickCreateStashButton(), p => CanClickCreateButton);
            NewStashMessage = "";
        }

        private void GitService_PropertyChanged(object sender, StashesChangedEventArgs e)
        {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanCreateStash"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanClickCreateButton"));
        }

        
        public string NewStashMessage
        {
            get { return _newStashMessage; }
            set
            {
                _newStashMessage = value;
                //CanClickCreateStashButton = wrapper.WorkingDirHasChanges() && NewStashMessage.Length > 0;
                //if (PropertyChanged != null)
                //    PropertyChanged(this, new PropertyChangedEventArgs("CreateStashButtonCommand"));

            }
        }
        
        public bool StashKeepIndex { get; set; }

        public RelayCommand CreateStashButtonCommand { get; private set; }

        public bool CanCreateStash
        {
            get { return wrapper.WorkingDirHasChanges(); }
        }

        public bool CanClickCreateButton
        {
            get { return wrapper.WorkingDirHasChanges() && NewStashMessage.Length > 0; }
        }

        public bool StashAll
        {
            get
            {
                return stashAll;
            }

            set
            {
                stashAll = value;
                if (value == true)
                {
                    StashUntracked = false;
                    StashIgnored = false;
                    OnPropertyChanged("StashUntracked");
                    OnPropertyChanged("StashIgnored");
                }
            }
        }

        public bool StashUntracked
        {
            get
            {
                return stashUntracked;
            }

            set
            {
                stashUntracked = value;
                if (value == true)
                {
                    StashAll = false;
                    StashIgnored = false;
                    OnPropertyChanged("StashAll");
                    OnPropertyChanged("StashIgnored");
                }
            }
        }

        public bool StashIgnored
        {
            get
            {
                return stashIgnored;
            }

            set
            {
                stashIgnored = value;
                if (value == true)
                {
                    StashAll = false;
                    StashUntracked = false;
                    OnPropertyChanged("StashAll");
                    OnPropertyChanged("StashUntracked");
                }
            }
        }

        private void OnClickCreateStashButton()
        {
            IGitStashSaveOptions options = new GitStashOptions { All = StashAll, Ignored = StashIgnored, KeepIndex = StashKeepIndex, Untracked = StashUntracked, Message = NewStashMessage };
            if (wrapper.SaveStash(options).Success)
            {
                NewStashMessage = "";
                PropertyChanged(this, new PropertyChangedEventArgs("Stashes"));
                PropertyChanged(this, new PropertyChangedEventArgs("NewStashMessage"));
                PropertyChanged(this, new PropertyChangedEventArgs("CreateStashButtonCommand"));
            }
            else
            {

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
