using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HedgeModManager.Misc;

namespace HedgeModManager.Updates
{
    public class GmiUpdateCommandList : IList<IUpdateCommand>
    {
        protected static char[] LineEndings = {'\r', '\n'};
        protected List<IUpdateCommand> Commands { get; set; } = new List<IUpdateCommand>();
        public const string TempDirName = ".hmmtemp";
        public int Count => Commands.Count;
        public bool IsReadOnly => false;

        public void Parse(string commands)
        {
            Clear();
            
            foreach (string line in commands.Split(LineEndings, StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith(";") || line.StartsWith("#"))
                    continue;

                int firstSpace = line.IndexOf(' ');
                if (firstSpace == -1)
                    continue;

                string cmdName = line.Substring(0, firstSpace).ToLowerInvariant();
                string cmdArg = line.Substring(firstSpace + 1);

                switch (cmdName)
                {
                    case CommandAddFile.Name:
                        Add(new CommandAddFile(cmdArg));
                        break;

                    case CommandRemoveFile.Name:
                        Add(new CommandRemoveFile(cmdArg));
                        break;

                    case CommandPrint.Name:
                        Add(new CommandPrint(cmdArg));
                        break;

                    case CommandPause.Name:
                        if (int.TryParse(cmdArg, out int timeout)) Add(new CommandPause(timeout));
                        break;
                }
            }
        }

        public async Task ExecuteAsync(ModInfo mod, ExecuteConfig config = default, CancellationToken cancellationToken = default)
        {
            foreach (var command in this)
                await command.PrepareExecute(mod, config?.Logger).ConfigureAwait(false);

            for (int i = 0; i < Count; i++)
            {
                CheckCancel();

                var command = this[i];
                await command.Execute(mod, config, cancellationToken).ConfigureAwait(false);
                CheckCancel();

                config?.OverallCallback?.Report((((double)i / (double)Count) * 100));
            }

            CheckCancel();
            for (int i = 0; i < Count; i++)
            {
                var command = this[i];
                await command.FinalizeExecute(mod, config?.Logger).ConfigureAwait(false);
            }
            
            DeleteTemp();
            config?.OverallCallback?.Report(100);

            void CheckCancel()
            {
                if (!cancellationToken.IsCancellationRequested)
                    return;

                DeleteTemp();
                cancellationToken.ThrowIfCancellationRequested();
            }

            void DeleteTemp()
            {
                try
                {
                    Directory.Delete(Path.Combine(mod.RootDirectory, TempDirName), true);
                }
                catch
                {
                    // ignore
                }
            }
        }

        public IEnumerator<IUpdateCommand> GetEnumerator()
        {
            return Commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IUpdateCommand item)
        {
            Commands.Add(item);
        }

        public void Clear()
        {
            Commands.Clear();
        }

        public bool Contains(IUpdateCommand item)
        {
            return Commands.Contains(item);
        }

        public void CopyTo(IUpdateCommand[] array, int arrayIndex)
        {
            Commands.CopyTo(array, arrayIndex);
        }

        public bool Remove(IUpdateCommand item)
        {
            return Commands.Remove(item);
        }

        public int IndexOf(IUpdateCommand item)
        {
            return Commands.IndexOf(item);
        }

        public void Insert(int index, IUpdateCommand item)
        {
            Commands.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Commands.RemoveAt(index);
        }

        public IUpdateCommand this[int index]
        {
            get => Commands[index];
            set => Commands[index] = value;
        }
    }

    public class ExecuteConfig
    {
        public IProgress<double> OverallCallback { get; set; }
        public IProgress<double?> CurrentCallback { get; set; }
        public ILogger Logger { get; set; } = DummyLogger.Instance;
    }

    public interface IUpdateCommand
    {
        Task PrepareExecute(ModInfo mod, ILogger logger);
        Task Execute(ModInfo mod, ExecuteConfig config = default, CancellationToken cancellationToken = default);
        Task FinalizeExecute(ModInfo mod, ILogger logger);
    }

    public class CommandAddFile : IUpdateCommand
    {
        public const string Name = "add";
        public string FileName { get; set; }

        public CommandAddFile(string fileName)
        {
            FileName = fileName;
        }

        public Task PrepareExecute(ModInfo mod, ILogger logger)
        {
            string fullPath = Path.Combine(mod.RootDirectory, GmiUpdateCommandList.TempDirName, FileName);
            string dirName = Path.GetDirectoryName(fullPath);

            if (!fullPath.IsSubPathOf(mod.RootDirectory))
                return Task.CompletedTask;

            if (!string.IsNullOrEmpty(dirName))
                Directory.CreateDirectory(dirName);

            return Task.CompletedTask;
        }

        public Task FinalizeExecute(ModInfo mod, ILogger logger)
        {
            string tempFullPath = Path.Combine(mod.RootDirectory, GmiUpdateCommandList.TempDirName, FileName);
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

            string serverPath = Path.Combine(mod.UpdateServer, Uri.EscapeUriString(FileName));
            string fullPath = Path.Combine(mod.RootDirectory, GmiUpdateCommandList.TempDirName, FileName);

            if (!fullPath.IsSubPathOf(mod.RootDirectory))
            {
                config.Logger?.WriteLine($"Ignoring {FileName}");
                return;
            }

            try
            {
                config.Logger.WriteLine($"Downloading {FileName}");
                await Singleton.GetInstance<HttpClient>().DownloadFileAsync(serverPath, fullPath, config.CurrentCallback,
                    cancellationToken);
            }
            catch(HttpRequestException e)
            {
                config.Logger.WriteLine($"Failed to download {FileName} : {e.Message}");
            }
        }

        public override string ToString()
        {
            return $"{Name} {FileName}";
        }
    }

    public class CommandRemoveFile : IUpdateCommand
    {
        public const string Name = "delete";
        public string FileName { get; set; }

        public CommandRemoveFile(string fileName)
        {
            FileName = fileName;
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

    public class CommandPrint : IUpdateCommand
    {
        public const string Name = "print";

        public string Text { get; set; }

        public CommandPrint(string text)
        {
            Text = text;
        }

        public Task PrepareExecute(ModInfo mod, ILogger logger) => Task.CompletedTask;

        public Task Execute(ModInfo mod, ExecuteConfig config = default, CancellationToken cancellationToken = default)
        {
            config?.CurrentCallback?.Report(null);
            config?.Logger?.WriteLine(Text);
            return Task.CompletedTask;
        }

        public Task FinalizeExecute(ModInfo mod, ILogger logger) => Task.CompletedTask;

        public override string ToString()
        {
            return $"{Name} {Text}";
        }
    }

    public class CommandPause : IUpdateCommand
    {
        public const string Name = "pause";
        public int Timeout { get; set; }

        public CommandPause(int timeout)
        {
            Timeout = timeout;
        }

        public Task PrepareExecute(ModInfo mod, ILogger logger) => Task.CompletedTask;

        public async Task Execute(ModInfo mod, ExecuteConfig config = default, CancellationToken cancellationToken = default)
        {
            if (Timeout <= 0)
                return;

            config?.CurrentCallback?.Report(null);
            await Task.Delay(Timeout, cancellationToken);
        }

        public Task FinalizeExecute(ModInfo mod, ILogger logger) => Task.CompletedTask;

        public override string ToString()
        {
            return $"{Name} {Timeout}ms";
        }
    }
}
