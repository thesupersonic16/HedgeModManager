using System;
using System.IO;
using System.Windows.Forms;

namespace SLWModLoader
{
    internal static class Program
    {
        //Variables/Constants
        public static string StartDirectory = Application.StartupPath;
        public static string ExecutableName = Path.GetFileName(Application.ExecutablePath);
        public static bool Restart = false;
        public const string ProgramName = "SLW Mod Loader";
        public const string ProgramNameShort = "SLWModLoader";
        public const string VersionString = "6.0";

        //Methods
        [STAThread]
        private static void Main()
        {
            LogFile.Initialize();
            LogFile.AddMessage($"Starting {ProgramName} (v{VersionString})...");

            #if DEBUG
            if (!(File.Exists(Path.Combine(StartDirectory, "slw.exe")) ||
                File.Exists(Path.Combine(StartDirectory, "SonicGenerations.exe"))))
            {
                StartDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Sonic Lost World";
                if (!File.Exists(StartDirectory))
                    StartDirectory = "D:\\SteamLibrary\\steamapps\\common\\Sonic Generations";
            }
            #endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            while (Restart)
            {
                Restart = false;
                LogFile.Initialize();
                LogFile.AddMessage($"Starting {ProgramName} (v{VersionString})...");
                Application.Run(new MainForm());
            }
        }

        public static bool IsURL(string URL)
        {
            return Uri.IsWellFormedUriString(URL, UriKind.RelativeOrAbsolute);
        }

        public static string GetString(int location, string mainString)
        {
            string s = mainString;
            s = s.Substring(s.IndexOf('\"', location) + 1);
            s = s.Replace("\\\"", "%22");
            s = s.Substring(0, s.IndexOf('\"'));
            s = s.Replace("%22", "\\\"");
            return s;
        }

    }
}