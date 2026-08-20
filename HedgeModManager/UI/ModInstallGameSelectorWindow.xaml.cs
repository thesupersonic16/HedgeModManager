using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Collections.ObjectModel;

namespace HedgeModManager.UI
{
    /// <summary>
    /// Interaction logic for GBModGameSelectorWindow.xaml
    /// </summary>
    public partial class ModInstallGameSelectorWindow : Window
    {
        public ModInstallGameSelectorWindowViewModel ViewModel = null;
        public GameInstall SelectedGame = null;

        public ModInstallGameSelectorWindow(List<GameInstall> compatibleGameInstalls)
        {
            var selectedGameInstall = compatibleGameInstalls.FirstOrDefault();
            if (!string.IsNullOrEmpty(RegistryConfig.LastGameInstall))
            {
                var gameInstall = compatibleGameInstalls
                    .FirstOrDefault(t => t.ExecutablePath == RegistryConfig.LastGameInstall);
                if (gameInstall != null)
                    selectedGameInstall = gameInstall;
            }
            DataContext = ViewModel = new ModInstallGameSelectorWindowViewModel
            {
                Games = new ObservableCollection<GameInstall>(compatibleGameInstalls),
                SelectedGame = selectedGameInstall
            };

            InitializeComponent();
        }

        public static Tuple<GameInstall, bool?> SelectGameInstall(List<Game> compatibleGames)
        {
            List<GameInstall> compatibleGameInstalls = HedgeApp.GameInstalls.Where(t => compatibleGames.Contains(t.Game)).ToList();

            if (compatibleGameInstalls.Count <= 1)
                return new(compatibleGameInstalls.FirstOrDefault(), true);

            var selector = new ModInstallGameSelectorWindow(compatibleGameInstalls);
            selector.ShowDialog();
            return new(selector.SelectedGame, selector.DialogResult);
        }


        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            HedgeApp.SelectGameInstall(SelectedGame = ViewModel.SelectedGame);
            DialogResult = true;
        }
    }
}