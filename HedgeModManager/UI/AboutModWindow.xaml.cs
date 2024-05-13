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
using Markdig.Helpers;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for AboutModWindow.xaml
    /// </summary>
    public partial class AboutModWindow : Window
    {
        public AboutModWindow(ModInfo mod)
        {
            DataContext = mod;
            InitializeComponent();
            Title = string.Format(Lang.Localise("ModDescriptionUIAbout"), mod.Title);
            TitleLbl.Text = mod.Title;
            if (!string.IsNullOrEmpty(mod.Version))
            {
                if (!mod.Version.ToLower()[0].IsDigit())
                    TitleLbl.Text += $" {mod.Version}";
                else
                    TitleLbl.Text += $" v{mod.Version}";
            }
            DescBx.Text = mod.Description;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        private void AboutModWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            var uri = ((Hyperlink)sender).NavigateUri.ToString();

            if (!string.IsNullOrEmpty(uri))
                Process.Start(uri);
        }
    }
}
