using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HedgeModManager.Updates.Commands
{
    [UpdateCommand(Name)]
    public class CommandPause : IUpdateCommand
    {
        public const string Name = "pause";
        public int Timeout { get; set; }

        public CommandPause()
        {

        }

        public CommandPause(int timeout)
        {
            Timeout = timeout;
        }

        public void Parse(string line)
        {
            if (int.TryParse(line, out int timeout))
                Timeout = timeout;
            else
                Timeout = -1;
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
