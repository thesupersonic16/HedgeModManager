using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using HedgeModManager.CodeCompiler;
using HedgeModManager.Exceptions;
using HedgeModManager.Properties;
using Newtonsoft.Json;
using static HedgeModManager.Lang;

namespace HedgeModManager.UI
{
    public class MainWindowViewModel : INotifyPropertyChanged, IDropTarget
    {
        // Dummy GameInstall for adding games
        public static GameInstall GameInstallAddGame { get; set; } = new GameInstall(HedgeModManager.Games.AddGame, null, null, GameLauncher.None);

        public CPKREDIRConfig CPKREDIR { get; set; }
        public ModsDB ModsDB { get; set; }
        public ObservableCollection<GameInstall> Games { get; set; } = new ObservableCollection<GameInstall>();
        public ObservableCollection<ModInfo> Mods { get; set; } = new ObservableCollection<ModInfo>();
        public ObservableCollection<ModInfo> ModsSearch { get; set; } = new ObservableCollection<ModInfo>();
        public ObservableCollection<ModProfile> Profiles { get; set; } = new ObservableCollection<ModProfile>();

        public ModInfo SelectedMod { get; set; }
        public ModProfile SelectedModProfile { get; set; }
        public CSharpCode SelectedCode { get; set; }
        public bool HiddenMode { get; set; }
        public bool DevBuild { get; set; }

        public bool HasNoMods => Mods.Count == 0;
        public bool HasNoCodes => ModsDB.CodesDatabase.Codes.Count == 0;
        public bool IsCustomGame => HedgeApp.CurrentGameInstall.IsCustom;

        public int RowAlternationCount => RegistryConfig.UseAlternatingRows ? 2 : 0;

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
                foreach (var file in dataObject.GetFileDropList())
                {
                    // Check if the file is a profile
                    if (file.ToLower().EndsWith(".json"))
                    {
                        var result = ExportProfile.Import(file);
                        if (result.IsInvalid)
                        {
                            HedgeApp.CreateOKMessageBox(Localise("CommonUIError"), Localise("ProfileWindowUIImportFail")).ShowDialog();
                            continue;
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
                                continue;
                        }

                        HedgeApp.ModProfiles.Add(result.Profile);
                        Profiles.Add(result.Profile);
                        result.Database.SaveDBSync(false);
                        // Save profiles
                        string profilePath = Path.Combine(HedgeApp.CurrentGameInstall.GameDirectory, "profiles.json");
                        File.WriteAllText(profilePath, JsonConvert.SerializeObject(HedgeApp.ModProfiles));

                    }
                    else
                    {
                        try
                        {
                            ModsDB.InstallMod(file);
                        }
                        catch (ModInstallException)
                        {
                            var box = new HedgeMessageBox(Localise("CommonUIError"), Localise("DialogUINoDecompressor"));
                            box.AddButton(Localise("CommonUIClose"), () => box.Close());
                            box.ShowDialog();
                        }
                    }
                }
            }
            else
            {
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            }
        }

        // NOTE: Loading values may not be needed as data may already be up to date
        public void SaveProfileConfig(ModProfile profile, ModsDB modsDB)
        {
            foreach (var mod in modsDB.Mods)
                mod.ExportConfig(profile);
        }
        public void LoadProfileConfig(ModProfile profile, ModsDB modsDB)
        {
            foreach (var mod in modsDB.Mods)
                mod.ImportConfig(profile);
        }
    }
}
