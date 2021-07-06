using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.Updates
{
    public class ModUpdateFetcher
    {
        public static async Task<IReadOnlyList<IModUpdateInfo>> FetchUpdates(IEnumerable<ModInfo> mods, NetworkConfig config = null)
        {
            var updates = new List<IModUpdateInfo>();

            foreach (var mod in mods)
            {
                if (config != null && config.IsServerBlocked(mod.UpdateServer))
                    continue;

                IModUpdateInfo info = null;

                info = await ModUpdateGmi.GetUpdate(mod);

                if (info != null)
                    updates.Add(info);
            }

            return updates;
        }
    }
}
