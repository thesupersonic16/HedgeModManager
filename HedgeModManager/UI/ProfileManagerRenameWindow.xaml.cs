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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static HedgeModManager.Lang;

namespace HedgeModManager.UI
{
    /// <summary>
    /// Interaction logic for ProfileManagerRenameWindow.xaml
    /// </summary>
    public partial class ProfileManagerRenameWindow : Window, INotifyPropertyChanged
    {
        public ModProfile Profile { get; set; }
        public string NewProfileName { get; set; }
        public ProfileManagerRenameWindow(ModProfile profile)
        {
            Profile = profile;
            DataContext = this;
            InitializeComponent();
            Title = Header.Text = Localise(profile == null ? "ProfileWindowUICreateTitle" : "ProfileWindowUIRenameTitle");
            if (profile == null)
                Profile = new ModProfile("New Profile", "");
            NewProfileName = Profile.Name;
        }

        private void UI_OK_Click(object sender, RoutedEventArgs e)
        {
            Profile.Name = NewProfileName;
            DialogResult = true;
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
