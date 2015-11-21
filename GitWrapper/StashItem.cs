using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitWrapper
{
    public class StashItem
    {
        public int Index { get; set; }
        public Stash Stash { get; set; }
        public string Branch { get; set; }
        public string Message { get; set; }
        public string ToolTip { get { return String.Format("{0} {1} {2}",Index,Branch,Message); } }
    }
}
