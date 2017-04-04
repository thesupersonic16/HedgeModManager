using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLWModLoader
{
    public partial class NewModPropNewForm : Form
    {

        public NewModForm modform;
        public ListViewItem listViewItem;

        public NewModPropNewForm(NewModForm modform, ListViewItem listViewItem)
        {
            InitializeComponent();
            this.modform = modform;
            this.listViewItem = listViewItem;
            Text = "New Property";
            if(listViewItem != null)
            {
                Text = "Edit Property";
                label1.Text = "Edit Property: " + listViewItem.Text;
                TypeComboBox.Text = listViewItem.Tag as string;
                textBox1.Text = listViewItem.Text;
                GroupComboBox.SelectedIndex = modform.getListView().Groups.IndexOf(listViewItem.Group);
                AddButton.Text = "Update";
                switch (listViewItem.Tag)
                {
                    case "Integer":
                        ValueTextBox.Visible = false;
                        ValueNumericUpDown.Visible = true;
                        ValueCheckBox.Visible = false;
                        if (listViewItem.SubItems[1].Text.Length > 0) ValueNumericUpDown.Value = int.Parse(listViewItem.SubItems[1].Text);
                        break;
                    case "Boolean":
                        ValueTextBox.Visible = false;
                        ValueNumericUpDown.Visible = false;
                        ValueCheckBox.Visible = true;
                        if (listViewItem.SubItems[1].Text.Length > 0) ValueCheckBox.Checked = bool.Parse(listViewItem.SubItems[1].Text);
                        break;
                    default:
                        ValueTextBox.Text = listViewItem.SubItems[1].Text;
                        break;
                }
            }
        }

        public void UpdateType()
        {
            switch (TypeComboBox.Text)
            {
                case "Integer":
                    ValueTextBox.Visible = false;
                    ValueNumericUpDown.Visible = true;
                    ValueCheckBox.Visible = false;
                    break;
                case "Boolean":
                    ValueTextBox.Visible = false;
                    ValueNumericUpDown.Visible = false;
                    ValueCheckBox.Visible = true;
                    break;
                default:
                    ValueTextBox.Visible = true;
                    ValueNumericUpDown.Visible = false;
                    ValueCheckBox.Visible = false;
                    break;
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if(listViewItem != null)
            {
                switch (TypeComboBox.Text)
                {
                    case "Integer":
                        listViewItem.SubItems[1].Text = ValueNumericUpDown.Value.ToString();
                        break;
                    case "Boolean":
                        listViewItem.SubItems[1].Text = ValueCheckBox.Checked.ToString();
                        break;
                    default:
                        listViewItem.SubItems[1].Text = ValueTextBox.Text;
                        break;
                }
                listViewItem.Text = textBox1.Text;
                listViewItem.Tag = TypeComboBox.Text;
                listViewItem.Group = modform.getListView().Groups[GroupComboBox.SelectedIndex];
            }
            else
            {
                switch (TypeComboBox.Text)
                {
                    case "Integer":
                        modform.AddProperty(textBox1.Text, ValueNumericUpDown.Value.ToString(), GroupComboBox.SelectedIndex, TypeComboBox.Text);
                        break;
                    case "Boolean":
                        modform.AddProperty(textBox1.Text, ValueCheckBox.Checked.ToString(), GroupComboBox.SelectedIndex, TypeComboBox.Text);
                        break;
                    default:
                        modform.AddProperty(textBox1.Text, ValueTextBox.Text, GroupComboBox.SelectedIndex, TypeComboBox.Text);
                        break;
                }
            }

            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateType();
        }

        private void ValueTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Set property if the user presses Enter.
            if (e.KeyChar == (char)13)
            {
                AddButton_Click(null, null);
            }
        }
    }
}
