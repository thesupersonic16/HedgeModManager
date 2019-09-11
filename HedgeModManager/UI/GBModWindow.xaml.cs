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
using GameBananaAPI;

namespace HedgeModManager.UI
{
    /// <summary>
    /// Interaction logic for GBModWindow.xaml
    /// </summary>
    public partial class GBModWindow : Window
    {
        public GBModWindow(GBAPIItemDataBasic mod)
        {
            DataContext = mod;
            InitializeComponent();
            ScreenshotBx.Source = new BitmapImage(new Uri(mod.ScreenshotURL));
            ScreenshotBx.Stretch = Stretch.Uniform;
            Description.NavigateToString($@"
<html>
    <body>
        <style>
            {Properties.Resources.GBStyleSheet}
        </style>
        {mod.Body}
    </body>
</html>");
            foreach(var group in mod.Credits)
            {
                CreditsPanel.Children.Add(new TextBlock() { Text = group.GroupName, FontSize = 14, TextWrapping = TextWrapping.WrapWithOverflow });
                foreach(var credit in group.Credits)
                {
                    var block = new TextBlock()
                    {
                        Text = credit.MemberID == 0 ? credit.MemberName : string.Empty,
                        FontSize = 16,
                        TextWrapping = TextWrapping.WrapWithOverflow
                    };
                    if(credit.MemberID != 0)
                    {
                        var link = new Hyperlink();
                        var run = new Run();
                        link.NavigateUri = new Uri($"https://gamebanana.com/members/{credit.MemberID}");
                        run.Text = credit.MemberName;
                        link.Click += (x, y) => { System.Diagnostics.Process.Start(link.NavigateUri.ToString()); };
                        link.Inlines.Add(run);
                        block.Inlines.Add(link);
                    }
                    CreditsPanel.Children.Add(block);
                    CreditsPanel.Children.Add(new TextBlock() { Text = credit.Role, FontSize = 12, TextWrapping = TextWrapping.WrapWithOverflow });
                }
            }
        }
    }
}