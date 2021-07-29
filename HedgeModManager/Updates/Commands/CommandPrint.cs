using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HedgeModManager.Updates.Commands
{
    [UpdateCommand(Name)]
    public class CommandPrint : IUpdateCommand
    {
        public const string Name = "print";

        public string Text { get; set; }

        public CommandPrint()
        {

        }

        public CommandPrint(string text)
        {
            Text = text;
        }

        public void Parse(string line)
        {
            Text = line;
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
}
