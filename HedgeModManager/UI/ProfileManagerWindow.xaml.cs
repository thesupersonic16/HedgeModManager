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

namespace HedgeModManager.UI
{
    /// <summary>
    /// Interaction logic for ProfileManager.xaml
    /// </summary>
    public partial class ProfileManagerWindow : Window
    {
        public ModProfile SelectedModProfile = null;

        public ProfileManagerWindow()
        {
            InitializeComponent();
        }

        private void UI_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void UI_Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new ProfileManagerRenameWindow(null);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                // Generate filename
                window.Profile.ModDBPath = HedgeApp.GenerateModDBFileName();
                // Add Profile
                HedgeApp.ModProfiles.Add(window.Profile);
                if (DataContext is MainWindowViewModel mainWindow)
                    mainWindow.Profiles.Add(window.Profile);
            }
        }

        private void UI_OK_Click(object sender, RoutedEventArgs e)
        {
            var item = ProfileListView.SelectedItem as ModProfile;
            if (item != null)
                SelectedModProfile = item;
            Close();
        }

        private void UI_ContextMenu_Opening(object sender, ContextMenuEventArgs e)
        {
            if (!(sender is ListViewItem listItem))
                return;
            if (!(DataContext is MainWindowViewModel mainWindow))
                return;

            var item = HedgeApp.FindChild<MenuItem>(listItem.ContextMenu, "ContextMenuItemDelete");
            if (item == null)
                return;

            // TODO: Check if its safe for ModsDB.ini to not exist
            // Prevent deleting the default profile
            item.IsEnabled = (listItem.DataContext as ModProfile)?.ModDBPath != "ModsDB.ini";

        }

        private void UI_ProfileRename_Click(object sender, RoutedEventArgs e)
        {
            if (!(ProfileListView.SelectedItem is ModProfile profile))
                return;
            if (!(DataContext is MainWindowViewModel mainWindow))
                return;
            new ProfileManagerRenameWindow(profile).ShowDialog();
        }

        private void UI_ProfileDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(ProfileListView.SelectedItem is ModProfile profile))
                return;
            if (!(DataContext is MainWindowViewModel mainWindow))
                return;
            if (File.Exists(Path.Combine(HedgeApp.ModsDbPath, profile.ModDBPath)))
                File.Delete(Path.Combine(HedgeApp.ModsDbPath, profile.ModDBPath));

            mainWindow.Profiles.Remove(profile);
            HedgeApp.ModProfiles.Remove(profile);
        }
    }
}
