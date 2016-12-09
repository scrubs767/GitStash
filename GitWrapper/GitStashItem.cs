using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitWrapper
{
    public class GitStashItem : IGitStash
    {
        public GitStashItem(Stash s)
        {
            // index_match "stash@{0}" "WIP on master: 2682378 Add project files.\n"
            // "stash@{1}" "On master: Test\n\n"

            
            var groups = Regex.Match(s.CanonicalName, @"stash@{(\d+)}").Groups;

            Index = Int32.Parse(groups[1].Value);


            var msg_match = s.Message;
            msg_match = msg_match.Replace("WIP o", "O");
            groups = Regex.Match(msg_match, @"On (.*): (.*)\n").Groups;

            Branch = groups[1].Value;
            Message = groups[2].Value;
        }

        public int Index { get; internal set; }
        public string Branch { get; internal set; }
        public string Message { get; internal set; }
    }
}
