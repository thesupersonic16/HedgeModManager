using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.CLI.Commands
{
    [CliDeprecatedCommandName("-save")]
    [CliCommand("save", "s", null, "Saves the configuration from other start options", "--save", "--profile \"Sonic Unleashed\" --save")]
    public class CliCommandSave : ICliCommand
    {
        public void Execute(List<CommandLine.Command> commands, CommandLine.Command command)
        {
            var window = HedgeApp.Current.MainWindow as MainWindow;

            // Profiles
            window.RefreshProfiles();
            HedgeApp.Config.ModsDbIni = Path.Combine(HedgeApp.ModsDbPath, window.SelectedModProfile.ModDBPath);
            HedgeApp.Config.Save(HedgeApp.ConfigPath);

        }
    }
}
