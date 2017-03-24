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

        public NewModPropNewForm(NewModForm modform)
        {
            InitializeComponent();
            this.modform = modform;
            Text = "New Property";
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            modform.AddProperty(textBox1.Text, textBox2.Text, GroupComboBox.SelectedIndex);
            Close();
        }
    }
}
