using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using static HedgeModManager.Lang;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void UI_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UI_GitHub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Properties.Resources.URL_HMM_GITHUB);
        }
    }
}
