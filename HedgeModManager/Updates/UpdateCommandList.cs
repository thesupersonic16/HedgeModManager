using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HedgeModManager.Misc;
using HedgeModManager.Updates.Commands;

namespace HedgeModManager.Updates
{
    public class UpdateCommandList : IList<IUpdateCommand>
    {
        protected static Dictionary<string, Type> RegisteredCommands { get; } = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        protected static char[] LineEndings { get; } = {'\r', '\n'};
        protected List<IUpdateCommand> Commands { get; set; } = new List<IUpdateCommand>();
        
        public const string TempDirName = ".hmmtemp";
        public int Count => Commands.Count;
        public bool IsReadOnly => false;

        static UpdateCommandList()
        {
            RegisterCommands(Assembly.GetExecutingAssembly());
        }

        public static void RegisterCommands(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(t =>
                t.GetCustomAttribute<UpdateCommandAttribute>() != null && typeof(IUpdateCommand).IsAssignableFrom(t));

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<UpdateCommandAttribute>();
                foreach (string cmdName in attribute.Names)
                {
                    if (RegisteredCommands.ContainsKey(cmdName))
                        RegisteredCommands[cmdName] = type;
                    else
                        RegisteredCommands.Add(cmdName, type);
                }
            }
        }

        public static Type GetCommandType(string name)
        {
            if (RegisteredCommands.TryGetValue(name, out var type))
                return type;

            return null;
        }

        public static IUpdateCommand ParseLine(string line, out string name, out string args)
        {
            name = null;
            args = null;

            if (string.IsNullOrEmpty(line) || line.StartsWith(";") || line.StartsWith("#"))
                return null;

            int firstSpace = line.IndexOf(' ');
            if (firstSpace == -1)
                return null;

            string cmdName = line.Substring(0, firstSpace).ToLowerInvariant();
            string cmdArg = line.Substring(firstSpace + 1);
            name = cmdName;
            args = cmdArg;

            var type = GetCommandType(cmdName);
            if (type == null)
                return null;

            var command = (IUpdateCommand)Activator.CreateInstance(type);
            command.Parse(cmdArg);

            return command;
        }

        public void Parse(string commands)
        {
            Clear();
            foreach (string line in commands.Split(LineEndings, StringSplitOptions.RemoveEmptyEntries))
            {
                var cmd = ParseLine(line, out _, out _);
                if (cmd != null)
                    Add(cmd);
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

                config?.OverallCallback?.Report((((double)i / (double)Count)));
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
        public IProgress<double?> OverallCallback { get; set; }
        public IProgress<double?> CurrentCallback { get; set; }
        public ILogger Logger { get; set; } = DummyLogger.Instance;
    }
}
