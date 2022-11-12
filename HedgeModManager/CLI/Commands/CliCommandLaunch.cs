using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.CLI.Commands
{
    [CliDeprecatedCommandName("-profile")]
    [CliCommand("launch", "l", null, "Launches the selected game", "--launch", "--game SonicLostWorld --launch")]
    public class CliCommandLaunch : ICliCommand
    {
        public void Execute(List<CommandLine.Command> commands, CommandLine.Command command)
        {
            HedgeApp.CurrentGameInstall?.StartGame(HedgeApp.Config.UseLauncher || HedgeApp.IsLinux);
            HedgeApp.Current.Shutdown();
        }
    }
}
