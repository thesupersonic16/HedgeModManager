using HedgeModManager.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public static class ModLoaders
    {
        public static ModLoader HE1ModLoader = new ModLoader()
        {
            ModLoaderDownloadURL = Resources.URL_HE1ML_DL,
            ModLoaderData = EmbeddedLoaders.HE1ModLoader,
            ModLoaderName = "Hedgehog Engine 1 Mod Loader",
            ModLoaderID = "HE1ModLoader",
            ModLoaderFileName = "dinput8.dll",
            DirectXVersion = 9,

            IncompatibleFiles = new []{ "d3d9.dll" },
            IncompatibleFileCallback = ModLoader.IncompatibleByOriginalName("GenerationsCodeLoader.dll", "LostCodeLoader.dll")
        };

        public static ModLoader HE2ModLoader = new ModLoader()
        {
            ModLoaderDownloadURL = Resources.URL_HE2ML_DL,
            ModLoaderData = EmbeddedLoaders.HE2ModLoader,
            ModLoaderName = "Hedgehog Engine 2 Mod Loader",
            ModLoaderID   = "HE2ModLoader",
            ModLoaderFileName = "d3d11.dll",
            DirectXVersion = 11,
        };

        public static ModLoader RainbowModLoader = new ModLoader()
        {
            ModLoaderDownloadURL = Resources.URL_RML_DL,
            ModLoaderData = EmbeddedLoaders.RainbowModLoader,
            ModLoaderName = "Rainbow Mod Loader",
            ModLoaderID = "RainbowModLoader",
            ModLoaderFileName = "d3d11.dll",
            DirectXVersion = 11
        };

        public static ModLoader HiteModLoader = new ModLoader()
        {
            ModLoaderDownloadURL = Resources.URL_HML_DL,
            ModLoaderData = EmbeddedLoaders.HiteModLoader,
            ModLoaderName = "Hite Mod Loader",
            ModLoaderID = "HiteModLoader",
            ModLoaderFileName = "d3d11.dll",
            DirectXVersion = 11,
        };

    }

    public class ModLoader
    {
        public string ModLoaderDownloadURL = string.Empty;
        public byte[] ModLoaderData = null;
        public string ModLoaderName = "None";
        public string ModLoaderID   = null;
        public string ModLoaderFileName = string.Empty;
        public uint DirectXVersion  = uint.MaxValue;
        public string[] IncompatibleFiles = Array.Empty<string>();
        public Func<string, bool> IncompatibleFileCallback = null;

        public static Func<string, bool> IncompatibleByOriginalName(params string[] originalNames)
        {
            return fileName =>
            {
                var info = FileVersionInfo.GetVersionInfo(fileName);
                if (originalNames.Contains(info.OriginalFilename))
                {
                    File.Delete(fileName);
                }

                return true;
            };
        }

        public bool MakeCompatible(string root)
        {
            foreach (string file in IncompatibleFiles)
            {
                var fullPath = Path.Combine(root, file);
                if (!File.Exists(fullPath))
                {
                    continue;
                }

                if (IncompatibleFileCallback != null)
                {
                    if (!IncompatibleFileCallback(fullPath))
                    {
                        return false;
                    }
                }
                else
                {
                    File.Delete(fullPath);
                }
            }
            return true;
        }
    }
}
