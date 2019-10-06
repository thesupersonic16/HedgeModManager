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
                    string URL = $"https://api.gamebanana.com/Core/Item/Data?itemtype={item.ItemType}&itemid={item.ItemID}&fields=";
                    foreach(var property in item.GetType().GetProperties())
                    {
                        var prop = (JsonPropertyAttribute)property.GetCustomAttribute(typeof(JsonPropertyAttribute));
                        if(prop != null)
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
            /// Parses the response data(XML) and writes all data into parameter "item".
            /// </summary>
            /// <param name="response">The Response data from GameBanana API in XML string format</param>
            /// <param name="item">Reference to a GBAPIItemData to write the data to</param>
            /// <returns>If Parse completed with no errors</returns>
            public bool ParseResponse(string response,ref GBAPIItemDataBasic item)
            {
                try
                {
                    item = JsonConvert.DeserializeObject<GBAPIItemDataBasic>(response);
                    return true;
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
            // Can we use LOCAL_USER instead of CLASSES_ROOT?
            // Pretty sure you can, You can try it, Also I cant see errors
            // but i can
            // Any errors?
            // yes
            // Where?
            // oh
            // How do you get the path to HMM? Need it for the bottom SetValue
            // There should be a static string in App
            // I see StartDirectory, Thats about it
            // Then lets add one lol
            // But I'm lazy
            try
            {
                var reg = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\{game.GBProtocol}");
                reg.SetValue("", $"URL:{protocolName}");
                reg.SetValue("URL Protocol", "");
                reg = reg.CreateSubKey("shell\\open\\command");
                reg.SetValue("", $"\"{App.AppPath}\" -gb \"%1\"");
                reg.Close();
            }catch
            {
                new ExceptionWindow(new Exception("Error installing Gamebanana handler. Please restart Hedge Mod Manager as admin")).ShowDialog();
            }

            return true;
        }

        public static void ParseCommandLine(string line)
        {
            // soo.....
            // Does HMM only work with one game per install?
            // How would i know
            // Then tahts a yes
            // I mean you can change game name in App.cs
            // Since when does GB do command line stuff
            // Never, We have to do it, Well, Its Fuck cant spell lol
            // pretty much Chrome will call a command
            // what if
            // i dont use chrome
            // Pretty sure Firefox does the same
            // but what if i use internet explorer 6.0
            // GB doesnt even support IE 6.0
            // thats a damn shame
            // Its a shame that you are using IE 6.0 in the first place
            // Its a legendary browser ok
            // Yeah, to download Chrome
            // And the surf the web in amazing speed
            // Dial-up speeds?
            // Fiber optics
            // :( I dont have fibre
            // But i do at amazing 10Mbps
            // Noice
            string[] split = line.Split(',');
            if (split.Length < 3) // help, I ddont know math
                return;
            string itemType = split[1];
            var protocal = split[0].Substring(0, split[0].IndexOf(':'));
            string itemDLURL = split[0].Substring(protocal.Length + 1, split[0].Length - (protocal.Length + 1));
            int itemID = 0;
            if (!int.TryParse(split[2], out itemID))
            {
                App.CreateOKMessageBox("Error", $"Invalid Gamebanana item id {itemID}").ShowDialog();
                return;
            }
            var item = new GBAPIItemDataBasic(itemType, itemID);
            if (!RequestItemData(ref item))
            {
                App.CreateOKMessageBox("Error", "Invalid Gamebanana item").ShowDialog();
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
            // TODO: Show Info Window (ofc it will need a Download button)
            // I FORGOT ABOUT THE DOWNLOAD BUTTON
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

        public List<GBAPIScreenshotData> Screenshots
        {
            get
            {
                return JsonConvert.DeserializeObject<List<GBAPIScreenshotData>>(ScreenshotsRaw);
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