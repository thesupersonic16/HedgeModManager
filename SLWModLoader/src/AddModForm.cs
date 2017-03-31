using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using System.IO;
namespace SLWModLoader
{
    public partial class AddModForm : Form
    {
        public AddModForm()
        {
            InitializeComponent();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            Close();
            if(makingItRBtn.Checked)
            {
                NewModNameForm nmnf = new NewModNameForm();
                if (nmnf.ShowDialog() == DialogResult.OK)
                {
                    NewModForm nmf = new NewModForm(nmnf.getModName());
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
            var zip = ZipFile.OpenRead(ZipPath);
            var location = string.Empty;
            foreach (var entry in zip.Entries)
            {
                if (entry.FullName.EndsWith("mod.ini"))
                {
                    location = "/" + entry.FullName;
                    break;
                }
            }
            if (location.Length != 0)
            {
                // TODO: Needs a lot of work
                string folderName = location.LastIndexOf("/") != 0 ?
                    location.Substring(location.Substring(0, location.LastIndexOf("/") - 1).LastIndexOf("/"),
                    location.LastIndexOf("/") - location.Substring(0, location.LastIndexOf("/") - 1).LastIndexOf("/")).Substring(1) :
                    Path.GetFileNameWithoutExtension(ZipPath);
                string path = Path.Combine(MainForm.ModsFolderPath, folderName);
                Directory.CreateDirectory(path);
                foreach (var entry in zip.Entries)
                {
                    if (entry.FullName.StartsWith(location.Substring(1, location.LastIndexOf("/"))))
                    {
                        var filePath = Path.Combine(path, entry.FullName.Substring(location.Substring(1, location.LastIndexOf("/")).Length));
                        if (filePath.IndexOf(".") != -1)
                            entry.ExtractToFile(filePath);
                        else
                            Directory.CreateDirectory(filePath);
                    }
                }
            }
            else
            {
                MessageBox.Show("Could not find a mod inside the selected archive.", Program.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            zip.Dispose();
        }

        public static void InstallFromFolder(string FolderPath)
        {
            var folder = FolderPath;
            if (!File.Exists(Path.Combine(folder, "mod.ini")))
            {
                // Asks to continue if no mods has been found.
                if (MessageBox.Show("Could not find mod.ini in the selected folder\nDo you want to continue?", Program.ProgramName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    // Aborts the installation.
                    return;
                }
            }
            // Creates all of the Directories.
            foreach (string dirPath in Directory.GetDirectories(folder, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(folder, Path.Combine(MainForm.ModsFolderPath, Path.GetFileName(folder))));

            // Copies all the files from the Directories.
            foreach (string newPath in Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(folder, Path.Combine(MainForm.ModsFolderPath, Path.GetFileName(folder))), true);
        }
    }
}
