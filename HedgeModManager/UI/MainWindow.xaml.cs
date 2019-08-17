using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.IO;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool IsCPKREDIRInstalled = false;
        public static ModsDB ModsDatabase;


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
            ModsDatabase = new ModsDB(App.ModsDbPath);
            ModsDatabase.DetectMods();
            ModsDatabase.GetEnabledMods();
            ModsDatabase.Mods.ForEach(mod => ModsList.Items.Add(mod));

            // Re-arrange the mods
            for (int i = (int)ModsDatabase["Main"]["ActiveModCount", typeof(int)]; i >= 0; --i)
            {
                for (int i2 = 0; i2 < ModsList.Items.Count; i2++)
                {
                    var mod = ModsList.Items[i2] as ModInfo;
                    if (ModsDatabase["Main"][$"ActiveMod{i}"] == System.IO.Path.GetFileName(mod.RootDirectory))
                    {
                        ModsList.Items.Remove(mod);
                        ModsList.Items.Insert(0, mod);
                    }
                }
            }
        }

        public void RefreshUI()
        {
            // Sets the DataContext for all the Components 
            DataContext = new
            {
                CPKREDIR = App.Config,
                ModsDB = ModsDatabase
            };

            Title = $"{App.ProgramName} ({App.VersionString}) - {App.CurrentGame.GameName}";

            var steamGame = App.GetSteamGame(App.CurrentGame);
            var exeDir = steamGame?.ExeDirectory ?? System.IO.Path.Combine(Directory.GetCurrentDirectory(), App.CurrentGame.ExecuteableName);
            bool hasOtherModLoader = File.Exists(System.IO.Path.Combine(steamGame?.RootDirectory ?? exeDir, $"d3d{App.CurrentGame.DirectXVersion}.dll"));
            IsCPKREDIRInstalled = App.CurrentGame.SupportsCPKREDIR ? App.IsCPKREDIRInstalled(exeDir) : hasOtherModLoader;
            string loaders = (IsCPKREDIRInstalled && App.CurrentGame.SupportsCPKREDIR ? App.CPKREDIRVersion : "");
            if (hasOtherModLoader)
            {
                if (string.IsNullOrEmpty(loaders))
                    loaders = $"{App.CurrentGame.CustomLoaderName}";
                else
                    loaders += $" & {App.CurrentGame.CustomLoaderName}";
            }

            if (string.IsNullOrEmpty(loaders))
                loaders = "None";

            Label_GameStatus.Content = $"Game Name: {App.CurrentGame.GameName}";
            Label_MLVersion.Content = $"Loaders: {loaders}";
            Button_OtherLoader.Content = hasOtherModLoader && App.CurrentGame.SupportsCPKREDIR ? $"Uninstall Code Loader" : $"Install Code Loader";
            Button_CPKREDIR.Content = $"{(IsCPKREDIRInstalled ? "Uninstall" : "Install")} Mod Loader";
        }

        public void SaveModsDB()
        {
            App.Config.Save(App.ConfigPath);
            ModsDatabase.Mods.Clear();
            foreach (var mod in ModsList.Items)
            {
                ModsDatabase.Mods.Add(mod as ModInfo);
            }
            ModsDatabase.SaveDB();
        }

        public void StartGame()
        {
            App.GetSteamGame(App.CurrentGame).StartGame();

            if (!App.Config.KeepOpen)
                Application.Current.Shutdown(0);
        }

        public void SetupRotation()
        {
            RotateTest.Width = Width;
            RotateTest.Height = Height;

            double oldWidth = Width;
            double oldHeight = Height;
            double newWidth = oldWidth * Math.Sin(Math.PI / 2 - Math.PI / 4) + oldHeight * Math.Sin(Math.PI / 4);
            double newHeight = oldHeight * Math.Sin(Math.PI / 2 - Math.PI / 4) + oldWidth * Math.Sin(Math.PI / 4);

            Left -= (newWidth - Width) / 2d;
            Top -= (newHeight - Height) / 2d;
            Width = newWidth;
            Height = newHeight;

            var timer = new DispatcherTimer();
            timer.Tick += dispatcherTimer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();
            //(RotateTest.RenderTransform as RotateTransform).Angle += Math.PI * 57.2958;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(App.CurrentGame.HasCustomLoader && !App.CurrentGame.SupportsCPKREDIR)
            {
                Button_CPKREDIR.IsEnabled = true;
                Button_OtherLoader.IsEnabled = false;
            }
            else
            {
                Button_CPKREDIR.IsEnabled = App.CurrentGame.SupportsCPKREDIR;
                Button_OtherLoader.IsEnabled = App.CurrentGame.HasCustomLoader;
            }
            //if ((DateTime.Now.Month == 4 && DateTime.Now.Day == 1) || !Steam.CheckDirectory(App.StartDirectory))
                //SetupRotation();

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

        private void UI_ModsList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                // Try Install mods from all files
                files.ToList().ForEach(t => ModsDatabase.InstallMod(t));
                Refresh();
            }
        }

        private void UI_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (ListViewItem)sender;
            var dialog = new AboutModWindow((ModInfo)item.Content);
            dialog.ShowDialog();
        }

        private void UI_OtherLoader_Click(object sender, RoutedEventArgs e)
        {
            App.InstallOtherLoader(true);
            RefreshUI();
        }

        private void UI_Edit_Mod(object sender, RoutedEventArgs e)
        {
            var mod = (ModInfo)ModsList.SelectedItem;
            var window = new EditModWindow(mod);
            if (window.ShowDialog().Value == true)
            {
                mod.Save();
            }
            RefreshMods();
        }

        private void UI_Description_Click(object sender, RoutedEventArgs e)
        {
            UI_MouseDoubleClick(null, null);
        }

        private void UI_Open_Folder(object sender, RoutedEventArgs e)
        {
            var mod = (ModInfo)ModsList.SelectedItem;
            Process.Start(mod.RootDirectory);
        }

        private void UI_Install_Mod(object sender, RoutedEventArgs e)
        {
            var choice = new OptionsWindow("Install a mod by...", "Installing from a folder", "Installing from an archive", "Making one (for developers!)").Ask();
            if (choice == 0)
            {
                var dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog())
                {
                    ModsDatabase.InstallMod(dialog.SelectedFolder);
                }
            }
            else if(choice == 1)
            {
                var dialog = new FileOpenDialog();
                var filters = new COMDLG_FILTERSPEC[3];
                filters[0] = new COMDLG_FILTERSPEC() { pszName = "Zip archive", pszSpec = "*.zip" };
                filters[1] = new COMDLG_FILTERSPEC() { pszName = "7z archive", pszSpec = "*.7z" };
                filters[2] = new COMDLG_FILTERSPEC() { pszName = "Rar archive", pszSpec = "*.rar" };
                dialog.SetFileTypes((uint)filters.Length, filters);
                if(dialog.Show(new WindowInteropHelper(this).Handle) == 0)
                {
                    dialog.GetResult(out var item);
                    item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out var path);
                    ModsDatabase.InstallMod(path);
                }
            }
            else if(choice == 2)
            {
                var mod = new ModInfo
                {
                    Title = GenerateModTitle(),
                    Author = Environment.UserName
                };
                mod.IncludeDirs.Add(".");
                var editor = new EditModWindow(mod);
                if (editor.ShowDialog().Value)
                {
                    ModsDatabase.CreateMod(mod, true);
                    RefreshMods();
                }
            }
        }

        protected string GenerateModTitle()
        {
            for(int i = 1; i < int.MaxValue; i++)
            {
                var title = $"{App.CurrentGame.GameName} Mod {i}";
                if (!Directory.Exists(Path.Combine(ModsDatabase.RootDirectory, title)))
                    return title;
            }
            return string.Empty;
        }

        // too slow
        // :^)
        // LOL
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            (RotateTest.RenderTransform as RotateTransform).Angle += 0.001d;
        }
    }
}
