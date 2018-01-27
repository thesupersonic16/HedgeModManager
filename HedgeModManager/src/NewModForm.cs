using SS16;
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

namespace HedgeModManager
{
    public partial class NewModForm : Form
    {
        public string ModName = string.Empty;
        public Mod _Mod;

        // Create a new mod.
        public NewModForm(string name)
        {
            InitializeComponent();
            ModName = name;
            // Automatically fill in some info.
            listView1.Groups[1].Items[0].SubItems[1].Text = name; // Title
            listView1.Groups[1].Items[3].SubItems[1].Text = DateTime.Now.ToShortDateString(); // Date
            listView1.Groups[1].Items[4].SubItems[1].Text = Environment.UserName; // Author
        }

        // Edit a mod.
        public NewModForm(Mod mod)
        {
            InitializeComponent();
            _Mod = mod;
            ModName = Path.GetFileName(mod.RootDirectory);
            // Automatically fill in some info.
            var ini = mod.GetIniFile();

            for (int i = 0; i < ini.GroupCount; ++i)
            {
                var iniGroup = ini[i];

                ListViewGroup group = null;
                foreach (ListViewGroup group2 in listView1.Groups)
                {
                    if (group2.Header.ToLower().Equals(iniGroup.GroupName.ToLower()))
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
        
        public void AddProperty(string propName, string propValue, int propGroup, string propTag)
        {
            var lvi = new ListViewItem();
            var lvsi = new ListViewItem.ListViewSubItem();
            lvsi.Text = propValue.Replace("\n", "\\n").Replace("\r", "");
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
            string filePath = Path.Combine(MainForm.ModsFolderPath, ModName);
            Directory.CreateDirectory(filePath);
            var iniFile = new IniFile();

            // Adds all the groups.
            foreach (ListViewGroup group in listView1.Groups)
                if (group.Items.Count > 0)
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
                                MessageBox.Show($"Would you like {Program.ProgramName} to create a save file?",
                                Program.ProgramName, MessageBoxButtons.YesNo) == DialogResult.Yes)
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
            if (listView1.Items.Count > 0 && listView1.FocusedItem != null)
                listView1.Items.Remove(listView1.FocusedItem);
        }

        private void ListView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Checks if there is a selected Item.
            if (listView1.FocusedItem != null)
            // Deletes the focused item if the user presses the Delete key.
            if (e.KeyChar == (char)46)
            {
                listView1.Items.Remove(listView1.FocusedItem);
            }
        }

        private void NewModForm_Load(object sender, EventArgs e)
        {
            if (Program.UseDarkTheme)

            if (Theme.ApplyDarkThemeToAll(this))
            {
                foreach (ListViewItem lvi in listView1.Items)
                {
                    if (lvi.ForeColor == Color.Black)
                    {
                        lvi.ForeColor = Color.FromArgb(200, 200, 180);
                        lvi.SubItems[0].ForeColor = Color.FromArgb(200, 200, 180);
                    }
                }
                // This is here to make it easier to see the groups
                listView1.BackColor = Color.FromArgb(64, 64, 64);
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
            var column = listView1.Columns[e.ColumnIndex];
            e.Graphics.FillRectangle(new SolidBrush(dark1), point.X, 0, 2, e.Bounds.Height);
            point.X += column.Width / 2 - TextRenderer.MeasureText(column.Text, listView1.Font).Width / 2;
            TextRenderer.DrawText(e.Graphics, column.Text, listView1.Font, point, listView1.ForeColor);
        }

        private void ListView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }
    }
}
