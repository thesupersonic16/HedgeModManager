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
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow(string header, params string[] options)
        {
            InitializeComponent();
            Header.Text = header;
            foreach(var option in options)
            {
                CheckStack.Children.Add(new RadioButton() { Content = option, Margin = new Thickness(2) });
            }
        }

        public int Ask()
        {
           ShowDialog();
            if (DialogResult == false)
                return -1;

            return GetSelected();
        }

        private int GetSelected()
        {
            for (int i = 0; i < CheckStack.Children.Count; i++)
            {
                if (((RadioButton)CheckStack.Children[i]).IsChecked.Value)
                {
                    return i;
                }
            }
            return -1;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if(GetSelected() < 0)
            {
                ErrorLabel.Visibility = Visibility.Visible;
                return;
            }
            DialogResult = true;
        }
    }
}
