using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitWrapper
{
    public class GitStashOptions : IGitStashApplyOptions, IGitStashDropOptions, IGitStashPopOptions, IGitStashSaveOptions
    {
        public bool All { get; set; }

        public bool Index { get; set; }

        public bool KeepIndex { get; set; }

        public string Message { get; set; }

        public IGitStash Stash { get; set; }

        public bool Untracked { get; set; }

        public bool Ignored { get; set; }
    }
}
