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

        private IGitStash stash;
        private GitStashWrapper wrapper;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnAfterDeleted()
        {
            if (AfterDelete != null)
            {
                AfterDelete(this, new AfterDeleteStashEventArgs());
            }
        }

        public StashViewModel(GitStashWrapper wrapper, IGitStash stash)
        {
            this.stash = stash;
            this.wrapper = wrapper;
            PopDropDownCommand = new RelayCommand(p => OnClickPopStash(), p => AlwaysTrueCanDropDown);
            ApplyDropDownCommand = new RelayCommand(p => OnClickApplyStash(), p => AlwaysTrueCanDropDown);
            DeleteDropDownCommand = new RelayCommand(p => OnClickDropStash(), p => AlwaysTrueCanDropDown);
        }

        private void OnClickPopStash()
        {
            wrapper.PopStash(new GitStashOptions(), stash.Index);
            OnAfterDeleted();
        }
        private void OnClickApplyStash()
        {
            wrapper.ApplyStash(new GitStashOptions(), stash.Index);
        }
        private void OnClickDropStash()
        {
            wrapper.DropStash(new GitStashOptions(), stash.Index);
            OnAfterDeleted();
        }
        public RelayCommand PopDropDownCommand { get; set; }
        public RelayCommand ApplyDropDownCommand { get; set; }
        public RelayCommand DeleteDropDownCommand { get; set; }
        public bool AlwaysTrueCanDropDown { get { return true; } }

        public string DisplayString {get{ return String.Format("stash{{{0}}}:{1}", stash.Index, stash.Message); }}
    }
}
