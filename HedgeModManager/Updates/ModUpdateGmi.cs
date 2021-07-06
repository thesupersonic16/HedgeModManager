using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.Updates
{
    public class ModUpdateGmi : IModUpdateInfo
    {
        public ModInfo Mod { get; internal set; }
        public string Version { get; internal set; }

        protected List<string> Commands { get; set; }

        public async Task<string> GetChangelog()
        {
            return string.Empty;
        }

        public static async Task<ModUpdateGmi> GetUpdate(ModInfo mod)
        {
            if (string.IsNullOrEmpty(mod.UpdateServer))
                return null;

            try
            {
                string modVersionPath = Path.Combine(mod.UpdateServer, "mod_version.ini");
                string modFilesPath = Path.Combine(mod.UpdateServer, "mod_files.txt");
                
                var versionResult = await HedgeApp.HttpClient.GetAsync(modVersionPath);
                if (!versionResult.IsSuccessStatusCode)
                    return null;

                var filesResult = await HedgeApp.HttpClient.GetAsync(modFilesPath);
                if (!filesResult.IsSuccessStatusCode)
                    return null;

                var update = new ModUpdateGmi
                {
                    Mod = mod
                };

                return update;
            }
            catch
            {
                return null;
            }
        }
    }
}
