using System;
using System.IO;
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
using GameBananaAPI;
using System.Net;
using System.Windows.Controls.Primitives;
using HedgeModManager.Controls;
using System.Net.Cache;
using System.Net.Http;
using PropertyTools.Wpf;
using static HedgeModManager.Lang;
using PopupBox = HedgeModManager.Controls.PopupBox;
using System.Diagnostics;
using System.Windows.Shell;
using HedgeModManager.Misc;
using HedgeModManager.Exceptions;
using System.Collections.ObjectModel;

namespace HedgeModManager.UI
{
    /// <summary>
    /// Interaction logic for GBModGameSelectorWindow.xaml
    /// </summary>
    public partial class GBModGameSelectorWindow : Window
    {

        public GBModGameSelectorWindowViewModel ViewModel = null;
        public GameInstall SelectedGame = null;

        public GBModGameSelectorWindow(List<GameInstall> compatibleGameInstalls)
        {
            DataContext = ViewModel = new GBModGameSelectorWindowViewModel
            {
                Games = new ObservableCollection<GameInstall>(compatibleGameInstalls),
                SelectedGame = compatibleGameInstalls.FirstOrDefault()
            };

            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedGame = ViewModel.SelectedGame;
            DialogResult = true;
        }
    }
}