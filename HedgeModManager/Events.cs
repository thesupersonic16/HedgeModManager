using HedgeModManager.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static HedgeModManager.Lang;

namespace HedgeModManager
{
    public static class Events
    {

        public static bool IsAprilFools = false;
        public static ResourceDictionary EventStrings = new ResourceDictionary();

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
            // 1st of April
            IsAprilFools = CheckDate(4, 1);

            // 16th of March
            if (CheckDate(3, 16))
            {
                // Display Iizuka's Birthday Splash
                SplashScreen splashScreen;

                splashScreen = new("Resources/Graphics/EasterEggs/splashBirthday.png");
                splashScreen.Show(false, true);
                splashScreen.Close(TimeSpan.FromSeconds(0.5));
            }

            if (IsAprilFools)
            {
                // Apply icon to HedgeWindow
                var gmiIcon = new BitmapImage(HedgeApp.GetResourceUri("Resources/Graphics/EasterEggs/icon128SonicGMI.png"));
                Application.Current.Resources["AppIcon"] = gmiIcon;
                HedgeApp.ProgramName = "Sonic Generations Mod Installer";
            }
        }

        public static void OnWindowLoaded(Window window)
        {
            // 1st of April
            if (IsAprilFools)
            {
                // Apply window icon
                window.Icon = Application.Current.Resources["AppIcon"] as ImageSource;
                window.Title = Localise(window.Title);

                var creditNames = window.FindName("CreditNames") as StackPanel;
                var creditRoles = window.FindName("CreditRoles") as StackPanel;
                if (creditNames != null && creditRoles != null)
                {
                    creditNames.Children.Insert(0, new Label() { Content = "Darío" });
                    creditRoles.Children.Insert(0, new Label() { Content = "Project lead" });
                }
            }
        }

        public static void OnMainUIRefresh(MainWindow mainWindow)
        {
            if (IsAprilFools)
            {
                string title = $"SonicGMI v{HedgeApp.VersionString} ({HedgeApp.ProgramName})";
                if (mainWindow.SelectedModProfile != null)
                    title += $" - {mainWindow.SelectedModProfile?.Name}";
                if (HedgeApp.IsLinux)
                    title += " (Linux)";
                mainWindow.Title = title;
            }
        }

        public static void OnLanguageLoad(ResourceDictionary dict, string culture)
        {
            EventStrings.Clear();
            if (IsAprilFools && culture.StartsWith("en"))
            {
                foreach (var item in dict)
                {
                    if (item is DictionaryEntry entry && entry.Value is string value)
                    {
                        EventStrings.Add(entry.Key, value
                            .Replace("HedgeModManager", "SonicGMI")
                            .Replace("Hedge Mod Manager", "SonicGMI"));
                    }
                }
            }

            if (!Application.Current.Resources.MergedDictionaries.Contains(EventStrings))
                Application.Current.Resources.MergedDictionaries.Add(EventStrings);
        }
    }
}
