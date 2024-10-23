using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace HedgeModManager
{
    public static class Linux
    {

        /// <summary>
        /// Performs any patches needed to let HedgeModManager to operate correctly
        /// </summary>
        /// <returns></returns>
        public static bool PatchHMMRegistry()
        {
            var regCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
            if (regCurrentUser != null)
            {
                var dllOverrides = regCurrentUser.CreateSubKey("Software\\Wine\\DllOverrides");

                if (dllOverrides != null)
                {
                    dllOverrides.SetValue("d3d11", "native,builtin");
                    dllOverrides.SetValue("dinput8", "native,builtin");
                }
            }

            return true;
        }

        /// <summary>
        /// Performs any patches needed for mods to execute correctly
        /// </summary>
        /// <param name="game">Game to apply the patch to</param>
        /// <returns></returns>
        public static bool PatchRegistry(Game game)
        {
            if (game == null)
                return false;

            string prefixPath = ConvertToUnix(Path.Combine(Steam.GetProtonPrefixPath(game.AppID)));
            string userReg = Path.Combine(prefixPath, "user.reg");

            // Check if registry exists
            if (!File.Exists(userReg))
                return false;

            // Registry data
            byte[] reg = File.ReadAllBytes(userReg);
            
            string modLoaderFileName = 
                Path.GetFileNameWithoutExtension(game.ModLoader.ModLoaderFileName);
            string group = @"[Software\\Wine\\DllOverrides]";
            string key = $"\"{modLoaderFileName}\"=\"native,builtin\"";

            // Scan
            if (HedgeApp.BoyerMooreSearch(reg, Encoding.UTF8.GetBytes(key)) == -1)
                File.AppendAllText(userReg, $"\n{group}\n{key}");

            return true;
        }

        public static bool GenerateDesktop()
        {
            string baseExec = null;
            string icon = null;
            // Check if bottles is used
            if (IsBottles())
            {
                // Parse bottle config
                var deserializer = new DeserializerBuilder()
                    .IgnoreUnmatchedProperties().Build();
                var config = deserializer.Deserialize<BottleConfig>(File.ReadAllText(GetBottleConfigPath()));
                var program = config.External_Programs?.FirstOrDefault(x => x.Value.name == "HedgeModManager").Value;

                // Ignore if program is not found
                if (program == null)
                    return false;

                string iconFolder = Path.Combine(ConvertToUnix(GetHomeDirectory()), ".local/share/icons/hicolor/256x256/apps");

                ExtractIcon(icon = Path.Combine(iconFolder, "hedgemodmanager_icon.png"));

                baseExec = $"flatpak run --command=bottles-cli com.usebottles.bottles run -b {config.Name} -e \"{program.path}\" --args";
            }

            if (baseExec != null)
            {
                GenerateDesktopAndRegister("hedgemm.desktop", "Hedge Mod Manager", $"{baseExec} \"%u\"", icon, true, "x-scheme-handler/hedgemm");

                // GameBanana
                foreach (var game in Games.GetSupportedGames()
                    .GroupBy(t => t.GBProtocol)
                    .Select(t => t.First()))
                    GenerateDesktopAndRegister($"{game.GBProtocol}.desktop", $"Hedge Mod Manager ({game.GameName})", $"{baseExec} \"-gb %u\"", icon, false, $"x-scheme-handler/{game.GBProtocol}");
            }

            return true;

            void ExtractIcon(string filePath)
            {
                Properties.Resources.IMG_HEDGEMODMANAGER.Save(filePath);
            }
        }

        public static bool GenerateDesktopAndRegister(string fileName, string name, string exec, string icon, bool display, string mimeType)
        {
            string directory = Path.Combine(GetHomeDirectory(), ".local/share/applications");
            string mimeappsPath = Path.Combine(GetHomeDirectory(), ".config/mimeapps.list");
            string mimeCachePath = Path.Combine(directory, "mimeinfo.cache");
            string filePath = Path.Combine(directory, fileName);

            // Create applications folder
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Create desktop file
            var ini = new IniFile();
            var group = ini["Desktop Entry"];
            ini.UseQuotes = false;
            group["Type"] = "Application";
            group["Name"] = name;
            group["Exec"] = exec;
            group["Comment"] = "A mod manager for Hedgehog Engine games on PC.";
            group["Icon"] = icon.Replace('\\', '/');
            group["StartupNotify"] = "false";
            group["NoDisplay"] = (!display).ToString().ToLower();
            group["MimeType"] = mimeType;
            group["Categories"] = "Game";

            // Write desktop file
            using (var stream = File.OpenWrite(filePath))
                ini.Write(stream);

            // Register desktop file
            IniFile mimeapps;
            if (!File.Exists(mimeappsPath))
                mimeapps = new IniFile();
            else
                mimeapps = new IniFile(mimeappsPath);

            mimeapps.UseQuotes = false;
            mimeapps["Default Applications"][mimeType] = fileName;

            using (var stream = File.OpenWrite(mimeappsPath))
                mimeapps.Write(stream);

            // Write to cache
            IniFile mimeCache;
            if (!File.Exists(mimeCachePath))
                mimeCache = new IniFile();
            else
                mimeCache = new IniFile(mimeCachePath);

            mimeCache.UseQuotes = false;
            mimeCache["MIME Cache"][mimeType] = fileName + ';';

            using (var stream = File.OpenWrite(mimeCachePath))
                mimeCache.Write(stream);

            return true;
        }

        public static void LinkRuntimeToProtonPrefix(Game game)
        {
            if (CheckDotNetRuntime(game.AppID) == false && IsBottles() && game.Is64Bit)
            {
                // Prefix paths
                string protonPath = ConvertToUnix(Path.Combine(Steam.GetProtonPrefixPath(game.AppID)));
                string bottlePath = ConvertToUnix(GetPrefixDirectory());

                // Directories
                string protonDotNet = Path.Combine(protonPath, "drive_c\\windows\\Microsoft.NET");
                string bottleDotNet = Path.Combine(bottlePath, "drive_c\\windows\\Microsoft.NET");
                string protonSys32 = Path.Combine(protonPath, "drive_c\\windows\\system32");
                string bottleSys32 = Path.Combine(bottlePath, "drive_c\\windows\\system32");

                // Check if the prefix exists
                if (!Directory.Exists(protonSys32))
                    return;

                if (Directory.Exists(protonDotNet))
                    Directory.Move(protonDotNet, protonDotNet + ".bak");
                RunUnixCommand($"/usr/bin/ln -s {ConvertToUnix(bottleDotNet)} {ConvertToUnix(protonDotNet)}");

                var dlls = new List<string>();
                dlls.Add("mscoree.dll");
                dlls.AddRange(Directory.EnumerateFiles(bottleSys32)
                    .Select(x => Path.GetFileName(x))
                    .Where(x => x.StartsWith("ucrt")));
                dlls.AddRange(Directory.EnumerateFiles(bottleSys32)
                    .Select(x => Path.GetFileName(x))
                    .Where(x => x.StartsWith("vcruntime")));

                foreach (string dll in dlls)
                {
                    if (File.Exists(Path.Combine(protonSys32, dll)))
                        RunUnixCommand($"/usr/bin/rm {ConvertToUnix(Path.Combine(protonSys32, dll))}");
                    RunUnixCommand($"/usr/bin/ln -s {ConvertToUnix(Path.Combine(bottleSys32, dll))} {ConvertToUnix(Path.Combine(protonSys32, dll))}");
                }
            }
        }

        // Hope it works
        public static bool? CheckDotNetRuntime(string gameId)
        {
            string protonPath = ConvertToUnix(Steam.GetProtonPrefixPath(gameId));
            if (!Directory.Exists(Path.Combine(protonPath, "drive_c\\windows\\Microsoft.NET")))
                return null;
            if (!Directory.Exists(Path.Combine(protonPath, "drive_c\\windows\\Microsoft.NET\\assembly")))
                return false;

            return true;
        }

        // Wine
        public static void RunUnixCommand(string command)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"start",
                Arguments = $"/b /wait /unix {command}",
                UseShellExecute = false
            }).WaitForExit(200);
        }

        public static string GetHomeDirectory() => Environment.GetEnvironmentVariable("WINEHOMEDIR").Replace("\\??\\", "");
        public static string GetPrefixDirectory() => Environment.GetEnvironmentVariable("WINEPREFIX").Replace("\\??\\", "");

        public static string ConvertToUnix(string windowsPath) => windowsPath.Replace('\\', '/').Replace("Z:", "");

        // Bottles
        public static string GetBottleConfigPath() => Path.Combine(GetPrefixDirectory(), "bottle.yml");
        public static bool IsBottles() => Environment.GetEnvironmentVariable("FLATPAK_ID") == "com.usebottles.bottles";

        public class BottleConfig
        {
            public Dictionary<string, Program> External_Programs { get; set; }
            public string Name { get; set; }

            public class Program
            {
                public string executable { get; set; }
                public string name { get; set; }
                public string path { get; set; }
                public string folder { get; set; }
            }
        }
    }
}
