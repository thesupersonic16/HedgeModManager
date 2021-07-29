using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HedgeModManager.Updates
{
    public class ModVersionInfo
    {
        public const string FileName = "mod_version.ini";
        public string Version { get; set; }
        public string Changelog { get; set; }
        public string ChangeLogPath { get; set; }
        public string BaseFileTreePath { get; set; }

        public string DownloadSize { get; set; }
        public UpdateType Type { get; set; } = UpdateType.Gmi;

        public static Task<ModVersionInfo> ParseFromServerAsync(string server, CancellationToken cancellationToken = default, string fileName = FileName)
        {
            string path = Path.Combine(server, fileName);
            return ParseFromWebAsync(path, cancellationToken);
        }

        public static async Task<ModVersionInfo> ParseFromWebAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                var httpResult = await Singleton.GetInstance<HttpClient>().GetAsync(url, cancellationToken).ConfigureAwait(false);
                if (!httpResult.IsSuccessStatusCode)
                    return null;

                var info = new ModVersionInfo();
                var contentStream = await httpResult.Content.ReadAsStreamAsync().ConfigureAwait(false);
                if (!info.Parse(contentStream))
                    return null;

                return info;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public bool Parse(Stream stream)
        {
            var file = new IniFile(stream);
            return Parse(file);
        }

        public bool Parse(IniFile ini)
        {
            if (!ini.Groups.ContainsKey("Main"))
                return false;

            if (!ini["Main"].Params.ContainsKey("VersionString"))
                return false;
            
            if (int.TryParse(ini["Main"]["UpdaterType"], out int updateType))
                Type = (UpdateType)updateType;

            Version = ini["Main"]["VersionString"];
            ChangeLogPath = ini["Main"]["Markdown"];
            DownloadSize = ini["Main"]["DownloadSizeString"];
            BaseFileTreePath = ini["Main"]["BaseFileTree"];

            if (int.TryParse(ini["Changelog"]["StringCount"], out int modChangeLogLineCount))
            {
                for (int i = 0; i < modChangeLogLineCount; ++i)
                    Changelog += $"- {ini["Changelog"][$"String{i}"]}\n";
            }

            return true;
        }

        public async Task<ModFileTree> GetBaseFileTree(string server, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(server))
                return null;

            if (string.IsNullOrEmpty(BaseFileTreePath))
                return null;

            try
            {
                return await ModFileTree.LoadFromUrl(Path.Combine(server, BaseFileTreePath), cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }

    public enum UpdateType
    {
        Gmi = 0,
        HmmV1 = 1,
    }
}
