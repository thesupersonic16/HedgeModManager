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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace GameBananaAPI
{
    public class GBAPI
    {

        // TODO: Add Core/List support
        public enum GBAPIRequestType
        {
            COREITEMDATA
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
            public string Build(GBAPIItemData item)
            {
                if (APIType == GBAPIRequestType.COREITEMDATA)
                {
                    var supportedFields = GetSupportedFields(item.ItemType);
                    string URL = $"https://api.gamebanana.com/Core/Item/Data?itemtype={item.ItemType}&itemid={item.ItemID}&fields=";
                    foreach(var property in item.GetType().GetProperties())
                    {
                        var prop = (JsonPropertyAttribute)property.GetCustomAttribute(typeof(JsonPropertyAttribute));
                        if(prop != null && supportedFields.Contains(prop.PropertyName))
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
            public bool ParseResponse(string response,ref GBAPIItemDataBasic item)
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

        public static List<string> GetSupportedFields(string itemType)
        {
            var url = $"https://api.gamebanana.com/Core/Item/Data/AllowedFields?itemtype={itemType}";
            using(var client = new WebClient())
            {
                try
                {
                    return JsonConvert.DeserializeObject<List<string>>(client.DownloadString(url));
                }
                catch
                {
                    return new List<string>();
                }
            }
        }

        public static bool RequestItemData(ref GBAPIItemDataBasic item)
        {
            var type = item.ItemType;
            var id = item.ItemID;
            var handler = new GBAPIRequestHandler();
            string request = handler.Build(item);
            string response = new WebClient() { Encoding = Encoding.ASCII }.DownloadString(request);
            response = Uri.UnescapeDataString(response);
            var result = handler.ParseResponse(response, ref item);
            item.ItemType = type;
            item.ItemID = id;
            return result;
        }

        /// <summary>
        /// Installs the GameBanana one-click install handler
        /// </summary>
        /// <returns></returns>
        public static bool InstallGBHandler(Game game)
        {
            string protocolName = $"HedgeModManager for {game.GameName}";
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

        public static void ParseCommandLine(string line)
        {
            string[] split = line.Split(',');
            if (split.Length < 3) // help, I ddont know math
                return;

            string itemType = split[1];
            var protocal = split[0].Substring(0, split[0].IndexOf(':'));
            string itemDLURL = split[0].Substring(protocal.Length + 1, split[0].Length - (protocal.Length + 1));

            if (!int.TryParse(split[2], out int itemID))
            {
                HedgeApp.CreateOKMessageBox("Error", $"Invalid GameBanana item id {itemID}").ShowDialog();
                return;
            }

            var item = new GBAPIItemDataBasic(itemType, itemID);
            if (!RequestItemData(ref item))
            {
                HedgeApp.CreateOKMessageBox("Error", "Invalid GameBanana item").ShowDialog();
                return;
            }
            var game = Games.Unknown;
            foreach(var gam in Games.GetSupportedGames())
            {
                if(gam.GBProtocol == protocal)
                {
                    game = gam;
                    break;
                }
            }
            if (game == Games.Unknown)
                return;

            new GBModWindow(item,itemDLURL, game).ShowDialog();
            return;
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
            foreach(var credit in CreditsData)
            {
                var credits = new List<GBAPICredit>();
                foreach(var cred in credit.Value)
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
        [JsonProperty("_aMetadata")]
        public GBAPIFileMetadata FileMetadata { get; set; }
    }

    public class GBAPIFileMetadata
    {
        [JsonIgnoreAttribute]
        public List<string> Files = new List<string>();
        [JsonIgnoreAttribute]
        public string MimeType { get; set; }

        [JsonExtensionData]
        private Dictionary<string, JToken> FileMetadata { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            try
            {
                MimeType = FileMetadata["_sMimeType"].ToString();
                if (FileMetadata.ContainsKey("_aArchiveFileTree"))
                    Files.AddRange(LoopDirectory("", FileMetadata["_aArchiveFileTree"].ToObject<JObject>()));
            }
            catch
            {
                // Failed
            }
        }

        private List<string> LoopDirectory(string dir, JObject jObject)
        {
            var list = new List<string>();
            foreach (var data in jObject)
            {
                switch (data.Value.Type)
                {
                    case JTokenType.String:
                        list.Add(Path.Combine(dir, data.Value.ToString()));
                        break;
                    case JTokenType.Object:
                        var obj = data.Value.ToObject<JObject>();
                        list.AddRange(LoopDirectory(Path.Combine(dir, data.Key), obj));
                        break;
                    case JTokenType.Array:
                        var array = data.Value.ToObject<JArray>();
                        foreach (var file in array)
                            list.Add(Path.Combine(dir, data.Key, file.ToString()));
                        break;
                    default:
                        break;
                }
            }
            return list;
        }
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
        public Dictionary<string, GBAPIFile> Files { get; set; } = new Dictionary<string, GBAPIFile>();

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

        [JsonProperty("_sRelativeImageDir")]
        public string ImageDirectory { get; set; }

        [JsonProperty("_sFile100")]
        public string FileSmall { get; set; }

        public string URL
        {
            get
            {
                return $"http://files.gamebanana.com/{ImageDirectory}/{FileName}";
            }
        }

        public string URLSmall
        {
            get
            {
                return $"http://files.gamebanana.com/{ImageDirectory}/{FileSmall}";
            }
        }
    }
}