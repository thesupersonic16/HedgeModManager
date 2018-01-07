using SS16;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace HedgeModManager
{
    public partial class CreateUpdateForm : Form
    {

        public List<FileItem> Items = new List<FileItem>();
        public Mod BaseMod, NewMod = null;

        public CreateUpdateForm()
        {
            InitializeComponent();
        }

        public CreateUpdateForm(Mod mod) : this()
        {
            TextBox_NewMod.Text = mod.FilePath;
            TextBox_NewMod.Enabled = false;
            TextBox_Version.Text = mod.Version;
        }

        public void RefreshFileList()
        {
            if (!(File.Exists(TextBox_NewMod.Text) && File.Exists(TextBox_BaseMod.Text)))
                return;

            try
            {
                BaseMod = new Mod(Path.GetDirectoryName(TextBox_BaseMod.Text));
                NewMod = new Mod(Path.GetDirectoryName(TextBox_NewMod.Text));

                Items.Clear();
                // Add all files
                foreach (string filePath in Directory.GetFiles(
                    NewMod.RootDirectory, "*.*", SearchOption.AllDirectories))
                {
                    var item = new FileItem();
                    item.FullPath = filePath;
                    item.RelativePath = filePath.Replace(NewMod.RootDirectory + "\\", "");
                    item.URL = $"http://localhost/{BaseMod.Title}/{item.RelativePath.Replace('\\', '/')}";
                    string BaseModFilePath =
                        filePath.Replace(NewMod.RootDirectory, BaseMod.RootDirectory);

                    // Checks if the file exists in the base folder
                    if (!File.Exists(BaseModFilePath))
                    {
                        Items.Add(item);
                        continue;
                    }

                    var modifiedBytes = File.ReadAllBytes(filePath);
                    var unModifiedBytes = File.ReadAllBytes(BaseModFilePath);
                    if (CompareByteArray(modifiedBytes, unModifiedBytes))
                        Items.Add(item);
                }
                UpdateListView();
            }
            catch { }
        }

        public void UpdateListView()
        {
            listView1.Items.Clear();
            foreach (var item in Items)
            {
                var lvi = new ListViewItem();
                lvi.SubItems[0].Text = item.RelativePath;
                lvi.SubItems.Add(new ListViewItem.ListViewSubItem());
                lvi.SubItems[1].Text = item.URL;
                lvi.Checked = true;
                lvi.Tag = item;
                listView1.Items.Add(lvi);
            }

            int i = 0;
            if (MainForm.CPKREDIRIni[Program.ProgramNameShort].ContainsParameter("DarkTheme") &&
                    MainForm.CPKREDIRIni[Program.ProgramNameShort]["DarkTheme"] == "1")
                foreach (ListViewItem lvi in listView1.Items)
                    if (++i % 2 == 0) lvi.BackColor = Color.FromArgb(46, 46, 46);
                    else lvi.BackColor = Color.FromArgb(54, 54, 54);

        }

        public bool CompareByteArray(byte[] bytes1, byte[] bytes2)
        {
            var sha256 = SHA256.Create();
            return !sha256.ComputeHash(bytes1).SequenceEqual(sha256.ComputeHash(bytes2));
        }

        public void CreateXMLUpdate(Mod modifiedMod, Mod unmodifiedMod, string outPath)
        {
            if (MessageBox.Show("NOTE: All files in the selected directory will be deleted", Program.ProgramName,
                    MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            var sha = new SHA256Managed();
            var root = new XElement("Update");

            // Recreates the directory
            if (Directory.Exists(outPath))
                Directory.Delete(outPath, true);
            Directory.CreateDirectory(outPath);

            try
            {
                // Creates all of the directories
                foreach (string path in Directory.GetDirectories(
                    modifiedMod.RootDirectory, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(path.Replace(modifiedMod.RootDirectory, outPath));

                // List of items that have been modified
                var items = new List<FileItem>();

                foreach (var item in Items)
                {
                    string unmodifiedFilePath = Path.Combine(unmodifiedMod.RootDirectory, item.RelativePath);

                    // Checks if the file exists in the unmodified folder
                    if (!File.Exists(unmodifiedFilePath))
                    {
                        items.Add(item);
                        continue;
                    }

                    var modifiedBytes = File.ReadAllBytes(item.FullPath);
                    var unModifiedBytes = File.ReadAllBytes(unmodifiedFilePath);
                    if (CompareByteArray(modifiedBytes, unModifiedBytes))
                        items.Add(item);
                }

                // UpdateInfo
                var updateInfo = new XElement("UpdateInfo", TextBox_Changelog.Text);
                updateInfo.Add(new XAttribute("version", modifiedMod.Version));
                updateInfo.Add(new XAttribute("downloadSize", textBox5.Text));

                // UpdateFiles
                var updateFiles = new XElement("UpdateFiles");

                foreach (var item in items)
                {
                    string sha256 = "";
                    using (var stream = File.OpenRead(item.FullPath))
                        sha256 = Program.ByteArrayToString(sha.ComputeHash(stream));

                    // UpdateFile
                    var updateFile = new XElement("UpdateFile");
                    updateFile.Add(new XElement("FilePath", item.RelativePath));
                    updateFile.Add(new XElement("URL", item.URL.Replace('\\', '/')));
                    updateFile.Add(new XElement("SHA256", sha256));
                    updateFiles.Add(updateFile);
                }

                // Copies all the modified files into the output directory
                foreach (var item in items)
                    File.Copy(item.FullPath, Path.Combine(outPath, item.RelativePath), true);

                // Saves the XML file
                root.Add(updateInfo);
                root.Add(updateFiles);
                var xml = new XDocument(root);
                xml.Save(Path.Combine(outPath, unmodifiedMod.Title + ".xml"));

                // Opens the output folder in explorer
                Process.Start("explorer", outPath);
            }
            catch (Exception ex)
            {
                MainForm.AddMessage("Exception thrown while creating update files", ex);
            }
        }
        
        #region GUIEvents

        private void CreateUpdateForm_Load(object sender, EventArgs e)
        {
            Theme.ApplyDarkThemeToAll(this);
        }

        private void Button_Generate_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(TextBox_Output.Text))
                CreateXMLUpdate(NewMod, BaseMod, TextBox_Output.Text);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int fileSize = 0;
            for (int i = 0; i < listView1.CheckedItems.Count; ++i)
            {
                var lvi = listView1.CheckedItems[i];
                fileSize += (int)new FileInfo(((FileItem)lvi.Tag).FullPath).Length;
            }
            if (fileSize > 1024 * 1024 * 1024)
                textBox5.Text = ((float)fileSize / (1024 * 1024 * 1024)).ToString("0.00") + "GB";
            else if (fileSize > 1024 * 1024)
                textBox5.Text = ((float)fileSize / (1024 * 1024)).ToString("0.00") + " MB";
            else if (fileSize > 1024)
                textBox5.Text = ((float)fileSize / 1024).ToString("0.00") + " KB";
            else
                textBox5.Text = fileSize + " Bytes";
        }

        private void TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (!((TextBox)sender).Enabled)
                return;
            var ofd = new OpenFileDialog()
            {
                Title = "Open Mod",
                Filter = "Mod|mod.ini"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
                ((TextBox)sender).Text = ofd.FileName;
        }

        private void TextBox_Output_DoubleClick(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog()
            {
                Title = Program.ProgramName,
                FileName = "Enter into a directory where you want all the update files to be saved," +
                " then press Save (All files in the selected directory will be deleted)"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                TextBox_Output.Text = Path.GetDirectoryName(saveFileDialog.FileName);
        }

        private void TextBox_NewMod_TextChanged(object sender, EventArgs e)
        {
            RefreshFileList();
        }

        private void ListView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                var item = listView1.SelectedItems[0].Tag as FileItem;
                new CreateUpdateURLForm(item).ShowDialog();
                listView1.SelectedItems[0].SubItems[1].Text = item.URL;
            }
        }

        private void ListView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void ListView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            // Colours
            var dark1 = Color.FromArgb(34, 34, 34);
            var dark2 = Color.FromArgb(70, 70, 70);

            // Draws the Header
            if (e.Bounds.Contains(listView1.PointToClient(MousePosition)))
                e.Graphics.FillRectangle(new SolidBrush(dark1), e.Bounds);
            else e.Graphics.FillRectangle(new SolidBrush(dark2), e.Bounds);
            var point = new Point(0, 6);
            point.X = e.Bounds.X;
            var column = listView1.Columns[e.ColumnIndex];
            e.Graphics.FillRectangle(new SolidBrush(dark1), point.X, 0, 2, e.Bounds.Height);
            point.X += column.Width / 2 - TextRenderer.MeasureText(column.Text, listView1.Font).Width / 2;
            TextRenderer.DrawText(e.Graphics, column.Text, listView1.Font, point, listView1.ForeColor);
        }

        #endregion GUIEvents

        private void Button_SetHostName_Click(object sender, EventArgs e)
        {
            foreach (var item in Items)
            {
                int index = item.URL.IndexOf("//") + 2;
                int hostNameIndexAfter = item.URL.Substring(index).IndexOf("/") + index;
                item.URL = item.URL.Substring(0, index) + textBox1.Text + 
                    item.URL.Substring(hostNameIndexAfter);
            }
            UpdateListView();
        }

        public class FileItem
        {
            public string FullPath, RelativePath, URL;
        }

    }
}
