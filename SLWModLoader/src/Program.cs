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
        public const string ProgramName = "SLW Mod Loader";
        public const string ProgramNameShort = "SLWModLoader";
        public const string VersionString = "6.0";

        //Methods
        [STAThread]
        private static void Main()
        {
            LogFile.Initialize();
            LogFile.AddMessage($"Starting {ProgramName} (v{VersionString})...");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        public static bool IsURL(string URL)
        {
            return Uri.IsWellFormedUriString(URL, UriKind.RelativeOrAbsolute);
        }
    }
}