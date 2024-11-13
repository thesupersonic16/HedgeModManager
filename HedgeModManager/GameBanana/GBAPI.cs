using HedgeModManager;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HedgeModManager.UI;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using System.Windows;

namespace GameBananaAPI
{
    public class GBAPI
    {

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
                request = $"https://api.gamebanana.com/Core/Item/Data?itemtype=File&itemid={file.Key}&fields=Metadata().aArchiveFilesList()&return_keys=1";
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
            string[] split = line.Split(',');
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