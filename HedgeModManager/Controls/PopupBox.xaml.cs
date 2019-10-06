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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HedgeModManager.Controls
{
    /// <summary>
    /// Interaction logic for PopupBox.xaml
    /// </summary>
    [ContentProperty("Children")]
    public partial class PopupBox : UserControl
    {
        public static readonly DependencyPropertyKey ChildrenProperty = DependencyProperty.RegisterReadOnly(
            "Children", typeof(UIElementCollection),
            typeof(PopupBox), new PropertyMetadata());

        public UIElementCollection Children
        {
            get
            {
                return (UIElementCollection)GetValue(ChildrenProperty.DependencyProperty);
            }
            private set
            {
                SetValue(ChildrenProperty, value);
            }
        }

        private UIElement OldContent;
        private Window mWindow;
        public PopupBox()
        {
            InitializeComponent();
            Children = ContentHost.Children;
        }

        public void Show(Window win)
        {
            Panel.SetZIndex(this, 3);
            mWindow = win;
            OldContent = (UIElement)win.Content;
            win.Content = this;
            BaseContent.Content = OldContent;
        }

        public void Close()
        {
            if (mWindow == null)
                return;

            BaseContent.Content = null;
            mWindow.Content = OldContent;
        }

        private void Background_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
