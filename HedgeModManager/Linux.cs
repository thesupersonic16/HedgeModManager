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

        public static bool GenerateDesktop()
        {
            string baseExec = null;
            string icon = null;
            // Check if bottles is used
            if (Environment.GetEnvironmentVariable("FLATPAK_ID") == "com.usebottles.bottles")
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
                foreach (Game game in Games.GetSupportedGames())
                    GenerateDesktopAndRegister($"{game.GBProtocol}.desktop", $"Hedge Mod Manager ({game.GameName})", $"{baseExec} -gb \"%u\"", icon, false, $"x-scheme-handler/{game.GBProtocol}");
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

            return true;
        }

        // Wine
        public static string GetHomeDirectory() => Environment.GetEnvironmentVariable("WINEHOMEDIR").Replace("\\??\\", "");
        public static string GetPrefixDirectory() => Environment.GetEnvironmentVariable("WINEPREFIX").Replace("\\??\\", "");

        public static string ConvertToUnix(string windowsPath) => windowsPath.Replace('\\', '/').Replace("Z:", "");

        // Bottles
        public static string GetBottleConfigPath() => Path.Combine(GetPrefixDirectory(), "bottle.yml");

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
