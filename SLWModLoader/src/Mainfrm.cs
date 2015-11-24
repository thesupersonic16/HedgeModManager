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
        public static string versionstring = "4.4";
        public static Thread generatemodsdbthread, loadmodthread, updatethread, patchthread;
        public static WebClient client = new WebClient();
        public static string[] configfile; public static List<string> oldmods = new List<string>(), logfile = new List<string>();

        public Mainfrm()
        {
            logfile.Add("Initializing main form...");
            logfile.Add("");

            //Initialize the form
            InitializeComponent();

            //Set the form's title
            Text = $"SLW Mod Loader (v {versionstring})";
            modsdir.Text = @"C:\Program Files (x86)\Steam\SteamApps\common\Sonic Lost World";

            //Load the config file
            if (File.Exists(Application.StartupPath + "\\config.txt"))
            {
                logfile.Add($"Reading config file from \"{Application.StartupPath+"\\config.txt"}\"...");

                configfile = File.ReadAllLines(Application.StartupPath + "\\config.txt");
                if (configfile.Length > 1 && configfile[1] != null) { modsdir.Text = configfile[1]; }
                if (configfile.Length > 2 && configfile[2] != null && (configfile[2].ToUpper() == "TRUE" || configfile[2].ToUpper() == "FALSE")) { makelogfile.Checked = Convert.ToBoolean(configfile[2]); }

                logfile.Add("Config file read.");
            }
            else logfile.Add("No config file found. Proceeding with default settings...");

            logfile.Add("");

            //Set the mod directory textbox
            if (Directory.Exists(modsdir.Text))
            {
                if (!Directory.Exists(modsdir.Text + "\\mods") && MessageBox.Show("A \"mods\" folder does not exist within your Sonic Lost World installation directory. Would you like to create one?", "SLW Mod Loader", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) { Directory.CreateDirectory(modsdir.Text + "\\mods"); logfile.Add($"Mods directory made at {modsdir.Text+"\\mods"}"); logfile.Add(""); }
            }
            else { MessageBox.Show("SLW Mod Loader could not find your Sonic Lost World installation directory. You'll have to manually set it.","SLW Mod Loader",MessageBoxButtons.OK,MessageBoxIcon.Warning); modsdir.Text = ""; }

            //3.5 update
            if ((configfile == null) || (IsFloat(configfile[0]) && Convert.ToSingle(configfile[0]) < 3.5f))
            {
                logfile.Add("Deleting un-needed files leftover from previous editions of the mod loader.");
                //Delete all the leftover files from previous versions of the mod loader if they exist
                if (File.Exists(Application.StartupPath + "\\cpkredirInst.exe")) { File.Delete(Application.StartupPath + "\\cpkredirInst.exe"); }

                if (File.Exists(Application.StartupPath + "\\cpkmakec.exe")) { File.Delete(Application.StartupPath + "\\cpkmakec.exe"); }
                if (File.Exists(Application.StartupPath + "\\cpkmaker.out.csv")) { File.Delete(Application.StartupPath + "\\cpkmaker.out.csv"); }
                if (File.Exists(Application.StartupPath + "\\CpkMaker.DLL")) { File.Delete(Application.StartupPath + "\\CpkMaker.DLL"); }
                logfile.Add("");
            }
        }

        private bool IsFloat(string s)
        {
            float result;
            return float.TryParse(s, out result);
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            //Define thread variables
            loadmodthread = new Thread(new ThreadStart(LoadMods));
            updatethread = new Thread(new ThreadStart(CheckForUpdates));
            generatemodsdbthread = new Thread(new ThreadStart(GenerateModsDB));
            patchthread = new Thread(new ThreadStart(PatchEXE));

            //Load the list of mods
            statuslbl.Text = "Loading mods...";
            logfile.Add($"Started loading mods from \"{modsdir.Text+"\\mods"}\"..."); logfile.Add("");
            loadmodthread.Start();

            //Remove leftover temporary files if they exist
            if (Directory.Exists(Application.StartupPath + "\\temp")) { logfile.Add("Deleting temporary folder..."); logfile.Add(""); Directory.Delete(Application.StartupPath + "\\temp", true); }
            if (File.Exists(Application.StartupPath + "\\update.bat")) { logfile.Add("Deleting temporary file..."); logfile.Add(""); File.Delete(Application.StartupPath + "\\update.bat"); }

            //Check for updates
            statuslbl.Text = "Checking for updates to mod loader...";
            logfile.Add("Started checking for updates to mod loader..."); logfile.Add("");
            updatethread.Start();
        }

        private string GetModINIinfo(List<string> modini, string datatoget)
        {
            for (int i = 0; i < modini.Count; i++)
            {
                if (modini[i].Length > datatoget.Length+2 && modini[i].Substring(0,datatoget.Length+2) == datatoget + "=\"") { return modini[i].Substring(modini[i].IndexOf(datatoget + "=\"") + datatoget.Length + 2, modini[i].Length- (modini[i].IndexOf(datatoget + "=\"") + datatoget.Length + 3)); }
            }
            return null;
        }

        private void PatchEXE()
        {
            if (File.Exists(modsdir.Text + "\\slw.exe"))
            {
                //Read the executable
                byte[] slwexe;
                try { slwexe = File.ReadAllBytes(modsdir.Text + "\\slw.exe"); } catch (Exception ex) { logfile.Add("ERROR: "+ex.Message); return; }

                //Check to see if the executable is patched or not
                for (long i = 11918776; i < slwexe.Length; i++)
                {
                    //Break if "cpkredir" is found, meaning the executable has already been patched
                    if (slwexe[i] == 99 && slwexe[i + 2] == 112 && slwexe[i + 3] == 107 &&
                        slwexe[i + 4] == 114 && slwexe[i + 5] == 101 && slwexe[i + 6] == 100 &&
                        slwexe[i + 7] == 105 && slwexe[i + 8] == 114)
                    { break; }

                    //Ask if you should patch the executable if "imagehlp" is found, meaning the executable hasn't yet been patched
                    if (slwexe[i] == 105 && slwexe[i+1] == 109 && slwexe[i+2] == 97 &&
                        slwexe[i+3] == 103 && slwexe[i+4] == 101 && slwexe[i+5] == 104 &&
                        slwexe[i+6] == 108 && slwexe[i+7] == 112)
                    {
                        //We do this via an invoke to freeze the GUI thread until the messagebox is answered.
                        DialogResult dopatch = DialogResult.No;
                        Invoke(new Action(() =>
                        {
                            dopatch = MessageBox.Show("Your Sonic Lost World executable has not yet been patched for use with CPKREDIR, which is required to load mods. Would you like to patch it now?", "SLW Mod Loader", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        }));

                        if (dopatch == DialogResult.Yes)
                        {
                            Invoke(new Action(() => statuslbl.Text = "Patching executable..."));

                            /*
                              Here we're essentially hex editing the slw.exe executable to change
                              the string "imagehlp" to "cpkredir", typically found at decimal address
                              11,918,776 or beyond, depending on which version of the game the user is
                              using.
                            */

                            //cpkredir                                      -cpkredir
                            //c p k r e d i r                               -cpkredir spaced out
                            //99 112 107 114 101 100 105 114                -cpkredir in binary

                            slwexe[i] = 99; slwexe[i + 1] = 112; //99  = c, 112 = p
                            slwexe[i + 2] = 107; slwexe[i + 3] = 114; //107 = k, 114 = r
                            slwexe[i + 4] = 101; slwexe[i + 5] = 100; //101 = e, 100 = d
                            slwexe[i + 6] = 105; slwexe[i + 7] = 114; //105 = i, 114 = r

                            //Now that we've edited the executable, all that's left is to make a backup of the old one...
                            if (!File.Exists(modsdir.Text + "\\slw_Backup.exe")) { File.Move(modsdir.Text + "\\slw.exe", modsdir.Text + "\\slw_Backup.exe"); }
                            else { File.Delete(modsdir.Text + "\\slw.exe"); }

                            //...and write the new one.
                            File.WriteAllBytes(modsdir.Text + "\\slw.exe", slwexe);
                            Invoke(new Action(() => statuslbl.Text = ""));
                        }
                        break;
                    }
                }
            }
        }

        private void LoadMods()
        {
            Invoke(new Action(() => { modslist.Items.Clear(); oldmods.Clear(); }));
            if (!string.IsNullOrEmpty(modsdir.Text) && Directory.Exists(modsdir.Text+"\\mods"))
            {
                foreach (string mod in Directory.GetDirectories(modsdir.Text+"\\mods"))
                {
                    if (File.Exists(mod + "\\mod.ini")) { Invoke(new Action(() =>
                    {
                        List<string> modini = new List<string>() { mod };
                        modini.AddRange(File.ReadAllLines(mod + "\\mod.ini"));

                        ListViewItem modlvi = new ListViewItem(GetModINIinfo(modini, "Title")) { Tag = modini };
                        modlvi.SubItems.Add(GetModINIinfo(modini, "Version")); modlvi.SubItems.Add(GetModINIinfo(modini, "Author"));
                        modlvi.SubItems.Add((GetModINIinfo(modini, "SaveFile") != null)?"Yes":"No");
                        modslist.Items.Add(modlvi);
                    })); }
                    else { oldmods.Add(mod); }
                }
            }

            bool moveoldmods = false;
            Invoke(new Action(() => { nomodsfound.Visible = refreshlbl.Visible = (modslist.Items.Count < 1); modslist.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); }));
            Invoke(new Action(() => { moveoldmods = (oldmods.Count > 0 && MessageBox.Show("Your mods folder seems to contain mods designed for the pre-3.0 version of the mod loader. Would you like to attempt to update them?", "SLW Mod Loader", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes); if (moveoldmods) { statuslbl.Text = "Updating old mods..."; } }));

            if (moveoldmods)
            {
                logfile.Add("Updating old mods...");
                foreach (string oldmod in oldmods)
                {
                    //Make a temporary directory
                    if (Directory.Exists(Application.StartupPath + "\\temp")) { Directory.Delete(Application.StartupPath + "\\temp", true); }
                    Directory.CreateDirectory(Application.StartupPath + "\\temp");

                    //Move the old mod into the temporary directory
                    Directory.Move(oldmod, Application.StartupPath + "\\temp\\" + new DirectoryInfo(oldmod).Name);

                    //Re-format the old mod's directory
                    Directory.CreateDirectory(oldmod + "\\disk");
                    //Directory.CreateDirectory(oldmod + "\\disk\\sonic2013_patch_0");
                    File.WriteAllLines(oldmod + "\\mod.ini", new string[]
                    {
                        "[Main]",
                        "IncludeDir0=\".\"",
                        "IncludeDirCount=1",
                        "",
                        "[Desc]",
                        $"Title=\"{new DirectoryInfo(oldmod).Name}\"",
                        "Description=\"This mod was automatically updated from it's previous form, and as such, does not have a description.\"",
                        "Version=\"1.0\"",
                    });

                    //Move the old mod back into it's original directory
                    Directory.Move(Application.StartupPath + "\\temp\\" + new DirectoryInfo(oldmod).Name, oldmod + "\\disk\\sonic2013_patch_0");
                }

                logfile.Add("Finished updating old mods.");

                Invoke(new Action(() => statuslbl.Text = ""));
                loadmodthread = new Thread(new ThreadStart(LoadMods));
                loadmodthread.Start();
            }
            else { logfile.Add("Finised loading mods."); logfile.Add(""); }
        }

        private void CheckForUpdates()
        {
            try
            {
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                Updatefrm.latest = new StreamReader(client.OpenRead("https://api.github.com/repos/Radfordhound/SLW-Mod-Loader/releases/latest")).ReadToEnd();
                Updatefrm.latestversion = Updatefrm.latest.Substring(Updatefrm.latest.IndexOf("tag_name") + 11, 3);
                logfile.Add("Got latest release information from GitHub.");

                if (Convert.ToSingle(Updatefrm.latestversion) > Convert.ToSingle(versionstring) && MessageBox.Show($"A new version of the application (version v{Updatefrm.latestversion}) has been released. Would you like to download it?", "SLW Mod Loader", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Invoke(new Action(() => { Close(); }));
                    new Updatefrm().ShowDialog();
                }
                else { logfile.Add("No new updates avaliable."); Invoke(new Action(() => { statuslbl.Text = ""; })); }

                if (configfile != null && configfile.Length > 0 && configfile[0] != null && Convert.ToSingle(versionstring) > Convert.ToSingle(configfile[0]))
                {
                    Invoke(new Action(() => new NewUpdateFrm().ShowDialog()));
                }
            }
            catch { logfile.Add("Checking for updates has failed. Please try again."); Invoke(new Action(() => { statuslbl.Text = "Checking for updates failed."; })); }
            patchthread.Start();
        }

        private void GenerateModsDB()
        {
            logfile.Add("");
            logfile.Add("Deleting old ModsDB.ini file...");
            
            //Delete old ModsDB.ini file if it exists
            if (File.Exists(modsdir.Text + "\\mods\\ModsDB.ini")) { File.Delete(modsdir.Text + "\\mods\\ModsDB.ini"); }

            logfile.Add("Forming a list of checked mods...");

            //Form a list of "checked" mods
            int checkeditemcount = 0;
            List<string> checkedmods = new List<string>(), mods = new List<string>(), modsdb;

            Invoke(new Action(() =>
            {
                checkeditemcount = modslist.CheckedItems.Count;
                foreach (ListViewItem checkedmod in modslist.CheckedItems) { checkedmods.Add(new DirectoryInfo(((List<string>)checkedmod.Tag)[0]).Name); }
                foreach (ListViewItem mod in modslist.Items) { mods.Add(new DirectoryInfo(((List<string>)mod.Tag)[0]).Name); }
            }));

            logfile.Add("Generating ModsDB.ini...");

            //Generate the ModsDB.ini file using this data
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

            logfile.Add("Saving newly-generated ModsDB.ini...");

            //Save the generated file
            File.WriteAllLines(modsdir.Text + "\\mods\\ModsDB.ini", modsdb);

            logfile.Add("Closing mod loader and starting Sonic Lost World...");

            //Close the mod loader and start Sonic Lost World
            Invoke(new Action(() => { statuslbl.Text = "Starting SLW..."; }));
            Process slw = new Process();
            slw.StartInfo = new ProcessStartInfo(Application.StartupPath + "\\Sonic Lost World.url");

            Invoke(new Action(() => { Close(); }));
            slw.Start();
        }

        private void refreshlbl_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            descriptionlbl.Text = "Click on a mod to see it's description!";
            descriptionlbl.LinkBehavior = LinkBehavior.NeverUnderline;

            loadmodthread = new Thread(new ThreadStart(LoadMods));
            loadmodthread.Start();
        }

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            descriptionlbl.Text = "Click on a mod to see it's description!";
            descriptionlbl.LinkBehavior = LinkBehavior.NeverUnderline;

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

        private void modslist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (modslist.SelectedItems.Count > 0 && modslist.SelectedItems[0].Tag != null && ((List<string>)modslist.SelectedItems[0].Tag).Count > 0)
            {
                string description = GetModINIinfo((List<string>)modslist.SelectedItems[0].Tag, "Description");
                descriptionlbl.Tag = description;
                descriptionlbl.LinkBehavior = LinkBehavior.NeverUnderline;

                if (TextRenderer.MeasureText(description, new Font(descriptionlbl.Font.FontFamily, descriptionlbl.Font.Size, descriptionlbl.Font.Style)).Width > 532)
                {
                    while (TextRenderer.MeasureText(description, new Font(descriptionlbl.Font.FontFamily, descriptionlbl.Font.Size, descriptionlbl.Font.Style)).Width > 532)
                    {
                        description = description.Substring(0, description.Length - 1);
                    }
                    if (description.Substring(description.Length - 3, 3) != "...") { description += "..."; descriptionlbl.LinkBehavior = LinkBehavior.HoverUnderline; }
                }

                descriptionlbl.Text = (!string.IsNullOrEmpty(description))?description:"This mod doesn't contain a description. Click here to learn more about it.";
            }
            else if (modslist.SelectedItems.Count <= 0)
            {
                descriptionlbl.Text = "Click on a mod to see it's description. Then try clicking on me! :)";
                descriptionlbl.LinkBehavior = LinkBehavior.NeverUnderline;
            }
        }

        private void makelogfile_CheckedChanged(object sender, EventArgs e)
        {
            //We use a boolean present in the main Program.cs file rather than simply using the pre-built makelogfile.Checked variable so we don't have to rely on the checkbox existing.
            Program.writelog = makelogfile.Checked;
        }

        private void reportlbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new Process() { StartInfo = new ProcessStartInfo("https://github.com/Radfordhound/SLW-Mod-Loader/issues/new") }.Start();
        }

        private void descriptionlbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (modslist.SelectedItems.Count > 0) { new descriptionFrm(GetModINIinfo((List<string>)modslist.SelectedItems[0].Tag, "Description"), GetModINIinfo((List<string>)modslist.SelectedItems[0].Tag, "Title"), GetModINIinfo((List<string>)modslist.SelectedItems[0].Tag, "Author"), GetModINIinfo((List<string>)modslist.SelectedItems[0].Tag, "Date"), GetModINIinfo((List<string>)modslist.SelectedItems[0].Tag, "URL"), GetModINIinfo((List<string>)modslist.SelectedItems[0].Tag, "Version"), GetModINIinfo((List<string>)modslist.SelectedItems[0].Tag, "AuthorURL"), (!string.IsNullOrEmpty(GetModINIinfo((List<string>)modslist.SelectedItems[0].Tag, "BackgroundImage"))?((List<string>)modslist.SelectedItems[0].Tag)[0]+"\\"+GetModINIinfo((List<string>)modslist.SelectedItems[0].Tag, "BackgroundImage"):""), GetModINIinfo((List<string>)modslist.SelectedItems[0].Tag, "TextColor"), GetModINIinfo((List<string>)modslist.SelectedItems[0].Tag, "HeaderColor")).ShowDialog(); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new AboutFrm().ShowDialog();
        }

        protected override bool ProcessDialogKey(Keys key)
        {
            if (ModifierKeys == Keys.None && key == Keys.Escape) { Close(); return true; }
            return base.ProcessDialogKey(key);
        }

        private void Mainfrm_Closing(object sender, FormClosingEventArgs e)
        {
            //Write config file
            if (File.Exists(Application.StartupPath + "\\config.txt")) { File.Delete(Application.StartupPath + "\\config.txt"); }
            File.WriteAllLines(Application.StartupPath + "\\config.txt",new string[] { versionstring, modsdir.Text, Program.writelog.ToString() });

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
