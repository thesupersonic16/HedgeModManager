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
    public partial class CreateUpdateURLForm : Form
    {
        public CreateUpdateForm.FileItem Item;

        public CreateUpdateURLForm(CreateUpdateForm.FileItem item)
        {
            InitializeComponent();
            Item = item;
            TextBox_URL.Text = item.URL;
        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
            Item.URL = TextBox_URL.Text;
        }

        private void CreateUpdateURLForm_Load(object sender, EventArgs e)
        {
            MainForm.ApplyDarkTheme(this);
        }
    }
}
