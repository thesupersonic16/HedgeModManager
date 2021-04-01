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
        private const string ConfigPath = @"SOFTWARE\HEDGEMM";
        private const string PersonalizePath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        public static bool UseLightMode;
        public static string LastGameDirectory;
        public static string UILanguage;

        static RegistryConfig()
        {
            Load();
        }

        public static void Save()
        {
            var key = Registry.CurrentUser.CreateSubKey(ConfigPath);
            key.SetValue("LastGame", LastGameDirectory);
            key.SetValue("UILanguage", UILanguage);
            key.Close();
        }

        public static void Load()
        {
            var key = Registry.CurrentUser.CreateSubKey(ConfigPath);
            LastGameDirectory = (string)key.GetValue("LastGame", string.Empty);
            UILanguage = (string)key.GetValue("UILanguage", HedgeApp.PCCulture);
            key.Close();

            var personalizeKey = Registry.CurrentUser.OpenSubKey(PersonalizePath);

            if (personalizeKey != null)
            {
                var useLightThemeStr = personalizeKey.GetValue("SystemUsesLightTheme", 0)?.ToString();
                if (int.TryParse(useLightThemeStr, out int useLightTheme))
                {
                    UseLightMode = useLightTheme != 0;
                }

                personalizeKey.Close();
            }
        }
    }
}
