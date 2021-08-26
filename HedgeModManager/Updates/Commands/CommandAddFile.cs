using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HedgeModManager.Misc;

namespace HedgeModManager.Updates.Commands
{
    [UpdateCommand(Name)]
    public class CommandAddFile : IUpdateCommand
    {
        public const string Name = "add";
        public string FileName { get; set; }

        public CommandAddFile()
        {

        }

        public CommandAddFile(string fileName)
        {
            FileName = fileName;
        }

        public void Parse(string line)
        {
            FileName = line;
        }

        public Task PrepareExecute(ModInfo mod, ILogger logger)
        {
            string fullPath = Path.Combine(mod.RootDirectory, UpdateCommandList.TempDirName, FileName);
            string dirName = Path.GetDirectoryName(fullPath);

            if (!fullPath.IsSubPathOf(mod.RootDirectory))
                return Task.CompletedTask;

            if (!string.IsNullOrEmpty(dirName))
                Directory.CreateDirectory(dirName);

            return Task.CompletedTask;
        }

        public Task FinalizeExecute(ModInfo mod, ILogger logger)
        {
            string tempFullPath = Path.Combine(mod.RootDirectory, UpdateCommandList.TempDirName, FileName);
            string fullPath = Path.Combine(mod.RootDirectory, FileName);

            if (!fullPath.IsSubPathOf(mod.RootDirectory))
                return Task.CompletedTask;

            if (!File.Exists(tempFullPath))
                return Task.CompletedTask;

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            File.Move(tempFullPath, fullPath);

            return Task.CompletedTask;
        }

        public async Task Execute(ModInfo mod, ExecuteConfig config, CancellationToken cancellationToken)
        {
            config?.CurrentCallback?.Report(0);

            if (string.IsNullOrEmpty(FileName))
                return;

            string serverPath = Path.Combine(mod.UpdateServer, Uri.EscapeDataString(FileName));
            string fullPath = Path.Combine(mod.RootDirectory, UpdateCommandList.TempDirName, FileName);

            if (!fullPath.IsSubPathOf(mod.RootDirectory))
            {
                config?.Logger?.WriteLine($"Ignoring {FileName}");
                return;
            }

            try
            {
                config?.Logger.WriteLine($"Downloading {FileName}");
                await Singleton.GetInstance<HttpClient>().DownloadFileAsync(serverPath, fullPath, config.CurrentCallback,
                    cancellationToken);
            }
            catch (HttpRequestException e)
            {
                config.Logger.WriteLine($"Failed to download {FileName} : {e.Message}");
            }
        }

        public override string ToString()
        {
            return $"{Name} {FileName}";
        }
    }
}
