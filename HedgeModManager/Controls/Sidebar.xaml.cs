using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HedgeModManager.Controls
{
    /// <summary>
    /// Interaction logic for Sidebar.xaml
    /// </summary>
    [ContentProperty("Children")]
    public partial class Sidebar : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public static readonly DependencyPropertyKey ChildrenProperty = DependencyProperty.RegisterReadOnly(
            "Children", typeof(UIElementCollection), 
            typeof(Sidebar), new PropertyMetadata());

        private bool mOpen;
        private bool mOpenClick;

        public bool IsOpen
        {
            get
            {
                return mOpen;
            }
            set
            {
                if (value)
                {
                    ToggleButton.Content = OpenOnClick ? "<" : string.Empty;
                    SidebarHost.Visibility = Visibility.Visible;
                    TempArea.Visibility = Visibility.Visible;
                    SidebarHost.RenderTransform.BeginAnimation(TranslateTransform.XProperty, 
                        CreateAnimation(SidebarHost.ActualWidth == 0 ? -SidebarWidth : -SidebarHost.ActualWidth, 0, 0.4));
                }
                else
                {
                    ToggleButton.Content = OpenOnClick ? ">" : string.Empty;
                    TempArea.Visibility = Visibility.Collapsed;
                }
                mOpen = value;
                RaisePropertyChanged("IsOpen");
            }
        }
        public double SidebarWidth
        {
            get
            {
                return SidebarHost.Width;
            }

            set
            {
                SidebarHost.Width = value;
                RaisePropertyChanged("SidebarWidth");
            }
        }

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

        public bool OpenOnClick
        {
            get
            {
                return mOpenClick;
            }
            set
            {
                mOpenClick = value;
                ToggleButton.Content = value ? ">" : string.Empty;
                RaisePropertyChanged("OpenOnClick");
            }
        }

        public Sidebar()
        {
            InitializeComponent();
            Children = SidebarHost.Children;
            MouseEnter += Sidebar_MouseEnter;
            MouseLeave += Sidebar_MouseLeave;
        }

        private void Sidebar_MouseLeave(object sender, MouseEventArgs e)
        {
            if(!OpenOnClick)
            {
                IsOpen = false;
            }
        }

        private void Sidebar_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!OpenOnClick)
            {
                IsOpen = true;
            }
        }

        private void Sidebar_Click(object sender, RoutedEventArgs e)
        {
            if (OpenOnClick)
            {
                IsOpen = !IsOpen;
            }
        }

        private AnimationTimeline CreateAnimation(double from, double to, double dur,
              EventHandler whenDone = null)
        {
            IEasingFunction ease = new BackEase
            { Amplitude = 0.5, EasingMode = EasingMode.EaseOut };
            var duration = new Duration(TimeSpan.FromSeconds(dur));
            var anim = new DoubleAnimation(from, to, duration)
            { EasingFunction = ease };
            if (whenDone != null)
                anim.Completed += whenDone;
            anim.Freeze();
            return anim;
        }

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
