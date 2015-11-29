using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitStash.Common
{
    public class ProjectDirectoryEventArgs : EventArgs
    {
        public ProjectDirectoryEventArgs(string path)
        {
            Path = path;
        }
        public string Path { get; private set; }
        
    }
}
