using System;
using System.ComponentModel;

using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using TeamExplorer.Common;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using GitStash.UI;
using System.Windows.Threading;
using GitStash.Properties;

namespace GitStash
{
    

    [TeamExplorerPage(GitStashPackage.StashPage, Undockable = true)]
    public class StashPage : TeamExplorerBasePage
    {
        private static ITeamExplorer teamExplorer;
        private static IVsOutputWindowPane outputWindow;
        private StashPageControl ui;
        private IServiceProvider serviceProvider;
        //public VsGitFlowWrapper GitWrapper { get; set; }

        [ImportingConstructor]
        public StashPage([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            //GitWrapper = new VsGitFlowWrapper(@"C:\Users\Stephen\Documents\GitHub\test", serviceProvider.GetService(typeof(SVsGeneralOutputWindowPane)) as IVsOutputWindowPane);
            Title = "GitStash";
            //gitService = (IGitExt)serviceProvider.GetService(typeof(IGitExt));
            teamExplorer = (ITeamExplorer)serviceProvider.GetService(typeof(ITeamExplorer));
            //gitService.PropertyChanged += OnGitServicePropertyChanged;

            var outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            var customGuid = new Guid("D9B93453-B887-407F-99EC-66C6FD5CA84C");
            outWindow.CreatePane(ref customGuid, "GitStash", 1, 1);
            outWindow.GetPane(ref customGuid, out outputWindow);
            ui = new StashPageControl(serviceProvider);
            PageContent = ui;
        }

        public static void ShowPage(string page)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() =>
                    teamExplorer.NavigateToPage(new Guid(page), null)));

        }
    }
}

