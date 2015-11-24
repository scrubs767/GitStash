using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using System.Windows;


namespace GitStash.Options
{
    [Guid(GitStashPackage.StashOptionsPage)]
    public class StashOptionsPage : UIElementDialogPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public StashOptionsPage()
        {
            GitLocation = "";
        }

        private string gitLocation = "";
        public string GitLocation { get { return gitLocation; } set { gitLocation = value; NotifyPropertyChanged("GitLocation"); } }

        protected override UIElement Child
        {
            get
            {
                return new StashSettingsPageControl(this);
            }
        }
    }
}
