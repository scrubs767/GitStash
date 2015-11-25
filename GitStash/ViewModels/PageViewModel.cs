using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.TeamFoundation.Controls;
using GitStash.ViewModels;
using Microsoft.VisualStudio.Shell.Interop;
using System.Linq;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using GitWrapper;

namespace GitStash
{
    public class PageViewModel : INotifyPropertyChanged
    {
        private static IGitExt gitService;
        private IGitStashWrapper wrapper;

        

        public PageViewModel(IGitStashWrapper wrapper)
        {
           this. wrapper = wrapper;
            SelectBranchCommand = new RelayCommand(p => SelectBranch(), p => CanSelectBranch);
        }

        public string CurrentBranch
        {
            get
            {
                if (gitService != null)
                    return gitService.ActiveRepositories.FirstOrDefault().CurrentBranch.Name;

                return "no current branch";
            }
        }

        private void SelectBranch()
        {
            StashPage.ShowPage(TeamExplorerPageIds.GitBranches);
        }

        public bool CanSelectBranch
        {
            get { return true; }
        }

        public RelayCommand SelectBranchCommand { get; private set; }

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
