using System;
using System.Windows.Forms;

namespace SLWModLoader
{
    internal static class Program
    {
        public static string VersionString = "6.0", StartDirectory = Application.StartupPath;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            LogFile.Initialize();
            LogFile.AddMessage("Starting application...\n");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}