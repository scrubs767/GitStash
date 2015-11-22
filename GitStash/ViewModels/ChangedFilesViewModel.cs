using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitStash.Sections;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using GitWrapper;

namespace GitStash.ViewModels
{
    public class ChangedFilesViewModel : INotifyPropertyChanged
    {
        private GitStashWrapper wrapper;
        private string basePath;
        public ChangedFilesViewModel(IGitExt service, string basePath)
        {
            this.basePath = basePath;
            this.wrapper = new GitStashWrapper(service.ActiveRepositories.FirstOrDefault().RepositoryPath);
        }

        IList<string> UntrackedFiles
        {
            get
            {
                //return wrapper.GetUntrackedChangesList(0);
                IList<string> ret = new List<string>();
                ret.Add(@"c:\Users\Stephen\Documents\Visual Studio 2015\Projects\Visual Studio\GitStash\GitStash\Sections\Foo.txt");
                ret.Add(@"c:\Users\Stephen\Documents\Visual Studio 2015\Projects\Visual Studio\GitStash\GitStash\Sections\Bar.txt");
                ret.Add(@"c:\Users\Stephen\Documents\Visual Studio 2015\Projects\Visual Studio\GitStash\GitWrapper\Baz.txt");
                ret.Add(@"c:\Users\Stephen\Documents\Visual Studio 2015\Projects\Visual Studio\GitStash\GitWrapper\Foo\Bing.txt");
                return ret;
            }
        }

        private void Reload()
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
