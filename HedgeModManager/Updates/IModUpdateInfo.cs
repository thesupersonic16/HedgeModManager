using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HedgeModManager.Updates
{
    public interface IModUpdateInfo
    {
        public ModInfo Mod { get; }
        public string Version { get; }
        public bool SingleFileMode { get; }
        public Task<string> GetChangelog();
        public Task ExecuteAsync(ExecuteConfig config = default, CancellationToken cancellationToken = default);
    }
}
