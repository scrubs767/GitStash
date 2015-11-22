using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitStash.Sections;

namespace GitStash.ViewModels
{
    public class RecommendedActionsViewModel : INotifyPropertyChanged
    {
        private RecommendedActionsSection recommendedActionsSection;

        public RecommendedActionsViewModel(RecommendedActionsSection recommendedActionsSection)
        {
            this.recommendedActionsSection = recommendedActionsSection;
            CanStash = true;
            CanApply = true;
            CanPop = true;
            CanDrop = true;
        }

        public bool CanStash { get; set; }
        public bool CanPop { get; set; }
        public bool CanApply { get; set; }
        public bool CanDrop { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
