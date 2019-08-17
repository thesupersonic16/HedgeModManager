using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for EditModWindow.xaml
    /// </summary>
    public partial class EditModWindow : Window
    {
        protected ModInfo Mod;

        public EditModWindow(ModInfo mod)
        {
            InitializeComponent();
            Title = string.IsNullOrEmpty(mod.Title) ? "Hedge Mod Manager" : $"Edit {mod.Title}"; 
            Mod = mod;
            Editor.Instance = Mod;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Mod.Title))
            {
                var box = new HedgeMessageBox("ERROR!", "Invalid title");
                box.AddButton("Ok", () => { box.Close(); });
                box.ShowDialog();
                return;
            }
            DialogResult = true;
        }
    }
}
