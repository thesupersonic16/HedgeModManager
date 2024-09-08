using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.CLI.Commands
{
    [CliDeprecatedCommandName("-game")]
    [CliCommand("game", "g", new [] { typeof(string) }, "Set selected game", "--game {GameName}", "--game SonicFrontiers")]
    public class CliCommandGame : ICliCommand
    {
        public void Execute(List<CommandLine.Command> commands, CommandLine.Command command)
        {
            string gameName = (string)command.Inputs[0];
            HedgeApp.SelectGameInstall(HedgeApp.GameInstalls.FirstOrDefault(
                t => t.Game.GameName.ToLowerInvariant() == gameName.ToLowerInvariant()));
        }
    }
}
