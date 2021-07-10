using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using HedgeModManager.Annotations;
using HedgeModManager.Updates;

namespace HedgeModManager.UI.Models
{
    [ViewInfo("Mod Updates", 800, 450)]
    public class ModUpdatesWindowViewModel : IViewModel, INotifyPropertyChanged
    {
        public IView View { get; set; }
        public ModUpdateInfoModel SelectedInfo { get; set; }

        public ObservableCollection<ModUpdateInfoModel> Mods { get; set; } =
            new ObservableCollection<ModUpdateInfoModel>();

        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsUpdating { get; set; }
        public bool IsIdle => !IsUpdating;
        public RelayCommand UpdateCommand { get; }
        public RelayCommand CancelCommand { get; }

        public ModUpdatesWindowViewModel()
        {
            UpdateCommand = new RelayCommand(Execute, () => IsIdle);
            CancelCommand = new RelayCommand(CancelAll);
        }

        public ModUpdatesWindowViewModel(IModUpdateInfo mod) : this()
        {
            if (mod == null)
                return;

            Mods.Add(new ModUpdateInfoModel(mod));
        }

        public ModUpdatesWindowViewModel(IEnumerable<IModUpdateInfo> mods) : this()
        {
            Set(mods);
        }

        public void Set(IEnumerable<IModUpdateInfo> mods)
        {
            Mods.Clear();

            if (mods == null)
                return;

            foreach (var mod in mods)
            {
                Mods.Add(new ModUpdateInfoModel(mod));
            }

            SelectedInfo = Mods.FirstOrDefault();
        }

        public async void Execute()
        {
            if (IsUpdating)
                return;

            IsUpdating = true;
            UpdateCommand.RaiseCanExecuteChanged();

            Mods = new ObservableCollection<ModUpdateInfoModel>(Mods.Where(x => x.DoUpdate));
            var tasks = new Task[Mods.Count];

            try
            {
                for (int i = 0; i < Mods.Count; i++)
                {
                    var mod = Mods[i];
                    mod.CancelSource = new CancellationTokenSource();

                    var config = new ExecuteConfig
                    {
                        CurrentCallback = mod.CurrentFileProgress,
                        OverallCallback = mod.OverallProgress,
                        Logger = mod.Logger
                    };

                    tasks[i] = mod.Info.ExecuteAsync(config, mod.CancelSource.Token);
                }

                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                // ignored
            }

            IsUpdating = false;
            UpdateCommand.RaiseCanExecuteChanged();
            View?.Close();
        }

        public void CancelAll()
        {
            foreach (var mod in Mods)
                if (mod.CancelSource != null && !mod.CancelSource.IsCancellationRequested)
                    mod.CancelSource.Cancel();

            IsUpdating = false;
            UpdateCommand.RaiseCanExecuteChanged();
            View?.Close();
        }
    }

    public class ModUpdateInfoModel
    {
        public IModUpdateInfo Info { get; set; }
        public string Title => Info.Mod.Title;
        public string OldVersion => Info.Mod.Version;
        public string NewVersion => Info.Version;
        public NotifyTask<string> Changelog => new NotifyTask<string>(Info.GetChangelog());
        public bool DoUpdate { get; set; } = true;
        public ILogger Logger { get; set; } = new StringLogger();
        public NotifyProgress<double> OverallProgress { get; set; } = new NotifyProgress<double>();
        public NotifyProgress<double?> CurrentFileProgress { get; set; } = new NotifyProgress<double?>();
        public CancellationTokenSource CancelSource { get; set; }

        public ModUpdateInfoModel(IModUpdateInfo info)
        {
            Info = info;
        }
    }
}
