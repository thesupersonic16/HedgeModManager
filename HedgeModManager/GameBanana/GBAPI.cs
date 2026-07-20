using HedgeModManager;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HedgeModManager.UI;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows;

namespace GameBananaAPI
{
    public class GBAPI
    {
        private static bool _serverRunning = false;
        private static CancellationTokenSource _serverCancellationTokenSource = new CancellationTokenSource();

        public static Dictionary<string, string> GameIDMappings = new()
        {
            { "6059" , "SonicGenerations" },
            { "6160" , "SonicForces" },
            { "6093" , "SonicLostWorld" },
            { "8707" , "PuyoPuyoTetris2" },
            { "11375", "SonicColorsUltimate" },
            { "15780", "SonicOrigins" },
            { "15779", "SonicFrontiers" },
            { "19886", "ShadowGenerations" },
            { "6559" , "UnleashedRecompiled" },
            { "21975", "UnleashedRecompiled" },
        };

        // TODO: Add Core/List support
        public enum GBAPIRequestType
        {
            COREITEMDATA
        }

        public static int GetGameBananaModID(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(url) && url.Contains("gamebanana.com/mods/"))
                {
                    // Read ID from URL
                    string id = url.Substring(url.IndexOf("mods/") + 5);
                    // Remove anything else after the ID if any
                    if (id.Contains("/"))
                        id = id.Substring(id.IndexOf("/"));
                    return int.Parse(id);
                }
            }
            catch
            {
                // ignored
            }

            return -1;
        }

        public class GBAPIRequestHandler
        {
            public string Suffix = "&format=json&return_keys=1";
            public GBAPIRequestType APIType = GBAPIRequestType.COREITEMDATA;

            /// <summary>
            /// Creates Request URL to GameBanana's API.
            ///  - Core/Item/Data
            ///      Calls Core/Item/Data with the specified item type, id and fields
            /// </summary>
            /// <returns>Request URL</returns>
            public async Task<string> BuildAsync(GBAPIItemData item)
            {
                if (APIType == GBAPIRequestType.COREITEMDATA)
                {
                    var supportedFields = await GetSupportedFieldsAsync(item.ItemType);
                    string URL = $"https://api.gamebanana.com/Core/Item/Data?itemtype={item.ItemType}&itemid={item.ItemID}&fields=";
                    foreach (var property in item.GetType().GetProperties())
                    {
                        var prop = (JsonPropertyAttribute)property.GetCustomAttribute(typeof(JsonPropertyAttribute));
                        if (prop != null && supportedFields.Contains(prop.PropertyName))
                        {
                            if (URL.Last() != '=')
                                URL += ',';
                            URL += prop.PropertyName;
                        }
                    }
                    return URL + Suffix;
                }
                return "";
            }

            /// <summary>
            /// Parses the response data(JSON) and writes all data into parameter "item".
            /// </summary>
            /// <param name="response">The Response data from GameBanana API in XML string format</param>
            /// <param name="item">Reference to a GBAPIItemData to write the data to</param>
            /// <returns>If Parse completed with no errors</returns>
            public bool ParseResponse(string response, ref GBAPIItemDataBasic item)
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<GBAPIItemDataBasic>(response);

                    if (obj != null)
                    {
                        item = obj;
                        return true;
                    }

                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// A normal Convert.ChangeType but returns null if fails
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object TryConvert(object obj, Type type)
        {
            try
            {
                return Convert.ChangeType(obj, type);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<List<string>> GetSupportedFieldsAsync(string itemType)
        {
            var url = $"https://api.gamebanana.com/Core/Item/Data/AllowedFields?itemtype={itemType}";
            try
            {
                return JsonConvert.DeserializeObject<List<string>>(await Singleton.GetInstance<HttpClient>().GetStringAsync(url));
            }
            catch
            {
                return new List<string>();
            }
        }

        public static async Task<GBAPIItemDataBasic> PopulateItemDataAsync(GBAPIItemDataBasic item)
        {
            var type = item.ItemType;
            var id = item.ItemID;
            var handler = new GBAPIRequestHandler();
            var request = await handler.BuildAsync(item);
            var response = await Singleton.GetInstance<HttpClient>().GetStringAsync(request);
            response = Uri.UnescapeDataString(response);
            if (!handler.ParseResponse(response, ref item))
                throw new Exception("Failed to parse GameBanana Item");

            item.ItemType = type;
            item.ItemID = id;

            // Populate file list
            foreach (var file in item.Files)
            {
                request = $"https://api.gamebanana.com/Core/Item/Data?itemtype=File&itemid={file.Key}&fields=aFlattenedFileList()&return_keys=1";
                response = await Singleton.GetInstance<HttpClient>().GetStringAsync(request);
                response = Uri.UnescapeDataString(response);
                file.Value.Files = JsonConvert.DeserializeObject<JObject>(response).First.First.ToObject<List<string>>();
            }

            return item;
        }

        /// <summary>
        /// Installs the GameBanana one-click install handler
        /// </summary>
        /// <returns></returns>
        public static bool InstallGBHandler(Game game)
        {
            string protocolName = $"HedgeModManager for {game}";
            try
            {
                var reg = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\{game.GBProtocol}");
                reg.SetValue("", $"URL:{protocolName}");
                reg.SetValue("URL Protocol", "");
                reg = reg.CreateSubKey("shell\\open\\command");
                reg.SetValue("", $"\"{HedgeApp.AppPath}\" -gb \"%1\"");
                reg.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool? ParseCommandLine(string line)
        {
            string[] split = line.Replace("https//", "https://").Split(',');

            if (line.StartsWith("hedgemm://gamebanana/pair", StringComparison.InvariantCultureIgnoreCase))
            {
                var splits = line.TrimEnd('/').Split('/');
                if (splits.Length < 2)
                    return false;
                string id = splits[splits.Length - 2];
                string key = splits[splits.Length - 1];

                RegistryConfig.GameBananaRemoteInstallID = id;
                RegistryConfig.GameBananaRemoteInstallKey = key;
                RegistryConfig.Save();

                HedgeApp.CreateOKMessageBox("Success", $"GameBanana Remote Install Paired Successfully").ShowDialog();
                _ = RunRemoteInstallServer(_serverCancellationTokenSource.Token);
                return false;
            }

            if (line.StartsWith("hedgemm://gamebanana/install", StringComparison.InvariantCultureIgnoreCase))
            {
                var splits = line.Substring("hedgemm://gamebanana/install".Length + 1).Split(',');

                if (splits.Length != 4)
                    return false;

                string gameID      = splits[0];
                string downloadURL = splits[1];
                string itemType    = splits[2];
                string itemID      = splits[3];
                string gameName    = GameIDMappings.ContainsKey(gameID) 
                    ? GameIDMappings[gameID] 
                    : gameID;

                if (int.TryParse(split[3], out int id))
                {
                    return new GBModWindow(itemType, id, downloadURL, gameName).ShowDialog();
                }
                else
                {
                    HedgeApp.CreateOKMessageBox("Error", $"Invalid GameBanana item id {split[2]}").ShowDialog();
                    return false;
                }
            }

            if (split.Length < 3) // help, I ddont know math
                return false;

            try
            {
                string itemType = split[1];
                var protocol = split[0].Substring(0, split[0].IndexOf(':'));
                string itemDLURL = split[0].Substring(protocol.Length + 1, split[0].Length - (protocol.Length + 1));

                if (!int.TryParse(split[2], out int itemID))
                {
                    HedgeApp.CreateOKMessageBox("Error", $"Invalid GameBanana item id {split[2]}").ShowDialog();
                    return false;
                }

                return new GBModWindow(itemType, itemID, itemDLURL, protocol).ShowDialog();
            }
            catch (Exception ex)
            {
                HedgeApp.CreateOKMessageBox("Error", ex.Message).ShowDialog();
                return false;
            }
        }

        public static async Task<string[]> FetchRemoteInstallQueue(string memberID, string secretKey, string appID, CancellationToken c = default)
        {
            string url = $"https://gamebanana.com/apiv11/RemoteInstall/{memberID}/{secretKey}/{appID}";

            var response = HedgeApp.HttpClient.GetAsync(url, c);
            if (!response.Result.IsSuccessStatusCode)
                return ["error"];

            try
            {
                var content = await response.Result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<string[]>(content);
            }
            catch { }
            return [];
        }

        public static async Task RunRemoteInstallServer(CancellationToken c = default)
        {
            if (string.IsNullOrEmpty(RegistryConfig.GameBananaRemoteInstallID) || string.IsNullOrEmpty(RegistryConfig.GameBananaRemoteInstallKey))
                return;

            if (_serverRunning)
                return;
            _serverRunning = true;
            try
            {
                var lastPoll = DateTime.MinValue;
                var refreshTime = TimeSpan.FromSeconds(30);
                while (!c.IsCancellationRequested)
                {
                    if (DateTime.Now - lastPoll < refreshTime)
                    {
                        await Task.Delay(250, c);
                        continue;
                    }

                    string memberID = RegistryConfig.GameBananaRemoteInstallID;
                    string secretKey = RegistryConfig.GameBananaRemoteInstallKey;
                    var uris = await FetchRemoteInstallQueue(memberID, secretKey, "HedgeModManager", c);
                    lastPoll = DateTime.Now;
                    if (uris == null)
                        continue;
                    
                    // TODO: Implement feedback
                    if (uris.Length == 1 && uris[0] == "error")
                        break;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (string uri in uris)
                        {
                            try
                            {
                                ParseCommandLine(uri);
                            }
                            catch
                            {
                            }
                        }
                    });
                }
            }catch { }
            _serverRunning = false;
        }

    }
    public class GBAPIItemData
    {
        public string ItemType { get; set; }
        public int ItemID;

        public GBAPIItemData(string itemType, int itemID)
        {
            ItemType = itemType;
            ItemID = itemID;
        }
    }

    public class GBAPICredit
    {
        public string MemberName { get; set; }
        public string Role { get; set; }
        public int MemberID { get; set; }
    }

    public class GBAPICreditGroups
    {
        [JsonIgnoreAttribute]
        public Dictionary<string, List<GBAPICredit>> Credits = new Dictionary<string, List<GBAPICredit>>();

        [JsonExtensionData]
        private Dictionary<string, JToken> CreditsData { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            foreach (var credit in CreditsData)
            {
                var credits = new List<GBAPICredit>();
                foreach (var cred in credit.Value)
                {
                    credits.Add(new GBAPICredit()
                    {
                        MemberName = cred[0].ToString(),
                        Role = cred[1].ToString(),
                        MemberID = cred[2].ToObject<int>()
                    }); ;
                }
                Credits.Add(credit.Key, credits);
            }
        }
    }

    public class GBAPIFile
    {
        [JsonProperty("_sFile")]
        public string FileName { get; set; }
        [JsonProperty("_nFilesize")]
        public int _nFilesize { get; set; }
        [JsonProperty("_sDownloadUrl")]
        public string DownloadURL { get; set; }
        [JsonProperty("_sDescription")]
        public string Description { get; set; }
        [JsonProperty("_tsDateAdded")]
        public int DateAdded { get; set; }
        [JsonProperty("_nDownloadCount")]
        public string DownloadCount { get; set; }
        public List<string> Files { get; set; }
    }

    public class GBAPIItemDataBasic : GBAPIItemData
    {
        [JsonProperty("name")]
        public string ModName { get; set; }
        [JsonProperty("userid")]
        public int OwnerID { get; set; }
        [JsonProperty("Owner().name")]
        public string OwnerName { get; set; }
        [JsonProperty("screenshots")]
        public string ScreenshotsRaw { get; set; }
        [JsonProperty("text")]
        public string Body { get; set; }
        [JsonProperty("description")]
        public string Subtitle { get; set; }
        [JsonProperty("Credits().aAuthorsAndGroups()")]
        public GBAPICreditGroups Credits { get; set; }
        [JsonProperty("Preview().sPreviewUrl()")]
        public Uri SoundURL { get; set; }
        [JsonProperty("Files().aFiles()")]
        public Dictionary<string, GBAPIFile> Files { get; set; }

        public List<GBAPIScreenshotData> Screenshots
        {
            get
            {
                if (ScreenshotsRaw != null)
                    return JsonConvert.DeserializeObject<List<GBAPIScreenshotData>>(ScreenshotsRaw);
                return new List<GBAPIScreenshotData>();
            }
        }

        public GBAPIItemDataBasic(string itemType, int itemID) : base(itemType, itemID)
        {
        }
    }

    public class GBAPIScreenshotData
    {
        [JsonProperty("_sCaption")]
        public string Caption { get; set; }

        [JsonProperty("_sFile")]
        public string FileName { get; set; }

        [JsonProperty("_nFilesize")]
        public int FileSize { get; set; }

        [JsonProperty("_sFile100")]
        public string FileSmall { get; set; }

        public string URL
        {
            get
            {
                return $"https://images.gamebanana.com/img/ss/mods/{FileName}";
            }
        }

        public string URLSmall
        {
            get
            {
                return $"https://images.gamebanana.com/img/ss/mods/{FileSmall}";
            }
        }
    }
}