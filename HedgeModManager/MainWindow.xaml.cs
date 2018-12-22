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
using System.Windows.Threading;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string ModsDbPath = Path.Combine(App.StartDirectory, "Mods");
        public static string ConfigPath = Path.Combine(App.StartDirectory, "cpkredir.ini");
        public static bool IsCPKREDIRInstalled = false;
        public static ModsDB ModsDatabase;
        public static CPKREDIRConfig Config;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            RefreshMods();
            RefreshUI();
        }

        public void RefreshMods()
        {
            ModsList.Items.Clear();
            ModsDatabase = new ModsDB(ModsDbPath);
            ModsDatabase.DetectMods();
            ModsDatabase.GetEnabledMods();
            ModsDatabase.Mods.ForEach(mod => ModsList.Items.Add(mod));

            // Re-arrange the mods
            for (int i = 0; i < (int)ModsDatabase["Main"]["ActiveModCount", typeof(int)]; i++)
            {
                for (int i2 = 0; i2 < ModsList.Items.Count; i2++)
                {
                    ModInfo mod = (ModInfo) ModsList.Items[i2];
                    if (ModsDatabase["Main"][$"ActiveMod{i}"] == Path.GetFileName(mod.RootDirectory))
                    {
                        ModsList.Items.Remove(mod);
                        ModsList.Items.Insert(i, mod);
                    }
                }
            }
        }

        public void RefreshUI()
        {
            var steamGame = App.GetSteamGame(App.CurrentGame);
            IsCPKREDIRInstalled = App.IsCPKREDIRInstalled(App.GetSteamGame(App.CurrentGame).ExeDirectory);
            string loaders = (IsCPKREDIRInstalled ? "CPKREDIR v0.5" : "");
            bool hasOtherModLoader = File.Exists(Path.Combine(steamGame.RootDirectory, $"d3d{App.CurrentGame.DirectXVersion}.dll"));
            if (hasOtherModLoader)
            {
                if (string.IsNullOrEmpty(loaders))
                    loaders = Config.ModLoaderVersion;
                else
                    loaders += $" & {Config.ModLoaderName} v{Config.ModLoaderVersion}";
            }

            Label_GameStatus.Content = $"Game Name: {App.CurrentGame.GameName}";
            Label_MLVersion.Content = $"Loaders: {loaders}";
            Button_OtherLoader.Content = hasOtherModLoader ? $"Uninstall {Config.ModLoaderName}" : $"Install {Config.ModLoaderName}";
            Button_CPKREDIR.Content = $"{(IsCPKREDIRInstalled ? "Uninstall" : "Install")} CPKREDIR";
        }

        public void SaveModsDB()
        {
            Config.Save(ConfigPath);
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

            if(!Config.KeepOpen)
                Application.Current.Shutdown(0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var timer = new DispatcherTimer();
            //timer.Tick += dispatcherTimer_Tick;
            //timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            //timer.Start();
            
            // Config
            Config = new CPKREDIRConfig(ConfigPath);
            DataContext = Config;

            Refresh();

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

        private void UI_RemoveMod_Click(object sender, RoutedEventArgs e)
        {
            var mod = ModsList.SelectedValue as ModInfo;
            if (mod == null)
                return;

            var box = new HedgeMessageBox("WARNING", string.Format(Properties.Resources.STR_UI_DELETEMOD, mod.Title));
            
            box.AddButton("  Cancel  ", () =>
            {
                box.Close();
            });
            box.AddButton("Delete", () =>
            {
                ModsDatabase.DeleteMod(ModsList.SelectedItem as ModInfo);
                Refresh();
                box.Close();
            });
            box.ShowDialog();
            Refresh();
        }

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

        private void UI_CPKREDIR_Click(object sender, RoutedEventArgs e)
        {
            App.InstallCPKREDIR(App.GetSteamGame(App.CurrentGame).ExeDirectory, IsCPKREDIRInstalled);
            RefreshUI();
        }

        private void UI_About_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }

        // too slow
        // :^)
        // LOL
        //private void dispatcherTimer_Tick(object sender, EventArgs e)
        //{
        //    (RotateTest.RenderTransform as RotateTransform).Angle += 10d;
        //}
    }
}
