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
    public partial class NewModPropEditForm : Form
    {

        public ListViewItem selectedItem;

        public NewModPropEditForm(ListViewItem selectedItem)
        {
            InitializeComponent();
            this.selectedItem = selectedItem;
            textBox1.Tag = selectedItem.Tag;
            textBox1.Text = selectedItem.SubItems[1].Text;
            Text = $"Change Property {selectedItem.Text}";
            label1.Text = $"Change Property {selectedItem.Text} to: ";
        }

        private void NewModPropEditForm_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                selectedItem.SubItems[1].Text = textBox1.Text;
                DialogResult = DialogResult.OK;
                Close();
            }

            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
    }
}
