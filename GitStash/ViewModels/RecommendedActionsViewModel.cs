using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitStash.Sections;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell.Interop;
using GitWrapper;
using System.Runtime.CompilerServices;

namespace GitStash.ViewModels
{
    public class RecommendedActionsViewModel : INotifyPropertyChanged
    {
        private static IGitExt gitService;
        private GitStashWrapper wrapper;

        public RecommendedActionsViewModel(IServiceProvider serviceProvider)
        {
            ITeamExplorer teamExplorer = (ITeamExplorer)serviceProvider.GetService(typeof(ITeamExplorer));
            teamExplorer.PropertyChanged += GitService_PropertyChanged;
            wrapper = new GitStashWrapper((IGitExt)serviceProvider.GetService(typeof(IGitExt)));
            gitService = ((IGitExt)serviceProvider.GetService(typeof(IGitExt)));
            ((IGitExt)serviceProvider.GetService(typeof(IGitExt))).PropertyChanged += GitService_PropertyChanged;
            CreateStashButtonCommand = new RelayCommand(p => OnClickCreateStashButton(), p => CanClickCreateStashButton);
            NewStashMessage = "";
        }

        private void GitService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("CanSelectBranch"));
                PropertyChanged(this, new PropertyChangedEventArgs("CreateStashNameEnabled"));
                PropertyChanged(this, new PropertyChangedEventArgs("CreateStashButtonEnabled"));
                PropertyChanged(this, new PropertyChangedEventArgs("CanClickCreateStashButton"));
            }
        }


        private string _newStashMessage = "";
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
        
        public bool CreateStashNameEnabled { get { return wrapper.WorkingDirHasChanges(); } }
        //public bool CreateStashButtonEnabled
        //{
        //    get
        //    {
        //        return wrapper.WorkingDirHasChanges() && NewStashMessage.Length > 0;
        //    }
        //}


        public bool StashKeepIndex { get; set; }

        bool stashAll = false;
        bool stashUntracked = false;
        bool stashIgnored = false;


        
        public RelayCommand CreateStashButtonCommand { get; private set; }
        public bool CanClickCreateStashButton
        {
            get { return true; }
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

        public void Update()
        {
            OnPropertyChanged("CurrentBranch");
        }
    }
}
