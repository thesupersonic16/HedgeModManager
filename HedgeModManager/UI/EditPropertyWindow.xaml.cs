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
    /// Interaction logic for EditPropertyWindow.xaml
    /// </summary>
    public partial class EditPropertyWindow : Window
    {
        protected PropInfo Instance;
        public EditPropertyWindow(PropInfo property)
        {
            InitializeComponent();
            Instance = property;
            Title = $"Edit {property.Name}";
            PropName.Content = Title;
            ValueBx.Text = Instance.ToString();
            if (property.PropertyInfo.PropertyType == typeof(string))
                ValueBx.AcceptsReturn = true;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var val = Convert.ChangeType(ValueBx.Text.Replace("\r\n", "\\n"), Instance.PropertyInfo.PropertyType);
                Instance.PropertyInfo.SetValue(Instance.Object, val);
                Instance.Notify();
                DialogResult = true;
            }
            catch
            {
                var box = new HedgeMessageBox("ERROR!", "Invalid input value");
                box.AddButton("OK", () => { box.Close(); });
                box.ShowDialog();
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            InvalidateVisual();
        }
    }
}
