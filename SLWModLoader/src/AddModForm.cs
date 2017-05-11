using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO.Compression;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

namespace SLWModLoader
{
    public partial class AddModForm : Form
    {
        public AddModForm()
        {
            InitializeComponent();
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            Close();
            if(makingItRBtn.Checked)
            {
                var nmnf = new NewModNameForm();
                if (nmnf.ShowDialog() == DialogResult.OK)
                {
                    var nmf = new NewModForm(nmnf.GetModName());
                    nmf.ShowDialog();
                }
            }

            if (folderInstallRBtn.Checked)
            {
                var sfd = new SaveFileDialog()
                {
                    Title = "Install Mod From Folder...",
                    FileName = "Enter into a directory that contains mod.ini and press Save"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    InstallFromFolder(Path.GetDirectoryName(sfd.FileName));
                }

            }

            if (fileInstallRBtn.Checked)
            {
                var ofd = new OpenFileDialog()
                {
                    Title = "Install Mod From Archive...",
                    Filter = "Zip (*.zip)|*.zip;"
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    InstallFromZip(ofd.FileName);
                }
            }
        }

        public static void InstallFromZip(string ZipPath)
        {
            // Path to the install temp folder.
            string tempFolder = Path.Combine(Program.StartDirectory, "temp_install");
            // Extracts all contents inside of the zip file.
            ZipFile.ExtractToDirectory(ZipPath, tempFolder);
            // Search and install mods from the temp folder after extracting.
            InstallFromFolder(tempFolder);
            // Deletes the temp folder with all of its contents.
            Directory.Delete(tempFolder, true);
        }

        // Requires 7-Zip to be installed.
        public static void InstallFrom7zArchive(string ArchivePath)
        {
            // Gets 7-Zip's Registry Key.
            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\7-Zip");
            // If null then try get it from the 64-bit Registry.
            if (key == null)
                key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\\7-Zip");
            // Checks if 7-Zip is installed by checking if the key and path value exists.
            if (key != null && key.GetValue("Path") is string path)
            {
                // Path to 7z.exe.
                string exe = Path.Combine(path, "7z.exe");
                // Path to the install temp folder.
                string tempFolder = Path.Combine(Program.StartDirectory, "temp_install");
                // Creates the temp folder.
                Directory.CreateDirectory(tempFolder);
                // Extracts the archive to the temp folder.
                Process.Start(exe, $"x \"{ArchivePath}\" -o\"{tempFolder}\" -y").WaitForExit(1000*60*5);
                
                // Search and install mods from the temp folder after extracting.
                InstallFromFolder(tempFolder);

                // Deletes the temp folder with all of its contents.
                Directory.Delete(tempFolder, true);
                key.Close();
            }
            else
            {
                MessageBox.Show("Failed to install from archive because 7-Zip is not installed.",
                    Program.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        public static void InstallFromFolder(string folderPath)
        {
            // A List of folders that have mod.ini in them.
            var folders = new List<string>();
            foreach (string dirPath in Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories))
            {
                if (File.Exists(Path.Combine(dirPath, "mod.ini")))
                    folders.Add(dirPath);
            }

            // Checks if theres a folder with a mod inside of it.
            if(folders.Count > 0)
            {
                foreach(string folder in folders)
                {
                    // Creates all of the Directories.
                    foreach (string dirPath in Directory.GetDirectories(folder, "*", SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(folder, Path.Combine(MainForm.ModsFolderPath, Path.GetFileName(folder))));

                    // Copies all the files from the Directories.
                    foreach (string newPath in Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories))
                        File.Copy(newPath, newPath.Replace(folder, Path.Combine(MainForm.ModsFolderPath, Path.GetFileName(folder))), true);
                }
            }
            else
            {
                MessageBox.Show("Could not detect any mods in the selected folder.", Program.ProgramName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        private void AddModForm_Load(object sender, EventArgs e)
        {
            MainForm.ApplyDarkTheme(this);
        }
    }
}
