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
using System.Windows.Shapes;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for AboutModWindow.xaml
    /// </summary>
    public partial class AboutModWindow : Window
    {
        public AboutModWindow(ModInfo mod)
        {
            InitializeComponent();
            Title = $"About {mod.Title}";
            TitleLbl.Content = $"{mod.Title} v{mod.Version}";
            AuthorLbl.Content = $"Made by {mod.Author} on {mod.Date}";
            DescBx.Text = mod.Description;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            InvalidateVisual();
        }
    }
}
