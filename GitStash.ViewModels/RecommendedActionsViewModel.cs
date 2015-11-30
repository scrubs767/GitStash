using GitWrapper;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.MVVM;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TeamExplorer.Common;

namespace GitStash.ViewModels
{
    public class RecommendedActionsViewModel : INotifyPropertyChanged
    {
        private IGitStashWrapper wrapper;
        bool stashAll = false;
        bool stashUntracked = false;
        bool stashIgnored = false;
        ITeamExplorerBase page;

        public RecommendedActionsViewModel(IGitStashWrapper wrapper, ITeamExplorerBase page)
        {
            this.page = page;
            this.wrapper = wrapper;
            wrapper.StashesChangedEvent += GitService_PropertyChanged;
            CreateStashButtonCommand = new RelayCommand(p => OnClickCreateStashButton(), p => CanClickCreateButton);
            NewStashMessage = "";
        }

        private void GitService_PropertyChanged(object sender, StashesChangedEventArgs e)
        {
                OnPropertyChanged("CanCreateStash");
                OnPropertyChanged("CanClickCreateButton");
        }
        
        public string NewStashMessage { get; set; }
        
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
            IGitStashResults results = wrapper.SaveStash(options);
            if (!string.IsNullOrEmpty(results.Message))
                page.ShowNotification(results.Message, NotificationType.Information);
            if (results.Success)
            {
                NewStashMessage = "";
                OnPropertyChanged("Stashes");
                OnPropertyChanged("NewStashMessage");
                OnPropertyChanged("CreateStashButtonCommand");
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
