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
                textBox1.Text = listViewItem.Text;
                textBox2.Text = listViewItem.SubItems[1].Text;
                GroupComboBox.SelectedIndex = modform.getListView().Groups.IndexOf(listViewItem.Group);
                AddButton.Text = "Update";
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if(listViewItem != null)
            {
                listViewItem.Text = textBox1.Text;
                listViewItem.SubItems[1].Text = textBox2.Text;
                listViewItem.Group = modform.getListView().Groups[GroupComboBox.SelectedIndex];
            }
            else
                modform.AddProperty(textBox1.Text, textBox2.Text, GroupComboBox.SelectedIndex);
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
