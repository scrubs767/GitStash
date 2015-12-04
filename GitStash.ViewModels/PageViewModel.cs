using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.TeamFoundation.Controls;
using GitStash.ViewModels;
using Microsoft.VisualStudio.Shell.Interop;
using System.Linq;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using GitWrapper;
using GitStash.Common;
using Microsoft.TeamFoundation.MVVM;
using SecondLanguage;

namespace GitStash.ViewModels
{
    public class PageViewModel : INotifyPropertyChanged
    {
        IGitStashWrapper wrapper;
        INavigateable page;
        Translator T;
        public PageViewModel(INavigateable page, IGitStashWrapper wrapper, Translator T)
        {
            this.wrapper = wrapper;
            this.page = page;
            this.T = T;
            SelectBranchCommand = new RelayCommand(p => SelectBranch(), p => CanSelectBranch);
            SelectChangesCommand = new RelayCommand(p => SelectChanges(), p => CanSelectChanges);
            wrapper.StashesChangedEvent += Wrapper_StashesChangedEvent;
            CanSelectChanges = wrapper.WorkingDirHasChanges();
        }

        private void Wrapper_StashesChangedEvent(object sender, StashesChangedEventArgs e)
        {
            CanSelectChanges = true;
            OnPropertyChanged("SelectChangesCommand");
            OnPropertyChanged("ChangesText");
        }

        private void SelectChanges()
        {
            page.ShowPage(TeamExplorerPageIds.GitChanges);
        }

        public bool CanSelectChanges { get; private set; }

        public string ChangesText { get { return wrapper.WorkingDirHasChanges() ? T["Changes"] : T["No changes available"]; } }

        public RelayCommand SelectChangesCommand { get; set; }

        public string CurrentBranch
        {
            get
            {
                return wrapper.CurrentBranch;
            }
        }

        private void SelectBranch()
        {
            page.ShowPage(TeamExplorerPageIds.GitBranches);
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
