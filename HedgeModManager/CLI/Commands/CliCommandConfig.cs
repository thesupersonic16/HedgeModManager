using HedgeModManager.UI;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace HedgeModManager.CLI.Commands
{
    [CliCommand("config", "c", new[] { typeof(string) }, "Open config window for a mod", "--config {ModPath}", "--config \"C:\\Example Mod\\\"")]
    public class CliCommandConfig : ICliCommand
    {
        public void Execute(List<CommandLine.Command> commands, CommandLine.Command command)
        {
            string modPath = (string)command.Inputs[0];

            if (File.Exists(modPath) && Path.GetExtension(modPath) == ".ini")
                modPath = Path.GetDirectoryName(modPath);

            if (Directory.Exists(modPath))
            {
                var modInfo = new ModInfo(modPath);

                if (modInfo.HasSchema)
                {
                    new ModConfigWindow(modInfo)
                    {
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    }
                    .ShowDialog();
                }
            }

            HedgeApp.Current.Shutdown(0);
        }
    }
}
