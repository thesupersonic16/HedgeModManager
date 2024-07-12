using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.CLI.Commands
{
    [CliCommand("help", "h", null, "Lists commands", "--help OR -h")]
    public class CliCommandHelp : ICliCommand
    {
        public void Execute(List<CommandLine.Command> commands, CommandLine.Command command)
        {
            CommandLine.ShowHelp();
            HedgeApp.Current.Shutdown();
        }
    }
}
