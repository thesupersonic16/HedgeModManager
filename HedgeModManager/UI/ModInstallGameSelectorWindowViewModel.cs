using GongSolutions.Wpf.DragDrop;
using HedgeModManager.CodeCompiler;
using HedgeModManager.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HedgeModManager.UI
{
    public class ModInstallGameSelectorWindowViewModel
    {
        public ObservableCollection<GameInstall> Games { get; set; } = new ObservableCollection<GameInstall>();
        public GameInstall SelectedGame { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
