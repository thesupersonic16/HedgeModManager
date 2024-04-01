using HedgeModManager.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HedgeModManager
{
    public static class Events
    {
        public static bool CheckDate(int month, int day)
        {
            if (RegistryConfig.AllowEvents &&
                (month == 0 || month == DateTime.Now.Month) &&
                (day == 0 || day == DateTime.Now.Day))
                return true;

            return false;
        }

        public static void OnStartUp()
        {
            // 16th of March
            if (CheckDate(3, 16))
            {
                // Display Iizuka's Birthday Splash
                SplashScreen splashScreen;

                splashScreen = new("Resources/Graphics/EasterEggs/splashBirthday.png");
                splashScreen.Show(false, true);
                splashScreen.Close(TimeSpan.FromSeconds(0.5));
            }

            // 1st of April
            if (CheckDate(4, 1))
            {
                // Apply icon to HedgeWindow
                var gmiIcon = new BitmapImage(HedgeApp.GetResourceUri("Resources/Graphics/EasterEggs/icon128SonicGMI.png"));
                Application.Current.Resources["AppIcon"] = gmiIcon;
            }
        }

        public static void OnWindowLoaded(Window window)
        {
            // 1st of April
            if (CheckDate(4, 1))
            {
                // Apply window icon
                window.Icon = Application.Current.Resources["AppIcon"] as ImageSource;
            }
        }

        public static void OnMainUIRefresh(MainWindow mainWindow)
        {
            // 1st of April
            if (CheckDate(4, 1))
            {
                string title = $"SonicGMI v{HedgeApp.VersionString} (Sonic Generations Mod Installer)";
                if (mainWindow.SelectedModProfile != null)
                    title += $" - {mainWindow.SelectedModProfile?.Name}";
                if (HedgeApp.IsLinux)
                    title += " (Linux)";
                mainWindow.Title = title;
            }
        }

    }
}
