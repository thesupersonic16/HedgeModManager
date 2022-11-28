using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public static class RegistryConfig
    {
        private const string PersonalizePath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        public static string ConfigPath { get; } = @"SOFTWARE\HEDGEMM";
        public static string LastGameDirectory;
        public static string ExtraGameDirectories;
        public static string UILanguage;
        public static string UITheme;

        public static int CodesSortingColumnIndex = 1;

        static RegistryConfig()
        {
            Load();
        }

        public static void Save()
        {
            var key = Registry.CurrentUser.CreateSubKey(ConfigPath);
            key.SetValue("LastGame", LastGameDirectory);
            key.SetValue("ExtraGameDirectories", ExtraGameDirectories);
            key.SetValue("UILanguage", UILanguage);
            key.SetValue("UITheme", UITheme);
            key.SetValue("CodesSortingColumnIndex", CodesSortingColumnIndex);
            key.Close();
        }

        public static void Load()
        {
            // Personalisation for defaulting 
            var personalizeKey = Registry.CurrentUser.OpenSubKey(PersonalizePath);
            bool useLightMode = false;
            if (personalizeKey != null)
            {
                var useLightThemeStr = personalizeKey.GetValue("AppsUseLightTheme", 0)?.ToString();
                if (int.TryParse(useLightThemeStr, out int IuseLightTheme))
                    useLightMode = IuseLightTheme != 0;

                personalizeKey.Close();
            }

            var key = Registry.CurrentUser.CreateSubKey(ConfigPath);
            LastGameDirectory = (string)key.GetValue("LastGame", string.Empty);
            ExtraGameDirectories = (string)key.GetValue("ExtraGameDirectories", string.Empty);
            UILanguage = (string)key.GetValue("UILanguage", HedgeApp.PCCulture);
            UITheme = (string)key.GetValue("UITheme", useLightMode ? "LightTheme" : "DarkerTheme");
            CodesSortingColumnIndex = (int)key.GetValue("CodesSortingColumnIndex", 1);
            key.Close();
        }
    }
}
