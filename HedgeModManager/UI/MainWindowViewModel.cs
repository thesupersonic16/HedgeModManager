using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace HedgeModManager.UI
{
    public class MainWindowViewModel : INotifyPropertyChanged, IDropTarget
    {
        public CPKREDIRConfig CPKREDIR { get; set; }
        public ModsDB ModsDB { get; set; }
        public IEnumerable<SteamGame> Games { get; set; }
        public ObservableCollection<ModInfo> Mods { get; set; } = new ObservableCollection<ModInfo>();
        public Dictionary<string, string> SupportedCultures => HedgeApp.SupportedCultures;

        public event PropertyChangedEventHandler PropertyChanged;


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
                // Try Install mods from all files
                foreach (string path in dataObject.GetFileDropList())
                    ModsDB.InstallMod(path);
            }
            else
            {
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            }
        }
    }
}
