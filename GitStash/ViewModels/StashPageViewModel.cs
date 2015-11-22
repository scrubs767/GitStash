using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.TeamFoundation.Controls;
using GitStash.ViewModels;
using System.Collections.Generic;
using GitWrapper;
using Microsoft.VisualStudio.Shell.Interop;
using System.Linq;

namespace GitStash
{
    public class StashPageViewModel : INotifyPropertyChanged
    {
        //private static IGitExt gitService;
        private static ITeamExplorer teamExplorer;
        private static IVsOutputWindowPane outputWindow;
        private object serviceProvider;

        public IEnumerable<GitStashItem> Stashes
        {
            get; set;
        }

        public string CurrentBranch
        {
            get
            {
               // if(gitService != null)
                //return gitService.ActiveRepositories.FirstOrDefault().CurrentBranch.Name;

                return "no current branch";
            }
        }

        public StashPageViewModel(IServiceProvider serviceProvider)
        {
            SelectBranchCommand = new RelayCommand(p => SelectBranch(), p => CanSelectBranch);
            StashDropDownCommand = new RelayCommand(p => ShowStash(), p => CanStash);
            PopDropDownCommand = new RelayCommand(p => ShowPop(), p => CanPop);
            ApplyDropDownCommand = new RelayCommand(p => ShowApply(), p => CanApply);
            //gitService = (IGitExt)serviceProvider.GetService(typeof(IGitExt));
            ShowStashSection = false;
            ShowPopSection = false;
            ShowApplySection = false;
        }

        public void ShowStash()
        {
            ShowStashSection = true;
            ShowPopSection = false;
            ShowApplySection = false;
            OnPropertyChanged("ShowStashSection");
            OnPropertyChanged("ShowPopSection");
            OnPropertyChanged("ShowApplySection");
        }

        public void ShowPop()
        {
            ShowStashSection = false;
            ShowPopSection = true;
            ShowApplySection = false;
            OnPropertyChanged("ShowStashSection");
            OnPropertyChanged("ShowPopSection");
            OnPropertyChanged("ShowApplySection");
        }

        public void ShowApply()
        {
            ShowStashSection = false;
            ShowPopSection = false;
            ShowApplySection = true;
            OnPropertyChanged("ShowStashSection");
            OnPropertyChanged("ShowPopSection");
            OnPropertyChanged("ShowApplySection");
        }

        public bool ShowStashSection { get; set; }
        public bool ShowApplySection { get; set; }
        public bool ShowPopSection { get; set; }

        private void SelectBranch()
        {
            StashPage.ShowPage(TeamExplorerPageIds.GitBranches);
        }

        public bool CanSelectBranch
        {
            get { return false; }
        }

        public RelayCommand SelectBranchCommand { get; private set; }
        public RelayCommand StashDropDownCommand { get; private set; }
        public RelayCommand PopDropDownCommand { get; private set; }
        public RelayCommand ApplyDropDownCommand { get; private set; }

        private bool stashFeatureVisible = true;
        private bool popFeatureVisible = true;
        private bool applyFeatureVisible = true;


        public bool CanStash
        {
            get
            {
                return stashFeatureVisible;
            }

            set
            {
                stashFeatureVisible = value;
            }
        }

        public bool CanPop
        {
            get
            {
                return popFeatureVisible;
            }

            set
            {
                popFeatureVisible = value;
            }
        }

        public bool CanApply
        {
            get
            {
                return applyFeatureVisible;
            }

            set
            {
                applyFeatureVisible = value;
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
