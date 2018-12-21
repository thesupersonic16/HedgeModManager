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
        public static string ModsDbPath = Path.Combine(App.StartDirectory, "Mods");
        public static ModsDB ModsDatabase = new ModsDB(ModsDbPath);

        public MainWindow()
        {
            InitializeComponent();
            UI_Refresh_Click(null, null);
        }

        public void Refresh()
        {
            RefeshMods();
        }

        public void RefeshMods()
        {
            ModsList.Items.Clear();
            ModsDatabase.Mods.ForEach(mod => ModsList.Items.Add(mod));

            // Re-arrange the mods
            var mods = ModsDatabase.Mods;
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].Enabled)
                {
                    for (int i2 = 0; i2 < (int)ModsDatabase["Main"]["ActiveModCount", typeof(int)]; i2++)
                    {
                        if (ModsDatabase["Main"][$"ActiveMod{i2}"] == Path.GetFileName(mods[i].RootDirectory))
                        {
                            ModsList.Items.Remove(mods[i]);
                            ModsList.Items.Insert(i2, mods[i]);
                        }
                    }
                }
            }
        }

        public void SaveModsDB()
        {
            ModsDatabase.Mods.Clear();
            foreach (var mod in ModsList.Items)
            {
                ModsDatabase.Mods.Add((ModInfo)mod);
            }
            ModsDatabase.SaveDB();
        }

        public void StartGame()
        {
            App.GetSteamGame(App.CurrentGame).StartGame();
        }

        private void UI_MoveMod_Click(object sender, RoutedEventArgs e)
        {
            var index = Math.Max(0, ModsList.SelectedIndex);
            var mod = ModsList.Items[index];
            if (sender.Equals(UpBtn))
            {
                ModsList.Items.RemoveAt(index);
                ModsList.Items.Insert(Math.Max(0, --index), mod);
            }
            else if (sender.Equals(TopBtn))
            {
                ModsList.Items.RemoveAt(index);
                ModsList.Items.Insert(0, mod);
            }
            else if (sender.Equals(DownBtn))
            {
                ModsList.Items.RemoveAt(index);
                ModsList.Items.Insert(++index, mod);
            }
            else if (sender.Equals(BottomBtn))
            {
                ModsList.Items.RemoveAt(index);
                ModsList.Items.Insert(ModsList.Items.Count, mod);
            }
            ModsList.SelectedIndex = index;
        }

        // TODO: RemoveMod

        private void UI_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        // TODO: AddMod

        private void UI_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveModsDB();
            Refresh();
        }

        private void UI_SaveAndPlay_Click(object sender, RoutedEventArgs e)
        {
            SaveModsDB();
            Refresh();
            StartGame();
        }

        private void UI_Play_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }
    }
}
