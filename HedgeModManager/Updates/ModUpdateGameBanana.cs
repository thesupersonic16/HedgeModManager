using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using GameBananaAPI;
using HedgeModManager.Misc;
using HedgeModManager.UI.Models;

namespace HedgeModManager.Updates
{
    public class ModUpdateGameBanana : IModUpdateInfo
    {
        public ModInfo Mod { get; internal set; }
        public string Version { get; internal set; }
        public bool SingleFileMode => true;

        protected GBAPIItemDataBasic Item;
        protected GBAPIFile File;
        protected ModsDB ModsDatabase;

        public ModUpdateGameBanana(string name, GBAPIItemDataBasic item, GBAPIFile file, ModsDB modsDb)
        {
            Item = item;
            File = file;
            Version = item.OwnerName;
            ModsDatabase = modsDb;
            Mod = new ModInfo()
            {
                Title = name ?? item.ModName,
                Version = null
            };
        }

        public async Task<string> GetChangelog()
        {
            return Item.Body;
        }

        public async Task ExecuteAsync(ExecuteConfig config, CancellationToken cancellationToken = default)
        {
            try
            {
                config.Logger.WriteLine("Connecting...");
                using (var resp = await Singleton.GetInstance<HttpClient>()
                    .GetAsync(File.DownloadURL, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                {
                    resp.EnsureSuccessStatusCode();

                    config.Logger.WriteLine("Downloading " + File.FileName);
                    var destinationPath = Path.GetFileName(resp.RequestMessage.RequestUri.AbsoluteUri);
                    using (var destinationFile = System.IO.File.Create(destinationPath, 8192, FileOptions.Asynchronous))
                        await resp.Content.CopyToAsync(destinationFile, config.OverallCallback, cancellationToken);
                    
                    config.Logger.WriteLine("Installing " + destinationPath);
                    ModsDatabase.InstallMod(destinationPath);
                    System.IO.File.Delete(destinationPath);
                }
            }
            catch
            {
                config.Logger.WriteLine("Failed!");
            }
        }
    }
}
