using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HedgeModManager.Updates
{
    public interface IUpdateCommand
    {
        void Parse(string line);
        Task PrepareExecute(ModInfo mod, ILogger logger);
        Task Execute(ModInfo mod, ExecuteConfig config = default, CancellationToken cancellationToken = default);
        Task FinalizeExecute(ModInfo mod, ILogger logger);
    }
}
