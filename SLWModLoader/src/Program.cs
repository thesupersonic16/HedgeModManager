using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLWModLoader
{
    static class Program
    {
        public static bool writelog = true;
        public static Mainfrm mainfrm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Mainfrm.logfile.Add("Starting mod loader...");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                mainfrm = new Mainfrm();
                Application.Run(mainfrm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: "+ex.Message+Environment.NewLine + Environment.NewLine + Environment.NewLine+"ADVANCED INFO (FOR DEBUGGING): "+Environment.NewLine + Environment.NewLine + ex.ToString(),"SLW Mod Loader",MessageBoxButtons.OK,MessageBoxIcon.Error);
                Mainfrm.logfile.Add("ERROR: " + ex.Message + Environment.NewLine + Environment.NewLine + Environment.NewLine + "ADVANCED INFO (FOR DEBUGGING): " + Environment.NewLine + Environment.NewLine + ex.ToString());
            }

            //Write log file
            if (writelog)
            {
                if (File.Exists(Application.StartupPath + "\\log.txt")) { File.Delete(Application.StartupPath + "\\log.txt"); }
                File.WriteAllLines(Application.StartupPath + "\\log.txt", Mainfrm.logfile);
            }
        }
    }
}
