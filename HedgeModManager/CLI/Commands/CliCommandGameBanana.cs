using GameBananaAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.CLI.Commands
{
    [CliCommand("gb", "gb", new[] { typeof(string) }, "Launches GameBanana download window", "--gb {protocol}:{downloadURL},{itemType},{itemID}")]
    public class CliCommandGameBanana : ICliCommand
    {
        public void Execute(List<CommandLine.Command> commands, CommandLine.Command command)
        {
            string url = (string)command.Inputs[0];
            GBAPI.ParseCommandLine(url);
            HedgeApp.Current.Shutdown();
        }
    }
}
