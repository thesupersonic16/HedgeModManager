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
        public static string LastGame;
        public static string ExtraGameDirectories;
        public static string UILanguage;
        public static string UITheme;

        public static int CodesSortingColumnIndex = 1;

        public static bool CodesUseTreeView = true;

        static RegistryConfig()
        {
            Load();
        }

        public static void Save()
        {
            var key = Registry.CurrentUser.CreateSubKey(ConfigPath);
            key.SetValue(nameof(LastGame), LastGame);
            key.SetValue(nameof(ExtraGameDirectories), ExtraGameDirectories);
            key.SetValue(nameof(UILanguage), UILanguage);
            key.SetValue(nameof(UITheme), UITheme);
            key.SetValue(nameof(CodesSortingColumnIndex), CodesSortingColumnIndex);
            key.SetValue(nameof(CodesUseTreeView), CodesUseTreeView ? 1 : 0);
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
            LastGame                = (string)key.GetValue(nameof(LastGame), string.Empty);
            ExtraGameDirectories    = (string)key.GetValue(nameof(ExtraGameDirectories), string.Empty);
            UILanguage              = (string)key.GetValue(nameof(UILanguage), HedgeApp.PCCulture);
            UITheme                 = (string)key.GetValue(nameof(UITheme), useLightMode ? "LightTheme" : "DarkerTheme");
            CodesSortingColumnIndex = (int)key.GetValue(nameof(CodesSortingColumnIndex), 1);
            CodesUseTreeView        = (int)key.GetValue(nameof(CodesUseTreeView), 1) != 0;
            key.Close();
        }
    }
}
