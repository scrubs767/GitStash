using System;
using System.ComponentModel;
using System.IO;
using GitStash.Common;

namespace GitStash
{    
    public class GitStashFileWatcher : IGitStashProjectEvents
    {
        public event StashesChangedEventHandler StashesChangedEvent;
        public event ProjectDirectoryChangedEventHandler ProjectDirectoryChanged;
        FileSystemWatcher watcher;

        public void ChangeDirectory(string path)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException("invalid path");
            this.Path = path;

            if (watcher != null)
                watcher.Dispose();

            watcher = new FileSystemWatcher();            
            watcher.Path = Path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName
                                   | NotifyFilters.CreationTime | NotifyFilters.Attributes
                                   | NotifyFilters.Security | NotifyFilters.Size;
            watcher.Filter = "*.*";
            watcher.Changed += new FileSystemEventHandler(OnEvent);
            watcher.Created += new FileSystemEventHandler(OnEvent);
            watcher.Deleted += new FileSystemEventHandler(OnEvent);
            watcher.Error += new ErrorEventHandler(OnEvent);
            watcher.Renamed += new RenamedEventHandler(OnEvent);
            watcher.Disposed += Watcher_Disposed;
            watcher.EnableRaisingEvents = true;
            OnDirectoryChanged(path);
        }

        public string Path { get; private set; }

        private void OnDirectoryChanged(string path)
        {
            ProjectDirectoryChanged?.Invoke(this, new ProjectDirectoryEventArgs(path));
        }
        private void OnFilesChanged(PropertyChangedEventArgs e)
        {
            StashesChangedEvent?.Invoke(this, e);
        }

        private void Watcher_Disposed(object sender, EventArgs e)
        {
        }

        private void OnEvent(object sender, EventArgs e)
        {
            OnFilesChanged(new PropertyChangedEventArgs(""));
        }
    }
}
