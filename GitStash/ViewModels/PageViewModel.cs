using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.TeamFoundation.Controls;
using GitStash.ViewModels;
using Microsoft.VisualStudio.Shell.Interop;
using System.Linq;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;

namespace GitStash
{
    public class PageViewModel : INotifyPropertyChanged
    {
        private static IGitExt gitService;
        private static ITeamExplorer teamExplorer;
        private static IVsOutputWindowPane outputWindow;
        private object serviceProvider;

        public string CurrentBranch
        {
            get
            {
                if (gitService != null)
                    return gitService.ActiveRepositories.FirstOrDefault().CurrentBranch.Name;

                return "no current branch";
            }
        }

        public PageViewModel(IServiceProvider serviceProvider)
        {
            gitService = (IGitExt)serviceProvider.GetService(typeof(IGitExt));
            SelectBranchCommand = new RelayCommand(p => SelectBranch(), p => CanSelectBranch);
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
