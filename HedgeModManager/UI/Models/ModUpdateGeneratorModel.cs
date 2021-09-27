using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HedgeModManager.Annotations;
using HedgeModManager.Updates;

namespace HedgeModManager.UI.Models
{
    [ViewInfo("Generate Update Files", 700, 450)]
    public class ModUpdateGeneratorModel : IViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public IView View { get; set; }
        public ModInfo Mod { get; set; }
        public string NewVersion { get; set; }
        public string NewChangelog { get; set; }
        public GeneratorState State { get; protected set; }
        public RelayCommand ConfigureCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public ModFileTree GeneratedTree { get; protected set; }
        public ModFileEntry SelectedFile { get; set; }

        public bool IsIdle => State == GeneratorState.Idle;

        public bool IsBusy
        {
            get => !IsIdle;
            protected set => State = value ? GeneratorState.Busy : GeneratorState.Idle;
        }

        public ModUpdateGeneratorModel()
        {
            ConfigureCommand = new RelayCommand(ConfigureFiles, () => IsIdle);
            DeleteCommand = new RelayCommand(DeleteFile);
        }

        public ModUpdateGeneratorModel(ModInfo mod) : this()
        {
            Mod = mod;
        }

        public async void ConfigureFiles()
        {
            if (IsBusy)
                return;

            if (GeneratedTree != null)
            {
                CreateUpdateFiles();
                this.Close();
                return;
            }

            ConfigureCommand.RaiseCanExecuteChanged();

            IsBusy = true;
            
            GeneratedTree = await Mod.GenerateFileTree(false);

            IsBusy = false;
            ConfigureCommand.RaiseCanExecuteChanged();
        }

        public void CreateUpdateFiles()
        {
            if (IsBusy || GeneratedTree == null)
                return;

            var dialog = new FolderBrowserDialog
            {
                Title = "Select Folder"
            };

            if (!dialog.ShowDialog())
            {
                IsBusy = false;
                return;
            }

            GeneratedTree.Save(Path.Combine(dialog.SelectedFolder, ModFileTree.FixedFileName));

            Directory.CreateDirectory(Path.Combine(dialog.SelectedFolder, "_Sources"));
            File.WriteAllText(Path.Combine(dialog.SelectedFolder, "_Sources", "Changelog.md"), NewChangelog);

            var ini = new IniFile
            {
                ["Main"] =
                {
                    ["VersionString"] = NewVersion,
                    ["Markdown"] = "_Sources/Changelog.md",
                    ["UpdaterType"] = ((int)UpdateType.HmmV1).ToString()
                }
            };

            using var outStream = File.Create(Path.Combine(dialog.SelectedFolder, "mod_version.ini"));
            ini.Write(outStream);
        }

        public void DeleteFile()
        {
            if (IsBusy || SelectedFile == null)
                return;

            SelectedFile.Parent?.Remove(SelectedFile);
        }

        public enum GeneratorState
        {
            Idle,
            Busy
        }
    }
}
