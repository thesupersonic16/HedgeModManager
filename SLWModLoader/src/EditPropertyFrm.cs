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
    public partial class EditPropertyFrm : Form
    {
        public bool cancelled = true;

        public EditPropertyFrm(string name, string value, string type, string group, ListViewGroupCollection groups)
        {
            InitializeComponent();

            foreach (ListViewGroup definedgroup in groups)
            {
                groupCombobx.Items.Add(definedgroup);
            }

            nameTxtbx.Text = name;
            groupCombobx.Text = group;
            typeCombobx.Text = type;
            valueCombobx.Text = value;

            if (type == "Integer")
            {
                int i = 0;
                valueNumericUpDown.Value = (int.TryParse(value, out i)) ? Convert.ToInt32(value) : 0;
            }
            else
            {
                valueCombobx.Text = value;
            }

        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            cancelled = false;
            Close();
        }

        private void typeCombobx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (typeCombobx.Text == "Integer")
            {
                valueCombobx.Visible = false;
                valueNumericUpDown.Visible = true;
                int i = 0;
                valueNumericUpDown.Value = (int.TryParse(valueCombobx.Text, out i)) ? Convert.ToInt32(valueCombobx.Text) : 0;
            }
            else if (typeCombobx.Text == "Boolean")
            {
                valueCombobx.Visible = true;
                valueNumericUpDown.Visible = false;

                valueCombobx.Items.Clear();
                valueCombobx.DropDownStyle = ComboBoxStyle.DropDownList;
                valueCombobx.Items.Add("True"); valueCombobx.Items.Add("False");
                valueCombobx.Text = "True";
            }
            else
            {
                valueCombobx.Visible = true;
                valueNumericUpDown.Visible = false;

                valueCombobx.Items.Clear();
                valueCombobx.DropDownStyle = ComboBoxStyle.DropDown;
                valueCombobx.Text = valueNumericUpDown.Value.ToString();
            }
        }
    }
}
