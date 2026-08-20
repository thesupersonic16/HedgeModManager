using System;
using System.Collections.Generic;
using System.IO;
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

using static HedgeModManager.Lang;
using Path = System.IO.Path;

namespace HedgeModManager.UI
{
    /// <summary>
    /// Interaction logic for ModConfigWindow.xaml
    /// </summary>
    public partial class ModConfigWindow : Window
    {
        protected ModInfo Mod;
        protected FormSchema Schema;
        public ModConfigWindow()
        {
            InitializeComponent();
        }

        public ModConfigWindow(ModInfo mod)
        {
            InitializeComponent();
            Mod = mod;
            Schema = mod.ConfigSchema;
            var path = Path.Combine(Mod.RootDirectory, Schema.IniFile);
            if (File.Exists(path))
                Schema.TryLoad(mod);

            Title = LocaliseFormat("ModConfigUITitle", Mod.Title);
            var panel = FormBuilder.Build(mod.ConfigSchema, OnItemHover);
            panel.HorizontalAlignment = HorizontalAlignment.Stretch;
            panel.VerticalAlignment = VerticalAlignment.Stretch;
            ItemsHost.Children.Add(panel);
        }

        private void OnItemHover(string des)
        {
            DescriptionBx.Text = des;
        }

        private void UI_OK_Click(object sender, RoutedEventArgs e)
        {
            Schema.SaveIni(Path.Combine(Mod.RootDirectory, Schema.IniFile));
            DialogResult = true;
        }

        private void UI_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
