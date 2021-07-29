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
using HedgeModManager.UI.Models;

namespace HedgeModManager.UI
{
    /// <summary>
    /// Interaction logic for ViewImplWindows.xaml
    /// </summary>
    public partial class ViewImplWindows : Window, IView
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(IViewModel), typeof(ViewImplWindows), new PropertyMetadata(default(IViewModel)));

        public IViewModel Model
        {
            get => (IViewModel) GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public ViewImplWindows(IViewModel model)
        {
            InitializeComponent();

            ContentHost.DataContext = model;
            ContentHost.Content = model;
        }
    }
}
