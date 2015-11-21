using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using GitStash.Properties;
using TeamExplorer.Common;

namespace GitStash
{
    [TeamExplorerNavigationItem(GitStashPackage.StashNavigationItem, 1500)]
    public class StashNavigationItem : TeamExplorerBaseNavigationItem
    {
        [ImportingConstructor]
        public StashNavigationItem([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.Image = Resources.StashIcon;
            this.IsVisible = true;
            this.Text = "GitStash";
        }

        public override void Execute()
        {
            var service = this.GetService<ITeamExplorer>();
            if (service == null)
            {
                return;
            }
            service.NavigateToPage(new Guid(GitStashPackage.StashPage), null);
        }

    }
}
