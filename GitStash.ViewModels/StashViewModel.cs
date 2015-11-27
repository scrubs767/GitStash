using GitWrapper;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GitStash.ViewModels
{
    public class StashViewModel : INotifyPropertyChanged
    {
        public delegate void AfterDeleteHandler(object source, AfterDeleteStashEventArgs e);
        public event AfterDeleteHandler AfterDelete;

        public IGitStash Stash
        {
            get; set;
        }
        private IGitStashWrapper wrapper;
        public event PropertyChangedEventHandler PropertyChanged;
        
        public StashViewModel(IGitStashWrapper wrapper, IGitStash stash)
        {
            this.Stash = stash;
            this.wrapper = wrapper;
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
            wrapper.PopStash(new GitStashOptions(), Stash.Index);
            OnAfterDeleted();
        }
        private void OnClickApplyStash()
        {
            wrapper.ApplyStash(new GitStashOptions(), Stash.Index);
        }
        private void OnClickDropStash()
        {
            wrapper.DropStash(new GitStashOptions(), Stash.Index);
            OnAfterDeleted();
        }
        public RelayCommand PopDropDownCommand { get; set; }
        public RelayCommand ApplyDropDownCommand { get; set; }
        public RelayCommand DeleteDropDownCommand { get; set; }
        public bool AlwaysTrueCanDropDown { get { return true; } }

        public string DisplayString {get{ return String.Format("stash{{{0}}}: {1}", Stash.Index, Stash.Message); }}
    }
}
