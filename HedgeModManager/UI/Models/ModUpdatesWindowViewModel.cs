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
using static HedgeModManager.Lang;

namespace HedgeModManager.UI.Models
{
    [ViewInfo("ModUpdatesTitle", 800, 450)]
    public class ModUpdatesWindowViewModel : IViewModel, INotifyPropertyChanged
    {
        private IView mView;

        public IView View
        {
            get => mView;
            set
            {
                if (mView == value)
                    return;

                mView = value;
                OnViewAssigned(mView);
            }
        }

        public ModUpdateInfoModel SelectedInfo { get; set; }

        public ObservableCollection<ModUpdateInfoModel> Mods { get; set; } =
            new ObservableCollection<ModUpdateInfoModel>();

        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsUpdating { get; set; }
        public bool IsIdle => !IsUpdating;
        public RelayCommand UpdateCommand { get; }
        public RelayCommand CancelCommand { get; }

        public bool DownloadMode { get; set; }

        public string TitleText => DownloadMode ? Localise("ModDownloadsTitle") : Localise("ModUpdatesTitle");
        public string UpdateText => DownloadMode ? Localise("CommonUIDownload") : Localise("CommonUIUpdate");
        public string ChangelogText => DownloadMode ? Localise("ModDownloadsDescription") : Localise("ModUpdatesChangelog");


        public ModUpdatesWindowViewModel(bool downloadMode = false)
        {
            DownloadMode = downloadMode;
            UpdateCommand = new RelayCommand(Execute, () => IsIdle);
            CancelCommand = new RelayCommand(CancelAll);
        }

        public ModUpdatesWindowViewModel(IModUpdateInfo mod, bool downloadMode = false) : this(downloadMode)
        {
            if (mod == null)
                return;

            Mods.Add(new ModUpdateInfoModel(mod));
        }

        public ModUpdatesWindowViewModel(IEnumerable<IModUpdateInfo> mods, bool downloadMode = false) : this(downloadMode)
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

        private void OnViewAssigned(IView view)
        {
            if (view == null)
                return;

            view.Title = TitleText;
        }
    }

    public class ModUpdateInfoModel : INotifyPropertyChanged
    {
        public IModUpdateInfo Info { get; set; }
        public string Title => Info.Mod.Title;
        public string OldVersion => Info.Mod.Version;
        public string NewVersion => Info.Version;
        public string VersionText => OldVersion != null ? OldVersion + "->" + NewVersion : NewVersion;
        public NotifyTask<string> Changelog => new NotifyTask<string>(Info.GetChangelog());
        public bool DoUpdate { get; set; } = true;
        public bool MultiFileMode => !Info.SingleFileMode;
        public ILogger Logger { get; set; } = new StringLogger();
        public NotifyProgress<double?> OverallProgress { get; set; } = new NotifyProgress<double?>();
        public NotifyProgress<double?> CurrentFileProgress { get; set; } = new NotifyProgress<double?>();
        public CancellationTokenSource CancelSource { get; set; }


        public ModUpdateInfoModel(IModUpdateInfo info)
        {
            Info = info;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
