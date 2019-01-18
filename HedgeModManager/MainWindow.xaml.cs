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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool IsCPKREDIRInstalled = false;
        public static ModsDB ModsDatabase;
        public static Rectangle ResizeRect = null;


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

            TitleLabel.Content = $"{App.ProgramName} ({App.VersionString}) - {App.CurrentGame.GameName}";

            var steamGame = App.GetSteamGame(App.CurrentGame);
            IsCPKREDIRInstalled = App.IsCPKREDIRInstalled(App.GetSteamGame(App.CurrentGame).ExeDirectory);
            string loaders = (IsCPKREDIRInstalled ? "CPKREDIR v0.5" : "");
            bool hasOtherModLoader = File.Exists(System.IO.Path.Combine(steamGame.RootDirectory, $"d3d{App.CurrentGame.DirectXVersion}.dll"));
            if (hasOtherModLoader)
            {
                if (string.IsNullOrEmpty(loaders))
                    loaders = $"{App.Config.ModLoaderNameWithVersion}";
                else
                    loaders += $" & {App.Config.ModLoaderNameWithVersion}";
            }

            if (string.IsNullOrEmpty(loaders))
                loaders = "None";

            Label_GameStatus.Content = $"Game Name: {App.CurrentGame.GameName}";
            Label_MLVersion.Content = $"Loaders: {loaders}";
            Button_OtherLoader.Content = hasOtherModLoader ? $"Uninstall {App.Config.ModLoaderName}" : $"Install {App.Config.ModLoaderName}";
            Button_CPKREDIR.Content = $"{(IsCPKREDIRInstalled ? "Uninstall" : "Install")} CPKREDIR";
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

            if(!App.Config.KeepOpen)
                Application.Current.Shutdown(0);
        }

        public void SetupRotation()
        {
            RotateTest.Width = Width;
            RotateTest.Height = Height;

            double oldWidth = Width;
            double oldHeight = Height;
            double newWidth = oldWidth   * Math.Sin(Math.PI / 2 - Math.PI / 4) + oldHeight * Math.Sin(Math.PI / 4);
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

            if (App.CurrentGame.HasCustomLoader)
                Button_OtherLoader.IsEnabled = true;
            if (App.CurrentGame.SupportsCPKREDIR)
                Button_CPKREDIR.IsEnabled = true;

            
            if ((DateTime.Now.Month == 4 && DateTime.Now.Day == 1) || !Steam.CheckDirectory(App.StartDirectory))
                SetupRotation();

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

        private void UI_OtherLoader_Click(object sender, RoutedEventArgs e)
        {
            App.InstallOtherLoader(true);
            RefreshUI();
        }

        // too slow
        // :^)
        // LOL
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
           (RotateTest.RenderTransform as RotateTransform).Angle += 0.001d;
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

        private void UI_Grip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle senderRect)
            {
                ResizeRect = senderRect;
                senderRect.CaptureMouse();
            }
        }

        // TODO: Fix Slow Resizing
        private void UI_Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (ResizeRect != null)
            {
                var position = e.GetPosition(this);
                CaptureMouse();
                if (ResizeRect.Name.ToLower().Contains("right"))
                {
                    position.X += 5;
                    if (position.X > 0)
                        Width = position.X;
                }
                if (ResizeRect.Name.ToLower().Contains("left"))
                {
                    position.X -= 5;
                    Left += position.X;
                    position.X = Width - position.X;
                    if (position.X > 0)
                        Width = position.X;
                }
                if (ResizeRect.Name.ToLower().Contains("bottom"))
                {
                    position.Y += 5;
                    if (position.Y > 0)
                        Height = position.Y;
                }
                if (ResizeRect.Name.ToLower().Contains("top"))
                {
                    position.Y -= 5;
                    Top += position.Y;
                    position.Y = Height - position.Y;
                    if (position.Y > 0)
                        Height = position.Y;
                }
            }
        }

        private void UI_Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ResizeRect != null)
            {
                ResizeRect = null;
                ReleaseMouseCapture();
            }
        }
    }
}
