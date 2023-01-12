using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.CLI.Commands
{
    // TODO: Add a check to see if the profile exists
    // TODO: Handle profile configs
    [CliDeprecatedCommandName("-profile")]
    [CliCommand("profile", "p", new[] { typeof(string) }, "Set selected profile", "--profile {ProfileName}", "--profile \"Sonic Unleashed\"")]
    public class CliCommandProfile : ICliCommand
    {
        public void Execute(List<CommandLine.Command> commands, CommandLine.Command command)
        {
            string profileName = (string)command.Inputs[0];
            HedgeApp.Config.ModProfile = profileName;
        }
    }
}
