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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IOPath = System.IO.Path;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for InstallWindow.xaml
    /// </summary>
    public partial class InstallWindow : Window
    {
        public InstallWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Copy all the ModLoader files into a custom directory
        /// and creates a shortcut, After that it restarts the ModLoader in that custom directory
        /// </summary>
        /// <param name="path">Path to an Executable</param>
        /// <param name="gameName">The Name of the game</param>
        /// <returns></returns>
        public bool InstallModLoader(string path, string gameName)
        {
            path = path.Replace('/', '\\');
            // HedgeModManager.exe, HedgeModManager.pdb, cpkredir.dll, cpkredir.txt
            string[] files = new string[]
            {
                App.ProgramName + ".exe",
                App.ProgramName + ".pdb",
                "cpkredir.dll",
                "cpkredir.txt"
            };

            foreach (string file in files)
            {
                string filePath = IOPath.Combine(App.StartDirectory, file);
                if (File.Exists(filePath))
                {
                    // Copies the current file into the custom filepath
                    File.Copy(filePath, IOPath.Combine(path, file), true);

                    // Don't delete this file as its in use
                    if (file == App.ProgramName + ".exe")
                        continue;
                    if (file == App.ProgramName + ".pdb")
                        continue;

                    // Tries to delete the old files
                    try { File.Delete(filePath); } catch
                    {
                        Console.WriteLine(filePath);
                    }
                }
            }

            try
            {
                // Creates a shortcut to the ModLoader
                string shortcutPath = IOPath.Combine(App.StartDirectory, $"HedgeModManager - {gameName}.lnk");
                var wsh = new IWshRuntimeLibrary.WshShell();
                var shortcut = wsh.CreateShortcut(shortcutPath) as IWshRuntimeLibrary.IWshShortcut;
                shortcut.Description = $"HedgeModManager - {gameName}";
                shortcut.TargetPath = IOPath.Combine(path, App.ProgramName + ".exe");
                shortcut.WorkingDirectory = path;
                shortcut.Save();
            }
            catch { }

            // Starts the ModLoader
            var startInfo = new ProcessStartInfo()
            {
                FileName = IOPath.Combine(path, App.ProgramName + ".exe"),
                Verb = "runas" // Run as Admin
            };
            Process.Start(startInfo);

            try
            {
                // Tries to delete the old HedgeModManager.exe
                startInfo = new ProcessStartInfo()
                {
                    Arguments = $"/c powershell start-sleep 2 & del \"{App.ProgramName + ".exe"}\" & del \"{App.ProgramName + ".pdb"}\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "cmd.exe"
                };
                Process.Start(startInfo);
            }
            catch { }
            return true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App.SteamGames = Steam.SearchForGames();
            App.SteamGames.Where(t => t.Status).ToList().ForEach(t => UI_GameList.Items.Add(t));
        }

        private void UI_GameItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var game = ((ListViewItem)sender).Content as SteamGame;
            InstallModLoader(game.RootDirectory, game.GameName);
            Close();
        }

        private void UI_FrameTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void UI_FrameM_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Grid)sender).Background = new SolidColorBrush(Color.FromRgb(0x00, 0x7A, 0xCC));
        }

        private void UI_FrameM_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void UI_FrameC_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Grid)sender).Background = new SolidColorBrush(Color.FromRgb(0x00, 0x7A, 0xCC));
        }

        private void UI_FrameC_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

    }
}
