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
    [UpdateCommand("mkdir")]
    public class CommandMakeDirectory : IUpdateCommand
    {
        public string Path { get; set; }

        public CommandMakeDirectory() {}

        public CommandMakeDirectory(string path)
        {
            Path = path;
        }

        public void Parse(string line)
        {
            Path = line;
        }

        public Task PrepareExecute(ModInfo mod, ILogger logger) => Task.CompletedTask;

        public Task Execute(ModInfo mod, ExecuteConfig config = default, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task FinalizeExecute(ModInfo mod, ILogger logger)
        {
            string fullPath = System.IO.Path.Combine(mod.RootDirectory, Path);
            if (!fullPath.IsSubPathOf(mod.RootDirectory))
            {
                logger?.WriteLine($"Ignoring {Path}");
                return Task.CompletedTask;
            }

            try
            {
                Directory.CreateDirectory(fullPath);
            }
            catch
            {
                logger?.WriteLine($"Failed to create directory {Path}");
            }

            return Task.CompletedTask;
        }

        public override string ToString()
        {
            return $"mkdir {Path}";
        }
    }
}
