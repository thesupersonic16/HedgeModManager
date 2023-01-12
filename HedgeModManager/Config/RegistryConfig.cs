using Microsoft.Win32;
using System.Reflection;

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

        public static bool CodesUseTreeView = true;

        static RegistryConfig()
        {
            Load();
        }

        public static void Save()
        {
            var key = Registry.CurrentUser.CreateSubKey(ConfigPath);
            key.SetValue("ExecutablePath", Assembly.GetExecutingAssembly().Location);
            key.SetValue("LastGame", LastGameDirectory);
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
            LastGameDirectory       = (string)key.GetValue("LastGame", string.Empty);
            ExtraGameDirectories    = (string)key.GetValue(nameof(ExtraGameDirectories), string.Empty);
            UILanguage              = (string)key.GetValue(nameof(UILanguage), HedgeApp.PCCulture);
            UITheme                 = (string)key.GetValue(nameof(UITheme), useLightMode ? "LightTheme" : "DarkerTheme");
            CodesSortingColumnIndex = (int)key.GetValue(nameof(CodesSortingColumnIndex), 1);
            CodesUseTreeView        = (int)key.GetValue(nameof(CodesUseTreeView), 1) != 0;
            key.Close();
        }
    }
}
