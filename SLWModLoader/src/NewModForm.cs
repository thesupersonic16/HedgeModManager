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

        public NewModForm(string name)
        {
            InitializeComponent();
            modName = name;
            // Automatically fill in some info, I might remove this in the future
            listView1.Groups[1].Items[0].SubItems[1].Text = name; // Title
            listView1.Groups[1].Items[3].SubItems[1].Text = DateTime.Now.ToShortDateString(); // Date
            listView1.Groups[1].Items[4].SubItems[1].Text = Environment.UserName; // Author
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

        public void AddProperty(string propName, string PropValue, int PropGroup)
        {
            ListViewItem lvi = new ListViewItem();
            ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();
            lvsi.Text = PropValue;
            lvi.Text = propName;
            lvi.Group = listView1.Groups[PropGroup];
            lvi.SubItems.Add(lvsi);
            lvi.ForeColor = Color.FromArgb(255, 128, 0); // TODO: Use a different colour for user created properties
            lvi.Selected = true;
            listView1.Items.Add(lvi);
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            new NewModPropEditForm(listView1.FocusedItem).Show();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            new NewModPropEditForm(listView1.FocusedItem).Show();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(MainForm.ModsFolderPath, modName);
            Directory.CreateDirectory(filePath);
            IniFile iniFile = new IniFile();
            iniFile.AddGroup("Main");
            iniFile.AddGroup("Desc");
            
            // Adds all the properties into the ini file
            foreach (ListViewGroup lvg in listView1.Groups)
            {
                foreach (ListViewItem lvi in lvg.Items)
                {
                    iniFile[lvg.Header].AddParameter(lvi.Text, lvi.SubItems[1].Text);
                }
            }

            // Saves the ini file from memory
            iniFile.Save(Path.Combine(filePath, "mod.ini"));
            // Closes the Dialog
            DialogResult = DialogResult.OK;
            Close();
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            new NewModPropNewForm(this).Show();
        }

        private void rmvBtn_Click(object sender, EventArgs e)
        {
            if(listView1.Items.Count > 0 && listView1.FocusedItem != null)
            {
                listView1.Items.Remove(listView1.FocusedItem);
            }
        }
    }
}
