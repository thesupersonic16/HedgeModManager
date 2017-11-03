using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SS16
{
    // !!! IGNORE IDE0007 WARNING !!!
    public static class Theme
    {

        // Theme Functions
        public static void AddAllChildControls(Control control, List<Control> controls)
        {
            controls.Add(control);
            foreach (Control control2 in control.Controls)
                AddAllChildControls(control2, controls);
        }

        /// <summary>
        /// Applies a dark theme to the given control
        /// </summary>
        /// <param name="control">The Main control apply the dark theme to</param>
        /// <param name="controls">Extra controls to apply the theme to</param>
        /// <returns>Only true</returns>
        public static bool ApplyDarkThemeToAll(Control control, params Control[] controls)
        {
            var allControls = new List<Control>();

            AddAllChildControls(control, allControls);

            foreach (var control0 in controls)
            {
                if (!allControls.Contains(control0)) allControls.Add(control0);
                foreach (Control control1 in control0.Controls)
                    if (!allControls.Contains(control1))
                        allControls.Add(control1);
            }
            return ApplyDarkTheme(allControls.ToArray());
        }

        public static bool ApplyDarkTheme(params Control[] controls)
        {
            foreach (var control0 in controls)
            {
                control0.BackColor = Color.FromArgb(46, 46, 46);
                if (control0.ForeColor == Color.Black ||
                    control0.ForeColor == SystemColors.WindowText ||
                    control0.ForeColor == SystemColors.ControlText)
                    control0.ForeColor = Color.FromArgb(200, 200, 180);

                if (control0.GetType() == typeof(Button))
                {
                    ((Button)control0).FlatStyle = FlatStyle.Flat;
                    control0.BackColor = Color.FromArgb(54, 54, 54);
                }

                if (control0.GetType() == typeof(RadioButton))
                    ((RadioButton)control0).FlatStyle = FlatStyle.Flat;

                if (control0.GetType() == typeof(StatusStrip) ||
                    control0.GetType() == typeof(MenuStrip) ||
                    control0.GetType() == typeof(GroupBox))
                    control0.BackColor = Color.FromArgb(54, 54, 54);

                if (control0.GetType() == typeof(TabPage) ||
                    control0.GetType() == typeof(LinkLabel) ||
                    control0.GetType() == typeof(CheckBox) ||
                    control0.GetType() == typeof(Label))
                    control0.BackColor = Color.FromArgb(46, 46, 46);

                if (control0 is GroupBox groupBox)
                    groupBox.ForeColor = Color.FromArgb(200, 200, 180);

                if (control0 is MenuStrip menuStrip)
                    menuStrip.ForeColor = Color.Green;

                if (control0 is ListView listView)
                {
                    listView.OwnerDraw = true;
                    listView.DrawColumnHeader += ListView_DrawColumnHeader;
                    listView.DrawItem += ListView_DrawItem;
                    int i = 0;
                    foreach (ListViewItem lvi in listView.Items)
                        if (++i % 2 == 0) lvi.BackColor = Color.FromArgb(46, 46, 46);
                        else lvi.BackColor = Color.FromArgb(54, 54, 54);
                }

            }
            return true;
        }

        private static void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            var listView = (ListView)sender;

            // Colours
            var dark1 = Color.FromArgb(34, 34, 34);
            var dark2 = Color.FromArgb(70, 70, 70);


            // Draws the Header
            if (e.Bounds.Contains(listView.PointToClient(Control.MousePosition)))
                e.Graphics.FillRectangle(new SolidBrush(dark1), e.Bounds);
            else e.Graphics.FillRectangle(new SolidBrush(dark2), e.Bounds);
            var point = new Point(0, 6);
            point.X = e.Bounds.X;
            var column = listView.Columns[e.ColumnIndex];
            e.Graphics.FillRectangle(new SolidBrush(dark1), point.X, 0, 2, e.Bounds.Height);
            point.X += column.Width / 2 - TextRenderer.MeasureText(column.Text, listView.Font).Width / 2;
            TextRenderer.DrawText(e.Graphics, column.Text, listView.Font, point, listView.ForeColor);
        }

        private static void ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }
    }
}
