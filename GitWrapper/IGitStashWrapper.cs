using System.Collections.Generic;
using System.ComponentModel;

namespace GitWrapper
{
    public interface IGitStashWrapper
    {
        IList<IGitStash> Stashes { get; }

        event PropertyChangedEventHandler PropertyChanged;

        IGitStashResults ApplyStash(IGitStashApplyOptions options, int index);
        IGitStashResults DropStash(IGitStashDropOptions options, int index);
        IList<string> GetUntrackedChangesList(int stashIndex);
        IGitStashResults PopStash(IGitStashPopOptions options, int index);
        IGitStashResults SaveStash(IGitStashSaveOptions options);
        bool WorkingDirHasChanges();
        event GitStashWrapper.StashesChangedEventHandler StashesChangedEvent;
    }
}