using System;
using System.Collections.Generic;
using System.IO;
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

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
#if !DEBUG
        public static string ModsDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods");
#endif
#if DEBUG
        public static string ModsDbPath = Path.Combine(@"\\Ali\D\Games\Sonic Lost World", "Mods");
#endif
        public static ModsDB ModsDatabase = new ModsDB(ModsDbPath);
        public MainWindow()
        {
            InitializeComponent();
            RefreshClick(null, null);
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            ModsList.Items.Clear();
            ModsDatabase.Mods.ForEach(mod => ModsList.Items.Add(mod));
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            ModsDatabase.SaveDB();
        }
    }
}
