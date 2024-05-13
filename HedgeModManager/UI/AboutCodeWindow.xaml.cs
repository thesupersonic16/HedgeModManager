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
using HedgeModManager.CodeCompiler;
using Markdig.Helpers;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for AboutCodeWindow.xaml
    /// </summary>
    public partial class AboutCodeWindow : Window
    {
        public AboutCodeWindow(CSharpCode code)
        {
            DataContext = code;
            InitializeComponent();
            Title = string.Format(Lang.Localise("ModDescriptionUIAbout"), code.Name);
            TitleLbl.Text = code.Name;
            DescBx.Text = code.Description;
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
    }
}
