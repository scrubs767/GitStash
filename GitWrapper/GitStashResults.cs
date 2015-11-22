using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitWrapper
{
    public class GitStashResults : IGitStashResults
    {
        public GitStashResults(Stash stash)
        {
            Stash = stash;
            if (stash != null)
                Success = true;
            else
                Success = false;
        }

        public GitStashResults(StashApplyStatus status)
        {
            if (status.HasFlag(StashApplyStatus.NotFound) ||
                status.HasFlag(StashApplyStatus.UncommittedChanges) ||
                status.HasFlag(StashApplyStatus.Conflicts))
                Success = false;
            else
                Success = true;
        }

        internal Stash Stash { get; set; }
        public bool Success { get; internal set; }
    }
}
