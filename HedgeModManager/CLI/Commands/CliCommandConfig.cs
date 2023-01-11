using HedgeModManager.UI;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace HedgeModManager.CLI.Commands
{
    [CliCommand("config", "c", new[] { typeof(string) }, "Open config window for a mod", "--config \"{ModPath}\"", "--config \"C:\\Example Mod\\\"")]
    public class CliCommandConfig : ICliCommand
    {
        public void Execute(List<CommandLine.Command> commands, CommandLine.Command command)
        {
            string modPath = (string)command.Inputs[0];

            if (Directory.Exists(modPath))
            {
                var config = new ModConfigWindow(new ModInfo(modPath));
                {
                    config.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }

                config.ShowDialog();

                HedgeApp.Current.Shutdown(0);
            }
        }
    }
}
