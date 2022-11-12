using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.CLI.Commands
{
    [CliCommand("decrypt", null, new[] { typeof(string) }, "Decrypts reporting file (Private key is required)", "--decrypt {filePath} [output]", "--decrypt data.txt data.zip")]
    public class CliCommandDecrypt : ICliCommand
    {
        public void Execute(List<CommandLine.Command> commands, CommandLine.Command command)
        {
            string filePath = (string)command.Inputs[0];
            var outputFileName = Path.ChangeExtension(filePath, string.Empty);
            if (command.Inputs.Count > 1)
                outputFileName = $"{(string)command.Inputs[1]}";

            if (filePath.ToLowerInvariant().EndsWith(".txt"))
            {
                byte[] data = Convert.FromBase64String(File.ReadAllText(filePath));

                using (var encrypted = new MemoryStream(data))
                using (var decrypted = File.Create(outputFileName))
                {
                    CryptoProvider.Decrypt(encrypted, decrypted);
                    Console.WriteLine($"Successfully decrypted {outputFileName}");
                }
            } else
            {
                using (var encrypted = File.OpenRead(filePath))
                using (var decrypted = File.Create(outputFileName))
                {
                    CryptoProvider.Decrypt(encrypted, decrypted);
                    Console.WriteLine($"Successfully decrypted {outputFileName}");
                }
            }
            HedgeApp.Current.Shutdown();
        }
    }
}
