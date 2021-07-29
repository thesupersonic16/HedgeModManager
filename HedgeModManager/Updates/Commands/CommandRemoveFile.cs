using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HedgeModManager.Misc;

namespace HedgeModManager.Updates.Commands
{
    [UpdateCommand(Name)]
    public class CommandRemoveFile : IUpdateCommand
    {
        public const string Name = "delete";
        public string FileName { get; set; }

        public CommandRemoveFile()
        {

        }

        public CommandRemoveFile(string fileName)
        {
            FileName = fileName;
        }

        public void Parse(string line)
        {
            FileName = line;
        }

        public Task PrepareExecute(ModInfo mod, ILogger logger) => Task.CompletedTask;

        public Task Execute(ModInfo mod, ExecuteConfig config, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task FinalizeExecute(ModInfo mod, ILogger logger)
        {
            string fullPath = Path.Combine(mod.RootDirectory, FileName);
            try
            {
                if (!fullPath.IsSubPathOf(mod.RootDirectory))
                {
                    logger?.WriteLine($"Ignoring {FileName}");
                    return Task.CompletedTask;
                }

                bool isDir = FileName.EndsWith("/") || FileName.EndsWith("\\");

                if (!isDir)
                {
                    logger?.WriteLine($"Deleting File {FileName}");
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                }
                else
                {
                    logger?.WriteLine($"Deleting Directory {FileName}");
                    if (Directory.Exists(fullPath))
                        Directory.Delete(fullPath, true);
                }
            }
            catch
            {
                logger?.WriteLine($"Failed to delete {FileName}");
            }

            return Task.CompletedTask;
        }

        public override string ToString()
        {
            return $"{Name} {FileName}";
        }
    }
}
