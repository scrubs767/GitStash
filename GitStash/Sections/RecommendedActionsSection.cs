using GitStash.UI;
using GitStash.ViewModels;
using GitWrapper;
using Microsoft.TeamFoundation.Controls;
using SecondLanguage;
using TeamExplorer.Common;

namespace GitStash.Sections
{
    [TeamExplorerSection(GitStashPackage.RecommendedActionsSection, GitStashPackage.StashPage, 120)]
    public class RecommendedActionsSection : TeamExplorerBaseSection
    {
        private IGitStashWrapper wrapper;
        Translator T = Translator.Default;
        
        public RecommendedActionsSection()
        {
            T.RegisterTranslationsByCulture(@"po\*.po");
        }
        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            
            base.Initialize(sender, e);
            this.wrapper = GetService<IGitStashWrapper>();            
            SectionContent = new RecommendedActionsControl(new RecommendedActionsViewModel(wrapper,this));
            T.RegisterTranslationsByCulture(@"po\*.po");
            Title = T["Create Stash"];
            IsVisible = true;
        }
        public override void Refresh()
        {
            var service = GetService<ITeamExplorerPage>();
            service.Refresh();
        }
    }
}
