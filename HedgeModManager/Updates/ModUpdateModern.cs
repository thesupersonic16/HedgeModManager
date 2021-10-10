using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HedgeModManager.Updates.Commands;

namespace HedgeModManager.Updates
{
    public class ModUpdateModern : IModUpdateInfo
    {
        public ModInfo Mod { get; internal set; }
        public string Version { get; internal set; }
        public bool SingleFileMode { get; }

        protected ModFileTree BaseTree { get; set; }
        protected ModFileTree UpdateTree { get; set; }
        protected ModVersionInfo VersionInfo { get; set; }

        private UpdateCommandList Commands { get; set; } = new UpdateCommandList();
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

        public async Task ExecuteAsync(ExecuteConfig config = default, CancellationToken cancellationToken = default)
        {
            Commands.Clear();

            var result = UpdateTree.Compare(BaseTree);

            foreach (var entry in result.RemovedEntries)
                Commands.Add(new CommandRemoveFile(entry.FullPath));

            foreach (var entry in UpdateTree.ResolveFileSystem(Mod.RootDirectory).MissingEntries)
            {
                if (entry.IsFile)
                    Commands.Add(new CommandAddFile(entry.FullPath));
                else
                    Commands.Add(new CommandMakeDirectory(entry.FullPath));
            }

            foreach (var entry in result.AddedEntries)
            {
                if (entry.IsFile)
                    Commands.Add(new CommandAddFile(entry.FullPath));
                else
                    Commands.Add(new CommandMakeDirectory(entry.FullPath));
            }

            await Commands.ExecuteAsync(Mod, config, cancellationToken);
        }

        public static async Task<ModUpdateModern> GetUpdateAsync(ModVersionInfo info, ModInfo mod, CancellationToken cancellationToken = default)
        {
            var baseTree = mod.FileTree ??
                           await info.GetBaseFileTree(mod.UpdateServer, cancellationToken).ConfigureAwait(false);

            if (baseTree == null)
                return null;

            ModFileTree newTree = null;
            try
            {
                newTree = await ModFileTree
                    .LoadFromUrl(Path.Combine(mod.UpdateServer, ModFileTree.FixedFileName), cancellationToken)
                    .ConfigureAwait(false);
            }
            catch(HttpRequestException)
            {
                return null;
            }

            if (newTree == null)
                return null;

            return new ModUpdateModern
            {
                Mod = mod,
                BaseTree = baseTree,
                UpdateTree = newTree,
                Version = info.Version,
                VersionInfo = info
            };
        }
    }
}
