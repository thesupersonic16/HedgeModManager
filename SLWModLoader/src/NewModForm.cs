using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLWModLoader
{
    public partial class NewModForm : Form
    {
        public string modName = string.Empty;
        public Mod mod;

        // Create a new mod.
        public NewModForm(string name)
        {
            InitializeComponent();
            modName = name;
            // Automatically fill in some info.
            listView1.Groups[1].Items[0].SubItems[1].Text = name; // Title
            listView1.Groups[1].Items[3].SubItems[1].Text = DateTime.Now.ToShortDateString(); // Date
            listView1.Groups[1].Items[4].SubItems[1].Text = Environment.UserName; // Author
        }

        // Edit a mod.
        public NewModForm(Mod mod)
        {
            InitializeComponent();
            this.mod = mod;
            modName = Path.GetFileName(mod.RootDirectory);
            // Automatically fill in some info.
            var ini = mod.GetIniFile();

            for (int i = 0; i < ini.GroupCount; ++i)
            {
                var iniGroup = ini[i];

                ListViewGroup group = null;
                foreach (ListViewGroup group2 in listView1.Groups)
                {
                    if(group2.Header.ToLower().Equals(iniGroup.GroupName.ToLower()))
                    {
                        group = group2;
                        break;
                    }
                }

                if (group == null)
                    group = AddGroup(iniGroup.GroupName);

                for (int i2 = 0; i2 < iniGroup.ParameterCount; ++i2)
                {
                    string key = iniGroup[i2].Key.Replace("\n", "\\n");
                    string value = iniGroup[i2].Value.Replace("\n", "\\n");
                    bool hasProperty = false;
                    foreach (ListViewItem lvi in group.Items)
                    {
                        if (lvi.SubItems[0].Text.ToLower().Equals(key.ToLower()))
                        {
                            lvi.SubItems[1].Text = value;
                            hasProperty = true;
                        }
                    }
                    if (!hasProperty)
                        AddProperty(key, value, listView1.Groups.IndexOf(group), "String");
                }

            }
        }


        //Definitely needs a rewrite, unsure if adding support for IniFile.cs would help 

        #region Old code
        //private string name = "";

        //public DevNewModFrm(string name)
        //{
        //    InitializeComponent();
        //    this.name = name;
        //    listView1.Items[3].SubItems[1].Text = name;
        //    listView1.Items[6].SubItems[1].Text = $"{DateTime.Now.Date.Month.ToString()}/{DateTime.Now.Date.Day}/{DateTime.Now.Date.Year}";
        //    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        //}

        //protected override bool ProcessDialogKey(Keys key)
        //{
        //    if (ModifierKeys == Keys.None && key == Keys.Delete)
        //    {
        //        rmvBtn.PerformClick();
        //    }
        //    return base.ProcessDialogKey(key);
        //}

        //private void listView1_DoubleClick(object sender, EventArgs e)
        //{
        //    if (listView1.SelectedItems.Count > 0)
        //    {
        //        EditPropertyFrm editpfrm = new EditPropertyFrm(listView1.SelectedItems[0].SubItems[0].Text, (listView1.SelectedItems[0].SubItems.Count > 1) ? listView1.SelectedItems[0].SubItems[1].Text : "", (string)listView1.SelectedItems[0].Tag,listView1.SelectedItems[0].Group.Header,listView1.Groups);
        //        editpfrm.ShowDialog();

        //        if (!editpfrm.cancelled)
        //        {
        //            listView1.SelectedItems[0].SubItems[0].Text = editpfrm.nameTxtBx.Text;
        //            listView1.SelectedItems[0].Tag = editpfrm.typeComboBx.Text;

        //            if (listView1.SelectedItems[0].SubItems.Count > 1) { listView1.SelectedItems[0].SubItems[1].Text = ((editpfrm.typeComboBx.Text != "Integer") ?editpfrm.valueComboBx.Text:editpfrm.valueNud.Value.ToString()); }
        //            else { listView1.SelectedItems[0].SubItems.Add(editpfrm.valueComboBx.Text); }

        //            listView1.SelectedItems[0].ForeColor = (string.IsNullOrEmpty(editpfrm.valueComboBx.Text)) ? (editpfrm.nameTxtBx.Text == "IncludeDir0" || editpfrm.nameTxtBx.Text == "IncludeDirCount" || editpfrm.nameTxtBx.Text == "Title" || editpfrm.nameTxtBx.Text == "Description" || editpfrm.nameTxtBx.Text == "Author") ? Color.Red : Color.Orange : Color.Black;
        //            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

        //            foreach (ListViewGroup group in listView1.Groups)
        //            {
        //                if (group.Header == editpfrm.groupComboBx.Text)
        //                {
        //                    listView1.SelectedItems[0].Group = group;
        //                    return;
        //                }
        //            }

        //            listView1.Groups.Add(new ListViewGroup(editpfrm.groupComboBx.Text));
        //            listView1.SelectedItems[0].Group = listView1.Groups[listView1.Groups.Count - 1];
        //        }
        //    }
        //}

        //private void rmvBtn_Click(object sender, EventArgs e)
        //{
        //    if (listView1.SelectedItems.Count > 0)
        //    {
        //        listView1.Items.RemoveAt(listView1.SelectedItems[0].Index);
        //    }
        //}

        //private void addBtn_Click(object sender, EventArgs e)
        //{
        //    listView1.Items.Add(new ListViewItem("New Property", listView1.Groups[0]) { ForeColor = Color.Orange, Tag = "String" });
        //    listView1.Items[listView1.Items.Count - 1].Selected = true;
        //    editBtn.PerformClick();
        //}

        //private void okBtn_Click(object sender, EventArgs e)
        //{
        //    bool doclose = true; List<string> modini = new List<string>();
        //    DialogResult dr = DialogResult.None;

        //    foreach (ListViewGroup group in listView1.Groups)
        //    {
        //        if (!modini.Contains($"[{group.Header}]"))
        //        {
        //            modini.Add($"[{group.Header}]");
        //        }

        //        foreach (ListViewItem property in listView1.Items)
        //        {
        //            if (string.IsNullOrEmpty(property.Text) || property.SubItems.Count <= 1 || string.IsNullOrEmpty(property.SubItems[1].Text) || property.Name.Contains('=') || property.SubItems[1].Text.Contains('='))
        //            {
        //                if (dr == DialogResult.None)
        //                {
        //                    dr = MessageBox.Show("One or more of the given properties seem to be empty/invalid! Would you like to keep them anyway?", "SLW Mod Loader", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2);
        //                    doclose = (dr != DialogResult.Cancel);
        //                    if (!doclose) { return; }
        //                }
        //                if (dr == DialogResult.No)
        //                {
        //                    continue;
        //                }
        //            }

        //            if (property.Group.Header == group.Header)
        //            {
        //                modini.Add($"{property.Text}={(((string)property.Tag != "Integer")?"\"":"")}{((property.SubItems.Count > 1)?property.SubItems[1].Text:"")}{(((string)property.Tag != "Integer")?"\"":"")}");
        //            }
        //        }
        //        if (modini[modini.Count-1] == $"[{group.Header}]") { modini.RemoveAt(modini.Count-1); }
        //    }

        //    if (doclose)
        //    {
        //        Console.WriteLine(Mainfrm.slwdirectory + "\\mods\\" + name);
        //        string dirname = name;
        //        foreach (char c in new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))
        //        {
        //            dirname = dirname.Replace(c.ToString(), "");
        //        }
        //        dirname = Mainfrm.slwdirectory + "\\mods\\" + dirname;

        //        if (Directory.Exists(dirname) && MessageBox.Show($"A mod already exists in the \"{name}\" folder. Would you like to delete it and replace it with this one?","SLW Mod Loader",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes)
        //        {
        //            Directory.Delete(dirname,true);
        //        }
        //        else if (Directory.Exists(dirname)) { return; }
        //        Directory.CreateDirectory(dirname);
        //        File.WriteAllLines(dirname+"\\mod.ini", modini);

        //        Directory.CreateDirectory(dirname + "\\disk\\");
        //        Directory.CreateDirectory(dirname + "\\disk\\sonic2013_patch_0\\");
        //        Process.Start(dirname + "\\disk\\sonic2013_patch_0\\");

        //        Mainfrm.RefreshModList();

        //        Close();
        //    }
        //}
        #endregion

        public void AddProperty(string propName, string propValue, int propGroup, string propTag)
        {
            var lvi = new ListViewItem();
            var lvsi = new ListViewItem.ListViewSubItem();
            lvsi.Text = propValue;
            lvi.Tag = propTag;
            lvi.Text = propName;
            lvi.Group = listView1.Groups[propGroup];
            lvi.SubItems.Add(lvsi);
            lvi.ForeColor = Color.FromArgb(255, 128, 0); // TODO: Use a different colour for user created properties
            lvi.Selected = true;
            listView1.Items.Add(lvi);
        }

        public ListViewGroup AddGroup(string name)
        {
            var lvg = new ListViewGroup(name);
            listView1.Groups.Add(lvg);
            return lvg;
        }

        public ListView GetListView()
        {
            return listView1;
        }

        private void EditBtn_Click(object sender, EventArgs e)
        {
            new NewModPropNewForm(this, listView1.FocusedItem).Show();
        }

        private void ListView1_DoubleClick(object sender, EventArgs e)
        {
            new NewModPropNewForm(this, listView1.FocusedItem).Show();
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(MainForm.ModsFolderPath, modName);
            Directory.CreateDirectory(filePath);
            var iniFile = new IniFile();

            // Adds all the groups.
            foreach (ListViewGroup group in listView1.Groups)
                if(group.Items.Count > 0)
                    iniFile.AddGroup(group.Header);
            
            // Adds all the properties into the ini file.
            foreach (ListViewGroup lvg in listView1.Groups)
            {
                if (lvg.Items.Count == 0)
                    continue;
                foreach (ListViewItem lvi in lvg.Items)
                {
                    if (lvi.SubItems[1].Text.Length > 0)
                    {
                        iniFile[lvg.Header].AddParameter(lvi.Text, lvi.SubItems[1].Text);

                        if (lvi.Text.ToLower().Equals("savefile"))
                        {
                            if (!File.Exists(Path.Combine(filePath, lvi.SubItems[1].Text)) &&
                                MessageBox.Show("Would you like to create a save file?", Program.ProgramName,
                                MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                try
                                {
                                    // Writes an empty file.
                                    File.WriteAllBytes(Path.Combine(filePath, lvi.SubItems[1].Text), new byte[0]);
                                }
                                catch (Exception ex)
                                {
                                    MainForm.AddMessage("Exception thrown while creating a save file.", ex,
                                        $"Save File Location: {lvi.SubItems[1].Text}");
                                    MessageBox.Show("Failed to create save file. You'll have to create one manually.",
                                        Program.ProgramName);
                                }
                            }
                        }
                    }
                }
            }

            // Saves the ini file from memory.
            iniFile.Save(Path.Combine(filePath, "mod.ini"));
            // Closes the Dialog
            DialogResult = DialogResult.OK;
            Close();
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            new NewModPropNewForm(this, null).Show();
        }

        private void RmvBtn_Click(object sender, EventArgs e)
        {
            if(listView1.Items.Count > 0 && listView1.FocusedItem != null)
                listView1.Items.Remove(listView1.FocusedItem);
        }

        private void ListView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Checks if there is a selected Item.
            if(listView1.FocusedItem != null)
            // Deletes the focused item if the user presses the Delete key.
            if (e.KeyChar == (char)46)
            {
                listView1.Items.Remove(listView1.FocusedItem);
            }
        }

        private void NewModForm_Load(object sender, EventArgs e)
        {
            if(MainForm.ApplyDarkTheme(this))
            {
                foreach(ListViewItem lvi in listView1.Items)
                {
                    if(lvi.ForeColor == Color.Black)
                    {
                        lvi.ForeColor = Color.FromArgb(200, 200, 180);
                        lvi.SubItems[0].ForeColor = Color.FromArgb(200, 200, 180);
                    }
                }
            }
        }

        private void ListView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            // Colours
            var dark1 = Color.FromArgb(34, 34, 34);
            var dark2 = Color.FromArgb(70, 70, 70);

            // Draws the Header.
            if (e.Bounds.Contains(listView1.PointToClient(MousePosition)))
                e.Graphics.FillRectangle(new SolidBrush(dark1), e.Bounds);
            else e.Graphics.FillRectangle(new SolidBrush(dark2), e.Bounds);
            var point = new Point(0, 6);
            point.X = e.Bounds.X;
            var col = listView1.Columns[e.ColumnIndex];
            e.Graphics.FillRectangle(new SolidBrush(dark1), point.X, 0, 2, e.Bounds.Height);
            point.X += col.Width / 2 - TextRenderer.MeasureText(col.Text, listView1.Font).Width / 2;
            TextRenderer.DrawText(e.Graphics, col.Text, listView1.Font, point, listView1.ForeColor);
        }

        private void ListView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }
    }
}
