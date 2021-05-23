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
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using static HedgeModManager.Lang;

namespace HedgeModManager.UI
{
    /// <summary>
    /// Interaction logic for ProfileManager.xaml
    /// </summary>
    public partial class ProfileManagerWindow : Window, IDropTarget
    {

        public ProfileManagerWindow()
        {
            InitializeComponent();
        }

        public void ImportProfile(string path)
        {
            if (!(DataContext is MainWindowViewModel model))
                return;

            var result = ExportProfile.Import(path);
            if (result.IsInvalid)
            {
                HedgeApp.CreateOKMessageBox(Localise("CommonUIError"), Localise("ProfileWindowUIImportFail")).ShowDialog();
                return;
            }

            if (result.HasErrors)
            {
                bool abort = true;
                var box = new HedgeMessageBox(Localise("ProfileWindowUIImportFail"),
                    result.BuildMarkdown(), textAlignment: TextAlignment.Left, type: InputType.MarkDown);

                box.AddButton(Localise("CommonUIIgnore"), () =>
                {
                    abort = false;
                    box.Close();
                });
                box.AddButton(Localise("CommonUICancel"), () => box.Close());

                box.ShowDialog();
                if (abort)
                    return;
            }

            model.Profiles.Add(result.Profile);
            result.Database.SaveDBSync(false);
        }

        private void UI_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UI_ProfileRename_Click(null, null);
        }

        private void UI_Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new ProfileManagerRenameWindow(null);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                // Add Profile
                if (DataContext is MainWindowViewModel mainWindow)
                {
                    window.Profile.GeneratePath();
                    mainWindow.Profiles.Add(window.Profile);
                }
            }
        }

        private void UI_OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UI_ContextMenu_Opening(object sender, ContextMenuEventArgs e)
        {
            if (!(ProfileListView.SelectedItem is ModProfile profile))
                return;
            if (!(sender is ListViewItem listItem))
                return;
            if (!(DataContext is MainWindowViewModel mainWindow))
                return;

            var item = HedgeApp.FindChild<MenuItem>(listItem.ContextMenu, "ContextMenuItemDelete");
            if (item == null)
                return;

            // Make sure we're not deleting our current profile.
            item.IsEnabled = !profile.Enabled;
        }

        private void UI_ProfileRename_Click(object sender, RoutedEventArgs e)
        {
            if (!(ProfileListView.SelectedItem is ModProfile profile))
                return;
            if (!(DataContext is MainWindowViewModel mainWindow))
                return;

            new ProfileManagerRenameWindow(profile).ShowDialog();
        }

        private void UI_ProfileExport_Click(object sender, RoutedEventArgs e)
        {
            if (!(ProfileListView.SelectedItem is ModProfile profile))
                return;
            if (!(DataContext is MainWindowViewModel mainWindow))
                return;

            var sfd = new SaveFileDialog
            {
                Filter = "Mod Profile | *.json",
                DefaultExt = "json",
                FileName = Path.ChangeExtension(profile.ModDBPath, "json")
            };

            if (sfd.ShowDialog().Value)
            {
                profile.Export(sfd.FileName);
            }
        }

        private void UI_ProfileDuplicate_Click(object sender, RoutedEventArgs e)
        {
            if (!(ProfileListView.SelectedItem is ModProfile oldProfile))
                return;

            var window = new ProfileManagerRenameWindow(null);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                // Generate Path
                window.Profile.GeneratePath();

                // Copy Profile
                try
                {
                    File.Copy(
                        Path.Combine(HedgeApp.ModsDbPath, oldProfile.ModDBPath),
                        Path.Combine(HedgeApp.ModsDbPath, window.Profile.ModDBPath));
                }
                catch { }
                // Add Profile
                if (DataContext is MainWindowViewModel mainWindow)
                    mainWindow.Profiles.Add(window.Profile);
            }
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
        }

        private void ProfileListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(ProfileListView.SelectedItem is ModProfile profile))
                return;

            var ctrlKey = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            if (Keyboard.IsKeyDown(Key.Space))
                UI_ProfileRename_Click(null, null);
            if (ctrlKey && Keyboard.IsKeyDown(Key.N))
                UI_Add_Click(null, null);
            if (ctrlKey && Keyboard.IsKeyDown(Key.D))
                UI_ProfileDuplicate_Click(null, null);
            if (ctrlKey && Keyboard.IsKeyDown(Key.E))
                UI_ProfileExport_Click(null, null);
            if (ctrlKey && Keyboard.IsKeyDown(Key.I))
                UI_ProfileImport_Click(null, null);
            if (Keyboard.IsKeyDown(Key.Delete))
                UI_ProfileDelete_Click(null, null);
        }

        private void UI_ProfileImport_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainWindowViewModel model))
                return;

            var ofd = new OpenFileDialog
            {
                Filter = "Mod Profile | *.json",
                DefaultExt = "json"
            };

            if (!ofd.ShowDialog().Value)
                return;

            ImportProfile(ofd.FileName);
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            var dataObject = dropInfo.Data as IDataObject;
            if (dataObject != null && dataObject.GetDataPresent(DataFormats.FileDrop))
                dropInfo.Effects = DragDropEffects.Copy;
            else
                dropInfo.Effects = DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;
            if (dataObject != null && dataObject.ContainsFileDropList())
            {
                foreach (var file in dataObject.GetFileDropList())
                {
                    // Check if the file is a profile
                    if (file.ToLower().EndsWith(".json"))
                        ImportProfile(file);
                }
            }
            else
            {
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            }
        }
    }
}
