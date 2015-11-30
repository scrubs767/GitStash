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

        public GitStashResults(bool sucess, string Message)
        {
           
            this.Success = sucess;
            this.Message = Message;
        }

        public GitStashResults(Stash stash, string Message)
        {
            Stash = stash;
            if (stash != null)
                Success = true;
            else
                Success = false;
            this.Message = Message;
        }

        public GitStashResults(StashApplyStatus status, string Message)
        {
            if (status.HasFlag(StashApplyStatus.NotFound) ||
                status.HasFlag(StashApplyStatus.UncommittedChanges) ||
                status.HasFlag(StashApplyStatus.Conflicts))
                Success = false;
            else
                Success = true;
            this.Message = Message;
        }

        internal Stash Stash { get; set; }
        public bool Success { get; internal set; }
        public string Message { get; internal set; }
    }
}
