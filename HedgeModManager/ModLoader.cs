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

            IncompatibleFiles = new []{ "d3d9.dll", "cpkredir.dll", "cpkredir.txt" },
            IncompatibleFileCallback = ModLoader.IncompatibleByOriginalName("GenerationsCodeLoader.dll", "LostCodeLoader.dll", "cpkredir.dll") 
                                       + ModLoader.IncompatibleByMD5(("cpkredir.txt", "A095F4BFFC3B3E5D76F3B30EE84AB867"))
        };

        public static ModLoader HE2ModLoader = new ModLoader()
        {
            ModLoaderDownloadURL = Resources.URL_HE2ML_DL,
            ModLoaderData = EmbeddedLoaders.HE2ModLoader,
            ModLoaderName = "Hedgehog Engine 2 Mod Loader",
            ModLoaderID   = "HE2ModLoader",
            ModLoaderFileName = "dinput8.dll",
            DirectXVersion = 11,
            IncompatibleFiles = new[] { "d3d11.dll" },
            IncompatibleFileCallback = ModLoader.IncompatibleByOriginalName("HE2ModLoader.dll")
        };

        // Temporary workaround for Sonic Forces
        public static ModLoader HE2ModLoaderD3D11 = new ModLoader()
        {
            ModLoaderDownloadURL = Resources.URL_HE2ML_DL,
            ModLoaderData = EmbeddedLoaders.HE2ModLoader,
            ModLoaderName = "Hedgehog Engine 2 Mod Loader",
            ModLoaderID = "HE2ModLoader",
            ModLoaderFileName = "d3d11.dll",
            DirectXVersion = 11,
            IncompatibleFiles = new[] { "dinput8.dll" },
            IncompatibleFileCallback = ModLoader.IncompatibleByOriginalName("HE2ModLoader.dll")
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
            ModLoaderName = "Hite Mod Loader",
            ModLoaderID = "HiteModLoader",
            ModLoaderFileName = "dinput8.dll",
            DirectXVersion = 11,
            IncompatibleFiles = new[] { "d3d11.dll" },
            IncompatibleFileCallback = ModLoader.IncompatibleByOriginalName("HiteModLoader.dll")
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
                    try
                    {
                        File.Delete(fileName);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                return true;
            };
        }

        public static Func<string, bool> IncompatibleByMD5(params ValueTuple<string, string>[] types)
        {
            return fileName =>
            {
                var name = Path.GetFileName(fileName);
                var data = types.FirstOrDefault(x => x.Item1.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (data != default)
                {
                    var hash = HedgeApp.ComputeMD5Hash(fileName);
                    if (hash.Equals(data.Item2, StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            File.Delete(fileName);
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }
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
