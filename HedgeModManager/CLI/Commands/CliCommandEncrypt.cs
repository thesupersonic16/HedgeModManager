using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.CLI.Commands
{
    [CliCommand("encrypt", null, new[] { typeof(string) }, "Encrypts file for reporting", "--encrypt {filePath} [output]", "--encrypt data.bin")]
    public class CliCommandEncrypt : ICliCommand
    {
        public void Execute(List<CommandLine.Command> commands, CommandLine.Command command)
        {
            string filePath = (string)command.Inputs[0];
            var outputFileName = $"{filePath}.bytes";
            if (command.Inputs.Count > 1)
                outputFileName = $"{(string)command.Inputs[1]}.bytes";

            using (var file = File.OpenRead(filePath))
            using (var encrypted = File.Create(outputFileName))
            {
                CryptoProvider.Encrypt(file, encrypted);
                Console.WriteLine($"Successfully encrypted {outputFileName}");
            }
            HedgeApp.Current.Shutdown();
        }
    }
}
