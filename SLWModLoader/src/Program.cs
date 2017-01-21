using System;
using System.Windows.Forms;

namespace SLWModLoader
{
    internal static class Program
    {
        //Variables/Constants
        public static string StartDirectory = Application.StartupPath;
        public const string VersionString = "6.0";

        //Methods
        [STAThread]
        private static void Main()
        {
            LogFile.Initialize();
            LogFile.AddMessage("Starting application...");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}