using EnvDTE;
using GitStash.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitStash
{
    public class GitStashProjects : IGitStashProjects
    {
        IServiceProvider serviceProvider;
        public GitStashProjects(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public bool IsDirty
        {
            get
            {
                var dte = (DTE)serviceProvider.GetService(typeof(DTE));
                foreach (Project proj in dte.Solution.Projects)
                {
                    if (!proj.Saved)
                        return true;
                }
                return false;
            }
        }
    }
}
