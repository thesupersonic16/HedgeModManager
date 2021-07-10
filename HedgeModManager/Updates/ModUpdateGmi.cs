using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HedgeModManager.Updates
{
    public class ModUpdateGmi : IModUpdateInfo
    {
        public ModInfo Mod { get; internal set; }
        public string Version { get; internal set; }

        protected ModVersionInfo VersionInfo { get; set; }
        protected GmiUpdateCommandList Commands { get; set; } = new GmiUpdateCommandList();
        private string ChangelogCache { get; set; }

        public async Task<string> GetChangelog()
        {
            if (!string.IsNullOrEmpty(ChangelogCache))
                return ChangelogCache;

            if (string.IsNullOrEmpty(VersionInfo.ChangeLogPath))
                return VersionInfo.Changelog;
            
            try
            {
                var response = await Singleton.GetInstance<HttpClient>()
                    .GetAsync(Path.Combine(Mod.UpdateServer, VersionInfo.ChangeLogPath)).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                    return VersionInfo.Changelog;

                ChangelogCache = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return ChangelogCache;
            }
            catch
            {
                return VersionInfo.Changelog;
            }
        }

        public async Task ExecuteAsync(ExecuteConfig config, CancellationToken cancellationToken = default)
        {
            await Commands.ExecuteAsync(Mod, config, cancellationToken);
        }

        public static async Task<ModUpdateGmi> GetUpdateAsync(ModInfo mod, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(mod.UpdateServer))
                return null;

            string modVersionPath = Path.Combine(mod.UpdateServer, "mod_version.ini");
            string modFilesPath = Path.Combine(mod.UpdateServer, "mod_files.txt");

            var versionInfo = await ModVersionInfo.ParseFromWebAsync(modVersionPath, cancellationToken)
                .ConfigureAwait(false);

            if (versionInfo == null)
                return null;

            cancellationToken.ThrowIfCancellationRequested();

            var filesResult = await Singleton.GetInstance<HttpClient>().GetAsync(modFilesPath, cancellationToken)
                .ConfigureAwait(false);
            if (!filesResult.IsSuccessStatusCode)
                return null;

            cancellationToken.ThrowIfCancellationRequested();

            var update = new ModUpdateGmi
            {
                Mod = mod,
                VersionInfo = versionInfo,
                Version = versionInfo.Version
            };
            update.Commands.Parse(await filesResult.Content.ReadAsStringAsync());

            return update;
        }
    }
}
