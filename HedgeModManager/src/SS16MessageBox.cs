using SS16;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SS16
{
    public partial class SS16MessageBox : Form
    {

        protected int NextButtonX = -1;
        protected int ButtonSpacing = 20;

        public SS16MessageBox()
        {
            InitializeComponent();
        }

        public SS16MessageBox(string windowTitle, string title, string message)
        {
            InitializeComponent();
            Text = windowTitle;
            Label_Title.Text = title;
            Label_Message.Text = message;
        }

        public void AddButton(string text, int width, EventHandler onClick)
        {
            if (NextButtonX == -1)
                NextButtonX = Width - ButtonSpacing * 2;
            var button = new Button();
            // Sets the text on the button
            button.Text = text;
            // Adds the OnClick Handler
            button.Click += onClick;
            // Sets the Button's Location
            button.Location = new Point(NextButtonX - width, Height - 80 - ButtonSpacing);
            // Sets the Button's Size
            button.Size = new Size(width, 40);
            // Adds the Button to the window
            Controls.Add(button);
            // Prepares for the Next Button
            NextButtonX -= width + ButtonSpacing;
        }

        private void SS16MessageBox_Load(object sender, EventArgs e)
        {
            // Sets the With of the Window
            Width = Label_Title.Width;
            if (Label_Message.Width > Width)
                Width = Label_Message.Width;
            Width += 40;
            // Centres the Title Label
            Label_Title.Location = new Point(
                Size.Width / 2 - Label_Title.Size.Width / 2, Label_Title.Location.Y);
            // Applies the dark theme
            Theme.ApplyDarkThemeToAll(this);
        }
    }
}
