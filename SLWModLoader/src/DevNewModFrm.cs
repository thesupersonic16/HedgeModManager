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
    public partial class DevNewModFrm : Form
    {
        private string name = "";

        public DevNewModFrm(string name)
        {
            InitializeComponent();
            this.name = name;
            listView1.Items[3].SubItems[1].Text = name;
            listView1.Items[6].SubItems[1].Text = $"{DateTime.Now.Date.Month.ToString()}/{DateTime.Now.Date.Day}/{DateTime.Now.Date.Year}";
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        protected override bool ProcessDialogKey(Keys key)
        {
            if (ModifierKeys == Keys.None && key == Keys.Delete)
            {
                rmvBtn.PerformClick();
            }
            return base.ProcessDialogKey(key);
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                EditPropertyFrm editpfrm = new EditPropertyFrm(listView1.SelectedItems[0].SubItems[0].Text, (listView1.SelectedItems[0].SubItems.Count > 1) ? listView1.SelectedItems[0].SubItems[1].Text : "", (string)listView1.SelectedItems[0].Tag,listView1.SelectedItems[0].Group.Header,listView1.Groups);
                editpfrm.ShowDialog();

                if (!editpfrm.cancelled)
                {
                    listView1.SelectedItems[0].SubItems[0].Text = editpfrm.nameTxtbx.Text;
                    listView1.SelectedItems[0].Tag = editpfrm.typeCombobx.Text;

                    if (listView1.SelectedItems[0].SubItems.Count > 1) { listView1.SelectedItems[0].SubItems[1].Text = ((editpfrm.typeCombobx.Text != "Integer") ?editpfrm.valueCombobx.Text:editpfrm.valueNumericUpDown.Value.ToString()); }
                    else { listView1.SelectedItems[0].SubItems.Add(editpfrm.valueCombobx.Text); }

                    listView1.SelectedItems[0].ForeColor = (string.IsNullOrEmpty(editpfrm.valueCombobx.Text)) ? (editpfrm.nameTxtbx.Text == "IncludeDir0" || editpfrm.nameTxtbx.Text == "IncludeDirCount" || editpfrm.nameTxtbx.Text == "Title" || editpfrm.nameTxtbx.Text == "Description" || editpfrm.nameTxtbx.Text == "Author") ? Color.Red : Color.Orange : Color.Black;
                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                    foreach (ListViewGroup group in listView1.Groups)
                    {
                        if (group.Header == editpfrm.groupCombobx.Text)
                        {
                            listView1.SelectedItems[0].Group = group;
                            return;
                        }
                    }

                    listView1.Groups.Add(new ListViewGroup(editpfrm.groupCombobx.Text));
                    listView1.SelectedItems[0].Group = listView1.Groups[listView1.Groups.Count - 1];
                }
            }
        }

        private void rmvBtn_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                listView1.Items.RemoveAt(listView1.SelectedItems[0].Index);
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            listView1.Items.Add(new ListViewItem("New Property", listView1.Groups[0]) { ForeColor = Color.Orange, Tag = "String" });
            listView1.Items[listView1.Items.Count - 1].Selected = true;
            editBtn.PerformClick();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            bool doclose = true; List<string> modini = new List<string>();
            DialogResult dr = DialogResult.None;

            foreach (ListViewGroup group in listView1.Groups)
            {
                if (!modini.Contains($"[{group.Header}]"))
                {
                    modini.Add($"[{group.Header}]");
                }

                foreach (ListViewItem property in listView1.Items)
                {
                    if (string.IsNullOrEmpty(property.Text) || property.SubItems.Count <= 1 || string.IsNullOrEmpty(property.SubItems[1].Text) || property.Name.Contains('=') || property.SubItems[1].Text.Contains('='))
                    {
                        if (dr == DialogResult.None)
                        {
                            dr = MessageBox.Show("One or more of the given properties seem to be empty/invalid! Would you like to keep them anyway?", "SLW Mod Loader", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2);
                            doclose = (dr != DialogResult.Cancel);
                            if (!doclose) { return; }
                        }
                        if (dr == DialogResult.No)
                        {
                            continue;
                        }
                    }

                    if (property.Group.Header == group.Header)
                    {
                        modini.Add($"{property.Text}={(((string)property.Tag != "Integer")?"\"":"")}{((property.SubItems.Count > 1)?property.SubItems[1].Text:"")}{(((string)property.Tag != "Integer")?"\"":"")}");
                    }
                }
                if (modini[modini.Count-1] == $"[{group.Header}]") { modini.RemoveAt(modini.Count-1); }
            }
            
            if (doclose)
            {
                Console.WriteLine(Mainfrm.slwdirectory + "\\mods\\" + name);
                string dirname = name;
                foreach (char c in new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))
                {
                    dirname = dirname.Replace(c.ToString(), "");
                }
                dirname = Mainfrm.slwdirectory + "\\mods\\" + dirname;

                if (Directory.Exists(dirname) && MessageBox.Show($"A mod already exists in the \"{name}\" folder. Would you like to delete it and replace it with this one?","SLW Mod Loader",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Directory.Delete(dirname,true);
                }
                else if (Directory.Exists(dirname)) { return; }
                Directory.CreateDirectory(dirname);
                File.WriteAllLines(dirname+"\\mod.ini", modini);

                Directory.CreateDirectory(dirname + "\\disk\\");
                Directory.CreateDirectory(dirname + "\\disk\\sonic2013_patch_0\\");
                Process.Start(dirname + "\\disk\\sonic2013_patch_0\\");

                Mainfrm.RefreshModList();

                Close();
            }
        }
    }
}
