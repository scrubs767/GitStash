using GitWrapper;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.MVVM;
using SecondLanguage;
using System;
using System.ComponentModel;
using Scrubs.TeamExplorer;

namespace GitStash.ViewModels
{
    public class StashViewModel : INotifyPropertyChanged
    {
        Translator T;
        public delegate void AfterDeleteHandler(object source, AfterDeleteStashEventArgs e);
        public event AfterDeleteHandler AfterDelete;

        public IGitStash Stash
        {
            get; set;
        }
        private IGitStashWrapper wrapper;
        private ITeamExplorerBase page;

        public event PropertyChangedEventHandler PropertyChanged;
        
        public StashViewModel(IGitStashWrapper wrapper, IGitStash stash, ITeamExplorerBase page, Translator T)
        {
            this.page = page;
            this.Stash = stash;
            this.wrapper = wrapper;
            this.T = T;
            PopDropDownCommand = new RelayCommand(p => OnClickPopStash(), p => AlwaysTrueCanDropDown);
            ApplyDropDownCommand = new RelayCommand(p => OnClickApplyStash(), p => AlwaysTrueCanDropDown);
            DeleteDropDownCommand = new RelayCommand(p => OnClickDropStash(), p => AlwaysTrueCanDropDown);
        }
        protected virtual void OnAfterDeleted()
        {
            if (AfterDelete != null)
            {
                AfterDelete(this, new AfterDeleteStashEventArgs());
            }
        }
        private void OnClickPopStash()
        {
            IGitStashResults results = wrapper.PopStash(new GitStashOptions(), Stash.Index);
            if (!string.IsNullOrEmpty(results.Message))
                page.ShowNotification(results.Message, NotificationType.Information);
            OnAfterDeleted();
        }
        private void OnClickApplyStash()
        {
            IGitStashResults results = wrapper.ApplyStash(new GitStashOptions(), Stash.Index);
            if (!string.IsNullOrEmpty(results.Message))
                page.ShowNotification(results.Message, NotificationType.Information);
        }
        private void OnClickDropStash()
        {
            IGitStashResults results = wrapper.DropStash(new GitStashOptions(), Stash.Index);
            if (!string.IsNullOrEmpty(results.Message))
                page.ShowNotification(results.Message, NotificationType.Information);
            OnAfterDeleted();
        }
        public RelayCommand PopDropDownCommand { get; set; }
        public RelayCommand ApplyDropDownCommand { get; set; }
        public RelayCommand DeleteDropDownCommand { get; set; }
        public bool AlwaysTrueCanDropDown { get { return true; } }

        public string DisplayString {get{ return T["stash{{{0}}}: {1}", Stash.Index, Stash.Message]; }}
    }
}
