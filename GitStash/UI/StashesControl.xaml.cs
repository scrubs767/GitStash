using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GitStash.ViewModels;

namespace GitStash.UI
{
    /// <summary>
    /// Interaction logic for StashesControl.xaml
    /// </summary>
    public partial class StashesControl : UserControl
    {
        public StashesControl()
        {
            InitializeComponent();
        }

        public StashesControl(StashesViewModel stashesViewModel)
        {
            InitializeComponent();
            DataContext = stashesViewModel;
            
        }
    }
}
