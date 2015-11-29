using System;

namespace GitStash.Common
{
    public delegate void StashesChangedEventHandler(object sender, EventArgs e);
    public delegate void ProjectDirectoryChangedEventHandler(object sender, ProjectDirectoryEventArgs e);

    public interface IGitStashProjectEvents
    {
        event StashesChangedEventHandler StashesChangedEvent;
        event ProjectDirectoryChangedEventHandler ProjectDirectoryChanged;
        void ChangeDirectory(string path);
        string Path { get; }
    }
}