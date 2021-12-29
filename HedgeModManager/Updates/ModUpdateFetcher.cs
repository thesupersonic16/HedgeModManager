using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HedgeModManager.Updates
{
    public class ModUpdateFetcher
    {
        private static async Task<IModUpdateInfo> GetInfo(ModInfo mod, CancellationToken cancellationToken = default)
        {
            IModUpdateInfo info = null;

            var versionInfo = await ModVersionInfo.ParseFromServerAsync(mod.UpdateServer, cancellationToken)
                .ConfigureAwait(false);

            if (versionInfo == null)
            {
                return null;
            }

            info = await ModUpdateModern.GetUpdateAsync(versionInfo, mod, cancellationToken)
                .ConfigureAwait(false);

            if (info == null)
                info = await ModUpdateGmi.GetUpdateAsync(versionInfo, mod, cancellationToken).ConfigureAwait(false);

            return info;
        }

        public static async Task<IReadOnlyList<IModUpdateInfo>> FetchUpdates(IEnumerable<ModInfo> mods, NetworkConfig config = null, 
            Action<ModInfo, Status, Exception> statusCallback = null, CancellationToken cancellationToken = default)
        {
            var updates = new List<IModUpdateInfo>();
            foreach (var mod in mods)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!mod.HasUpdates)
                    continue;

                if (config != null && config.IsServerBlocked(mod.UpdateServer))
                {
                    statusCallback?.Invoke(mod, Status.Blocked, null);
                    continue;
                }

                IModUpdateInfo info = null;

                statusCallback?.Invoke(mod, Status.BeginCheck, null);

                try
                {
                    info = await GetInfo(mod, cancellationToken);
                }
                catch(Exception e)
                {
                    statusCallback?.Invoke(mod, Status.Failed, e);
                    continue;
                }

                if (info == null)
                {
                    statusCallback?.Invoke(mod, Status.Failed, null);
                    continue;
                }

                if (info.Version != mod.Version)
                    updates.Add(info);
                statusCallback?.Invoke(mod, Status.Success, null);
            }

            return updates;
        }

        public static async Task<FetchResult> FetchUpdate(ModInfo mod, NetworkConfig config = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!mod.HasUpdates)
                return new FetchResult(Status.NoUpdates, mod, null);

            if (config != null && config.IsServerBlocked(mod.UpdateServer))
                return new FetchResult(Status.Blocked, mod, null);

            IModUpdateInfo info = null;
            try
            {
                info = await GetInfo(mod, cancellationToken);
            }
            catch (Exception e)
            {
                return new FetchResult(Status.Failed, mod, null, e);
            }

            if (info == null)
                return new FetchResult(Status.Failed, mod, null);

            if (info.Version != mod.Version)
            {
                return new FetchResult(Status.Success, mod, info);
            }

            return new FetchResult(Status.UpToDate, mod, null); ;
        }

        public enum Status
        {
            NoUpdates,
            BeginCheck,
            Failed,
            Blocked,
            Success,
            UpToDate
        }

        public struct FetchResult
        {
            public Status Status { get; set; }
            public ModInfo Mod { get; set; }
            public IModUpdateInfo UpdateInfo { get; set; }
            public Exception FailException { get; set; }

            public FetchResult(Status status, ModInfo mod, IModUpdateInfo updateInfo, Exception failException)
            {
                Status = status;
                Mod = mod;
                UpdateInfo = updateInfo;
                FailException = failException;
            }

            public FetchResult(Status status, ModInfo mod, IModUpdateInfo updateInfo) : this(status, mod, updateInfo, null)
            {

            }
        }
    }
}
