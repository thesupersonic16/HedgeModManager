using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.UI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public CPKREDIRConfig CPKREDIR { get; set; }
        public ModsDB ModsDB { get; set; }
        public IEnumerable<Game> Games { get; set; }
        public ObservableCollection<ModInfo> Mods { get; set; } = new ObservableCollection<ModInfo>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
