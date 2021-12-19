using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using PropertyTools.DataAnnotations;
using PropertyTools.Wpf;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for EditModWindow.xaml
    /// </summary>
    public partial class EditModWindow : Window
    {
        protected ModInfo Mod { get; set; }

        public EditModWindow(ModInfo mod)
        {
            Mod = mod;
            Mod.IncludeDirsProperty.Clear();
            foreach (var dir in Mod.IncludeDirs)
                Mod.IncludeDirsProperty.Add(new StringWrapper(dir));

            InitializeComponent();
            Title = string.IsNullOrEmpty(mod.Title) ? "Hedge Mod Manager" : $"Edit {mod.Title}";
            Editor.TabVisibility = TabVisibility.Collapsed;
            Editor.ControlFactory = new PropertyGridControlFactoryEx();
            Editor.DataContext = Mod;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Mod.Title))
            {
                var box = new HedgeMessageBox("ERROR!", "Invalid title");
                box.AddButton("OK", () => { box.Close(); });
                box.ShowDialog();
                return;
            }
            Mod.IncludeDirs.Clear();
            foreach (var dir in Mod.IncludeDirsProperty)
                Mod.IncludeDirs.Add(dir.Value);

            DialogResult = true;
        }
    }

    public class PropertyGridControlFactoryEx : PropertyGridControlFactory
    {
        public Thickness ControlMargin = new Thickness(0, 5, 0, 0);
        public override FrameworkElement CreateControl(PropertyItem propertyItem, PropertyControlFactoryOptions options)
        {
            var control = base.CreateControl(propertyItem, options);
            control.Margin = ControlMargin;
            return control;
        }
    }
}
