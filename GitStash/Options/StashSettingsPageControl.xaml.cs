using Microsoft.Win32;
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

namespace GitStash.Options
{
    /// <summary>
    /// Interaction logic for StashSettingsPageControl.xaml
    /// </summary>
    public partial class StashSettingsPageControl : UserControl
    {
        private StashOptionsPage stashOptionsPage;

        public StashSettingsPageControl(StashOptionsPage stashOptionsPage)
        {
            this.stashOptionsPage = stashOptionsPage;
            InitializeComponent();
            DataContext = stashOptionsPage;
        }

        private void btnBrowseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                stashOptionsPage.GitLocation = openFileDialog.FileName;
        }
    }
}
