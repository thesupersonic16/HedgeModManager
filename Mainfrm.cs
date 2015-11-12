using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Collections.Generic;

namespace SLWModLoader
{
    public partial class Mainfrm : Form
    {
        public static string versionstring = "3.2";
        public static Thread generatemodsdbthread, loadmodthread, updatethread;
        public static WebClient client = new WebClient();
        public static string[] configfile; public static List<string> oldmods = new List<string>();

        public Mainfrm()
        {
            //Initialize the form
            InitializeComponent();

            //Set the form's title
            Text = $"SLW Mod Loader (v {versionstring})";
            modsdir.Text = @"C:\Program Files (x86)\Steam\SteamApps\common\Sonic Lost World";

            //Load the config file
            if (File.Exists(Application.StartupPath + "\\config.txt"))
            {
                configfile = File.ReadAllLines(Application.StartupPath + "\\config.txt");
                modsdir.Text = configfile[1];
            }

            //Set the mod directory textbox
            if (Directory.Exists(modsdir.Text))
            {
                if (!Directory.Exists(modsdir.Text+"\\mods") && MessageBox.Show("A \"mods\" folder does not exist within your Sonic Lost World installation directory. Would you like to create one?", "SLW Mod Loader", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) Directory.CreateDirectory(modsdir.Text + "\\mods");
            }
            else { MessageBox.Show("SLW Mod Loader could not find your Sonic Lost World installation directory. You'll have to manually set it.","SLW Mod Loader",MessageBoxButtons.OK,MessageBoxIcon.Warning); modsdir.Text = ""; }

            //3.1 Update
            if (Convert.ToSingle(configfile[0]) <= 3.0f || configfile == null)
            {
                DialogResult dopatch = DialogResult.Yes;
                while (dopatch == DialogResult.Yes) { dopatch = MessageBox.Show("We assume your slw.exe executable is unpatched given you just updated/installed the application. The following step is VERY IMPORTANT, so to prove that you've read it, click \"No.\" A window is about to open. In this window, you need to type Y and press enter. This will automatically patch your slw.exe executable, and will not need to be done again. If your executable is already patched, click \"Cancel\"", "SLW Mod Loader", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning); }

                if (dopatch == DialogResult.No)
                {
                    Process updateprocess = new Process() { StartInfo = new ProcessStartInfo(Application.StartupPath + "\\cpkredirInst.exe", '"' + (File.Exists(modsdir.Text + "\\slw.exe") ? modsdir.Text + "\\slw.exe" : File.Exists(Application.StartupPath + "\\slw.exe") ? Application.StartupPath + "\\slw.exe" : @"C:\Program Files (x86)\Steam\SteamApps\common\Sonic Lost World\slw.exe") + '"') };
                    updateprocess.Start();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Define thread variables
            loadmodthread = new Thread(new ThreadStart(LoadMods));
            updatethread = new Thread(new ThreadStart(CheckForUpdates));
            generatemodsdbthread = new Thread(new ThreadStart(GenerateModsDB));

            //Load the list of mods
            statuslbl.Text = "Loading mods...";
            loadmodthread.Start();

            //Remove leftover temporary files if they exist
            if (Directory.Exists(Application.StartupPath + "\\temp")) { Directory.Delete(Application.StartupPath + "\\temp", true); }
            if (File.Exists(Application.StartupPath + "\\update.bat")) { File.Delete(Application.StartupPath + "\\update.bat"); }

            //Check for updates
            statuslbl.Text = "Checking for updates...";
            updatethread.Start();
        }

        private void LoadMods()
        {
            Invoke(new Action(() => { modslist.Items.Clear(); oldmods.Clear(); }));
            if (!string.IsNullOrEmpty(modsdir.Text) && Directory.Exists(modsdir.Text+"\\mods"))
            {
                foreach (string mod in Directory.GetDirectories(modsdir.Text+"\\mods"))
                {
                    if (File.Exists(mod + "\\mod.ini")) { Invoke(new Action(() => { modslist.Items.Add(new ListViewItem(new DirectoryInfo(mod).Name) { Tag = new DirectoryInfo(mod).Name }); })); }
                    else { oldmods.Add(mod); }
                }
            }

            bool moveoldmods = false;
            Invoke(new Action(() => { nomodsfound.Visible = refreshlbl.Visible = (modslist.Items.Count < 1); modslist.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); }));
            Invoke(new Action(() => { moveoldmods = (oldmods.Count > 0 && MessageBox.Show("Your mods folder seems to contain mods designed for the pre-3.0 version of the mod loader. Would you like to attempt to update them?", "SLW Mod Loader", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes); if (moveoldmods) { statuslbl.Text = "Updating old mods..."; } }));

            if (moveoldmods)
            {
                foreach (string oldmod in oldmods)
                {
                    //Make a temporary directory
                    if (Directory.Exists(Application.StartupPath + "\\temp")) { Directory.Delete(Application.StartupPath + "\\temp", true); }
                    Directory.CreateDirectory(Application.StartupPath + "\\temp");

                    //Move the old mod into the temporary directory
                    Directory.Move(oldmod, Application.StartupPath + "\\temp\\"+new DirectoryInfo(oldmod).Name);

                    //Re-format the old mod's directory
                    Directory.CreateDirectory(oldmod + "\\disk");
                    //Directory.CreateDirectory(oldmod + "\\disk\\sonic2013_patch_0");
                    File.WriteAllLines(oldmod+"\\mod.ini",new string[]
                    {
                        "[Main]",
                        "IncludeDir0=\".\"",
                        "IncludeDirCount=1",
                        "SaveFile=\"save\\sonic.sav\"",
                        "",
                        "[Desc]",
                        $"Title=\"{new DirectoryInfo(oldmod).Name}\"",
                        "Description=\"This mod was automatically updated from it's previous form, and as such, does not have a description.\"",
                        "Version=\"1.0\"",
                        "Date=\"11/11/15\"",
                        "Author=\"Radfordhound\"",
                        "URL=\"\""
                    });

                    //Move the old mod back into it's original directory
                    Directory.Move(Application.StartupPath + "\\temp\\"+new DirectoryInfo(oldmod).Name, oldmod + "\\disk\\sonic2013_patch_0");
                }

                Invoke(new Action(() => statuslbl.Text = ""));
                loadmodthread = new Thread(new ThreadStart(LoadMods));
                loadmodthread.Start();
            }
        }

        private void CheckForUpdates()
        {
            try
            {
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                Updatefrm.latest = new StreamReader(client.OpenRead("https://api.github.com/repos/Radfordhound/SLW-Mod-Loader/releases/latest")).ReadToEnd();
                Updatefrm.latestversion = Updatefrm.latest.Substring(Updatefrm.latest.IndexOf("tag_name") + 11, 3);

                if (Convert.ToSingle(Updatefrm.latestversion) > Convert.ToSingle(versionstring) && MessageBox.Show($"A new version of the application (version v{Updatefrm.latestversion}) has been released. Would you like to download it?", "SLW Mod Loader", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Invoke(new Action(() => { Close(); }));
                    new Updatefrm().ShowDialog();
                }
                else { Invoke(new Action(() => { statuslbl.Text = ""; })); }
            }
            catch { Invoke(new Action(() => { statuslbl.Text = "Checking for updates failed."; })); }
        }

        private void GenerateModsDB()
        {
            //Make a backup for going back to vanilla Lost World if one doesn't already exist
            //if (!File.Exists(modsdir.Text + "\\disk\\sonic2013_patch_0_Backup.cpk") && File.Exists(modsdir.Text + "\\disk\\sonic2013_patch_0.cpk")) { File.Copy(modsdir.Text + "\\disk\\sonic2013_patch_0.cpk", modsdir.Text + "\\disk\\sonic2013_patch_0_Backup.cpk"); }

            ////Re-pack the cpk using cpkmackec.exe
            //using (Process proc = new Process())
            //{
            //    proc.StartInfo = new ProcessStartInfo("cpkmakec.exe", $"\"{Application.StartupPath}\\temp\" \"{modsdir.Text}\\disk\\sonic2013_patch_0.cpk\" -align=2048 -code=UTF-8 -mode=FILENAME -mask");
            //    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //    proc.Start();
            //    proc.WaitForExit();
            //}

            ////Delete the temp folder
            //Directory.Delete(Application.StartupPath+"\\temp", true);

            if (File.Exists(modsdir.Text + "\\mods\\ModsDB.ini")) { File.Delete(modsdir.Text + "\\mods\\ModsDB.ini"); }

            int checkeditemcount = 0;
            List<string> checkedmods = new List<string>(), mods = new List<string>(), modsdb;

            Invoke(new Action(() =>
            {
                checkeditemcount = modslist.CheckedItems.Count;
                foreach (ListViewItem checkedmod in modslist.CheckedItems) { checkedmods.Add((string)checkedmod.Tag); }
                foreach (ListViewItem mod in modslist.Items) { mods.Add((string)mod.Tag); }
            }));

            modsdb = new List<string>() { "[Main]", $"ActiveModCount={checkeditemcount.ToString()}" };

            for (int i = 0; i < checkeditemcount; i++)
            {
                modsdb.Add($"ActiveMod{i.ToString()}={checkedmods[i]}");
            }
            modsdb.Add("[Mods]");
            foreach (string mod in mods)
            {
                modsdb.Add($"{mod}={mod}\\mod.ini");
            }

            File.WriteAllLines(modsdir.Text + "\\mods\\ModsDB.ini", modsdb);

            //Close the mod loader and start Sonic Lost World
            Invoke(new Action(() => { statuslbl.Text = "Starting SLW..."; }));
            Process slw = new Process();
            slw.StartInfo = new ProcessStartInfo(Application.StartupPath + "\\Sonic Lost World.url");

            Invoke(new Action(() => { Close(); }));
            slw.Start();
        }

        private void refreshlbl_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            loadmodthread = new Thread(new ThreadStart(LoadMods));
            loadmodthread.Start();
        }

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            loadmodthread = new Thread(new ThreadStart(LoadMods));
            loadmodthread.Start();
        }

        //Up...
        private void MoveUpbtn_Click(object sender, EventArgs e)
        {
            if (modslist.SelectedItems.Count > 0 && modslist.SelectedItems[0].Index > 0)
            {
                int index = modslist.SelectedItems[0].Index - 1;
                ListViewItem lvi = modslist.SelectedItems[0];
                modslist.Items.RemoveAt(modslist.SelectedItems[0].Index);
                modslist.Items.Insert(index, lvi);
            }
        }

        //..and down and all around.
        private void MoveDownbtn_Click(object sender, EventArgs e)
        {
            if (modslist.SelectedItems.Count > 0 && modslist.SelectedItems[0].Index < modslist.Items.Count - 1)
            {
                int index = modslist.SelectedItems[0].Index + 1;
                ListViewItem lvi = modslist.SelectedItems[0];
                modslist.Items.RemoveAt(modslist.SelectedItems[0].Index);
                modslist.Items.Insert(index, lvi);
            }
        }

        #region SHHHH... DON'T LOOK! IT'S A SECRET!!!
        private void modsdir_TextChanged(object sender, EventArgs e)
        {
            if (modsdir.Text == "DO A BARREL ROLL") { label.Font = nomodsfound.Font = refreshlbl.Font = playbtn.Font = refreshbtn.Font = modsdirbtn.Font = modslist.Font = new Font("Comic Sans MS",8);  }
        }
        #endregion

        private void playbtn_Click(object sender, EventArgs e)
        {
            playbtn.Enabled = false;
            statuslbl.Text = "Generating ModsDB.ini...";
            generatemodsdbthread.Start();

            //if (modslist.CheckedItems.Count > 0)
            //{
            //    //Make a temporary directory
            //    //if (Directory.Exists(Application.StartupPath + "\\temp")) { Directory.Delete(Application.StartupPath + "\\temp", true); }
            //    //Directory.CreateDirectory(Application.StartupPath + "\\temp");

            //    //Copy all the stuff there
            //    //statuslbl.Text = "Copying mod data...";
            //    //foreach (ListViewItem mod in modslist.CheckedItems)
            //    //{
            //    //    if (Directory.Exists(modsdir.Text + "\\mods\\" + mod.Text))
            //    //    {
            //    //        DirectoryCopy(modsdir.Text + "\\mods\\" + mod.Text, Application.StartupPath + "\\temp", true);
            //    //    }
            //    //}

            //    statuslbl.Text = "Generating ModsDB.ini...";
            //    generatemodsdbthread.Start();
            //}
            //else
            //{
            //    if (File.Exists(modsdir.Text + "\\disk\\sonic2013_patch_0_Backup.cpk")) { File.Copy(modsdir.Text + "\\disk\\sonic2013_patch_0_Backup.cpk", modsdir.Text + "\\disk\\sonic2013_patch_0.cpk", true); }

            //    //Start Sonic Lost World
            //    statuslbl.Text = "Starting SLW...";
            //    Process slw = new Process();
            //    slw.StartInfo = new ProcessStartInfo(Application.StartupPath + "\\Sonic Lost World.url");

            //    slw.Start();
            //    Close();
            //}
        }

        private void modsdirbtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog() { ShowNewFolderButton = true, Description = "The folder which contains the Sonic Lost World executable (slw.exe), as well as a \"disk\" folder." };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                modsdir.Text = fbd.SelectedPath;
            }
        }

        private void MoveUpAll_Click(object sender, EventArgs e)
        {
            if (modslist.SelectedItems.Count > 0 && modslist.SelectedItems[0].Index > 0)
            {
                ListViewItem lvi = modslist.SelectedItems[0];
                modslist.Items.RemoveAt(modslist.SelectedItems[0].Index);
                modslist.Items.Insert(0, lvi);
            }
        }

        private void MoveDownAll_Click(object sender, EventArgs e)
        {
            if (modslist.SelectedItems.Count > 0 && modslist.SelectedItems[0].Index < modslist.Items.Count)
            {
                ListViewItem lvi = modslist.SelectedItems[0];
                modslist.Items.RemoveAt(modslist.SelectedItems[0].Index);
                modslist.Items.Insert(0, lvi);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new AboutFrm().ShowDialog();
        }

        private void Mainfrm_Closing(object sender, FormClosingEventArgs e)
        {
            //Write config file
            if (File.Exists(Application.StartupPath + "\\config.txt")) { File.Delete(Application.StartupPath + "\\config.txt"); }
            File.WriteAllLines(Application.StartupPath + "\\config.txt",new string[] { versionstring, modsdir.Text });

            //Delete leftover temporary junk
            if (Directory.Exists(Application.StartupPath + "\\temp")) { Directory.Delete(Application.StartupPath + "\\temp", true); }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                if (!File.Exists(temppath)) { file.CopyTo(temppath, false); }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
