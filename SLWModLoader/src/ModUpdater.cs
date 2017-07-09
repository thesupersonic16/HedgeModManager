using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SLWModLoader
{
    public class ModUpdater
    {
        protected WebClient WebClient = new WebClient();

        // Don't know why I've made this, Shouldn't have existed
        public ModUpdate GetUpdateFromINI(Mod mod, string data)
        {
            var ini = new IniFile();
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(data)))
                ini.Read(new StreamReader(stream));
            return GetUpdateFromINI(mod, ini);
        }


        // Don't know why I've made this, Shouldn't have existed
        public ModUpdate GetUpdateFromINI(Mod mod, IniFile ini)
        {
            // Checks
            if (!ini.ContainsGroup("Main"))
                return null;
            if (!ini["Main"].ContainsParameter("VersionString"))
                return null;

            if (!ini["Main"].ContainsParameter("DownloadSizeString"))
                return null;

            if (!ini["Main"].ContainsParameter("ChangeLog"))
                return null;

            if (!ini["Main"].ContainsParameter("FileList"))
                return null;

            // Variables
            string modVersion = ini["Main"]["VersionString"];
            string modDLSize = ini["Main"]["DownloadSizeString"];
            string modChangeLog = ini["Main"]["ChangeLog"];
            string modFileList = ini["Main"]["FileList"];

            // Checks if the mod isn't the lastest
            if (modVersion != mod.Version)
            {
                var update = new ModUpdate();
                update.Name = mod.Title;
                update.VersionString = modVersion;
                update.ChangeLog = modChangeLog;
                ReadV0UpdateFileList(update, WebClient.DownloadString(modFileList));
                return update;
            }
            else
                return null;
            

            // Sub-Methods
            void ReadV0UpdateFileList(ModUpdate update, string data)
            {
                var files = new Dictionary<string, Tuple<string, string>>();
                // Splits all the lines in mod_update_files.txt into an array
                var split = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                // Adds the file name and url to the files array
                foreach (string line in split)
                {
                    // Checks if the line starts with ';' or '#' if does then continue to the next line
                    if (line.StartsWith(";") || line.StartsWith("#"))
                        continue;

                    var file = new ModUpdateFile()
                    {
                        SHA256 = line.Split(':')[0],
                        FileName = line.Split(':')[1],
                        URL = line.Substring(line.IndexOf(":", line.IndexOf(":") + 1) + 1)
                    };
                    update.Files.Add(file);
                }
            }
        }

        // Loads update files from a TXT file for Radfordhound's Modloader
        public ModUpdate GetUpdateFromTXT(Mod mod, string data)
        {
            // TODO
            return null;
        }

        // Loads update files from an XML for SuperSonic16's Modloader
        public ModUpdate GetUpdateFromXML(Mod mod, XDocument xml)
        {
            // Checks
            if (xml.Root == null)
                return null;

            if (xml.Root.Element("UpdateInfo") == null)
                return null;

            var root = xml.Root;
            var modUpdateInfo = xml.Root.Element("UpdateInfo");
            var modUpdateFiles = xml.Root.Element("UpdateFiles");
            var modVersionAtt = modUpdateInfo.Attribute("version");
            var modDownloadSizeAtt = modUpdateInfo.Attribute("downloadSize");

            // Checks
            if (modVersionAtt == null || modDownloadSizeAtt == null || modUpdateFiles == null)
                return null;

            var update = new ModUpdate()
            {
                VersionString = modVersionAtt.Value,
                Name = mod.Title,
                DownloadSizeString = modDownloadSizeAtt.Value,
                ChangeLog = modUpdateInfo.Value
            };

            // Reads the list of files
            foreach (var element in modUpdateFiles.Elements())
            {
                if (element.Name != "UpdateFile")
                    continue;

                var filePathAtt = element.Element("FilePath");
                var URLAtt = element.Element("URL");
                var SHA256Att = element.Element("SHA256");

                // Checks
                if (filePathAtt == null || URLAtt == null)
                    continue;

                if (Path.IsPathRooted(filePathAtt.Value) ||
                    filePathAtt.Value.Contains(".."))
                    continue; // Path must not go back or be absolute

                var file = new ModUpdateFile();
                file.FileName = filePathAtt.Value;
                file.URL = URLAtt.Value;

                // Convert any Download links
                file.URL = DownloadTools.GetDirectDownloadURL(file.URL);

                if (SHA256Att != null)
                    file.SHA256 = SHA256Att.Value;

                update.Files.Add(file);
            }

            return update;
        }
        
        
        public class ModUpdate
        {
            public string Name, VersionString, DownloadSizeString, ChangeLog;
            public List<ModUpdateFile> Files = new List<ModUpdateFile>();
        }

        public class ModUpdateFile
        {
            public string FileName, URL, SHA256;
        }

    }
}
