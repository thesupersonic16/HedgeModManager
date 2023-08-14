using HedgeModManager.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public static class ModLoaders
    {
        public static ModLoader GenerationsCodeLoader = new ModLoader()
        {
            ModLoaderDownloadURL = Resources.URL_GCL_DL,
            ModLoaderData = EmbeddedLoaders.GenerationsCodeLoader,
            ModLoaderName = "Generations Code Loader",
            ModLoaderFileName = "d3d9.dll",
            DirectXVersion = 9,
        };

        public static ModLoader LostCodeLoader = new ModLoader()
        {
            ModLoaderDownloadURL = Resources.URL_LCL_DL,
            ModLoaderData = EmbeddedLoaders.LostCodeLoader,
            ModLoaderName = "Lost Code Loader",
            ModLoaderFileName = "d3d9.dll",
            DirectXVersion = 9,
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
    }
}
