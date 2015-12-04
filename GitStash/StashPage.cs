using System;

using Microsoft.TeamFoundation.Controls;
using TeamExplorer.Common;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using GitStash.UI;
using System.Windows.Threading;
using GitStash.Common;
using GitWrapper;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using GitStash.ViewModels;
using SecondLanguage;

namespace GitStash
{


    [TeamExplorerPage(GitStashPackage.StashPage, Undockable = true)]
    public class StashPage : TeamExplorerBasePage, INavigateable
    {
        private static ITeamExplorer teamExplorer;
        Translator T;
        private IGitStashWrapper gitWrapper;
        private IGitExt gitService;

        [ImportingConstructor]
        public StashPage([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            
            teamExplorer = (ITeamExplorer)serviceProvider.GetService(typeof(ITeamExplorer));
            gitService = (IGitExt)serviceProvider.GetService(typeof(IGitExt));                        
           
        }

        public override void Initialize(object sender, PageInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            gitWrapper = GetService<IGitStashWrapper>();
            T = GetService<IGitStashTranslator>().Translator;
            Title = T["Git Stash"];
            PageContent = new PageControl(new PageViewModel(this, gitWrapper, T));            
        }

        public override object GetExtensibilityService(Type serviceType)
        {
            if (serviceType == typeof(IGitStashWrapper))
                return gitWrapper;
            return base.GetExtensibilityService(serviceType);
        }
        public void ShowPage(string page)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() =>
                    teamExplorer.NavigateToPage(new Guid(page), null)));

        }
    }
}

