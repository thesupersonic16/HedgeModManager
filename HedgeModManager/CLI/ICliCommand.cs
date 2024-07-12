using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.CLI
{
    public interface ICliCommand
    {
        void Execute(List<CommandLine.Command> commands, CommandLine.Command command);
    }
}
