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
            public List<KeyValuePair<string, PropertyInfo>> Fields = new List<KeyValuePair<string, PropertyInfo>>();
            public string ItemType = "";
            public int ItemID = 0;
            public string Suffix = "&format=xml";
            public GBAPIRequestType APIType = GBAPIRequestType.COREITEMDATA;

            public void ProcessItemData(GBAPIItemData item)
            {
                ItemType = item.ItemType;
                ItemID = item.ItemID;
                APIType = GBAPIRequestType.COREITEMDATA;

                var type = item.GetType();
                foreach (var field in type.GetProperties())
                {
                    var GBField = (GBAPIField)field.GetCustomAttributes(typeof(GBAPIField), true).FirstOrDefault();
                    if (GBField != null)
                    {
                        Fields.Add(new KeyValuePair<string, PropertyInfo>(GBField.FieldName, field));
                    }
                }
            }

            /// <summary>
            /// Creates Request URL to GameBanana's API.
            ///  - Core/Item/Data
            ///      Calls Core/Item/Data with the specified item type, id and fields
            /// </summary>
            /// <returns>Request URL</returns>
            public string Build()
            {
                if (APIType == GBAPIRequestType.COREITEMDATA)
                {
                    string URL = $"https://api.gamebanana.com/Core/Item/Data?itemtype={ItemType}&itemid={ItemID}&fields=";
                    foreach (var field in Fields)
                    {
                        if (URL.Last() != '=')
                            URL += ',';
                        URL += field.Key;
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
            public bool ParseResponse(string response, GBAPIItemData item)
            {
                var responseXML = XDocument.Parse(response).Root;
                var elements = responseXML.Elements().ToList();
                if (elements.Count() != Fields.Count)
                {
                    new ExceptionWindow(new Exception("GameBanana Returned less values than requested! Parsing must abort!"), response).Show();
                    return false;
                }

                foreach (var field in Fields)
                {
                    var element = elements.First();
                    var GBFieldKey = (GBAPIFieldKeyArray)field.Value.GetCustomAttributes(typeof(GBAPIFieldKeyArray), true).FirstOrDefault();
                    if (GBFieldKey != null)
                    {
                        var arrayElements = element.Elements().ToArray();
                        var array = Array.CreateInstance(field.Value.PropertyType.GetElementType(), arrayElements.Length);
                        var keyInfo = field.Value.PropertyType.GetElementType().GetField(GBFieldKey.KeyName);
                        var arrayInfo = field.Value.PropertyType.GetElementType().GetField(GBFieldKey.ArrayName);
                        for (int i = 0; i < array.Length; ++i)
                        {
                            object obj = Activator.CreateInstance(field.Value.PropertyType.GetElementType());
                            // Key
                            object value = TryConvert(arrayElements[i].Attribute("key").Value, keyInfo.FieldType);
                            if (value != null)
                                keyInfo.SetValue(obj, value);
                            // Array
                            var subElements = arrayElements[i].Elements().ToArray();
                            var array2 = Array.CreateInstance(arrayInfo.FieldType.GetElementType(), subElements.Length);
                            for (int ii = 0; ii < array2.Length; ++ii)
                                array2.SetValue(XMLtoObject(arrayInfo.FieldType.GetElementType(), subElements[ii]), ii);
                            arrayInfo.SetValue(obj, array2);


                            array.SetValue(obj, i);
                        }
                        field.Value.SetValue(item, array);
                    }
                    else if (element.Name == "value")
                    {
                        field.Value.SetValue(item, XMLtoObject(field.Value.PropertyType, elements.First()));
                    }
                    else
                    {
                        var arrayElements = element.Elements().ToArray();
                        var array = Array.CreateInstance(field.Value.PropertyType.GetElementType(), arrayElements.Length);
                        for (int i = 0; i < array.Length; ++i)
                            array.SetValue(XMLtoObject(field.Value.PropertyType.GetElementType(), arrayElements[i]), i);
                        field.Value.SetValue(item, array);
                    }
                    elements.RemoveAt(0);
                }
                return true;
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

        public static object XMLtoObject(Type type, XElement element)
        {
            object obj = TryConvert(element.Value, type);
            if (obj != null)
                return obj;
            obj = Activator.CreateInstance(type);
            var fields = type.GetFields();
            var elements = new List<XElement>(element.Elements());
            if (element.Name == "value")
            {
                return XMLtoObject(type, elements.First());
            }
            foreach (var field in fields)
            {
                var curElement = elements.First();
                if (field.IsLiteral)
                {
                    return obj;
                }
                else if (field.FieldType.IsArray)
                {
                    var arrayElements = curElement.Elements().ToArray();
                    var array = Array.CreateInstance(field.FieldType.GetElementType(), arrayElements.Length);
                    for (int i = 0; i < array.Length; ++i)
                        array.SetValue(XMLtoObject(field.FieldType.GetElementType(), arrayElements[i]), i);
                    field.SetValue(obj, array);
                }
                else
                {
                    object value = TryConvert(curElement.Value, field.FieldType);

                    if (value != null)
                        field.SetValue(obj, value);
                    else
                        field.SetValue(obj, XMLtoObject(field.FieldType, curElement));
                }
                elements.RemoveAt(0);
            }
            return obj;
        }

        public static object JSONToObject(Type type, string data)
        {
            //TODO
            var obj = Activator.CreateInstance(type);
            foreach(var prop in type.GetProperties())
            {
                var attribute = prop.GetCustomAttribute(typeof(GBAPIField));
            }
            return obj;
        }

        public static bool RequestItemData(GBAPIItemData item)
        {
            var handler = new GBAPIRequestHandler();
            handler.ProcessItemData(item);
            string request = handler.Build();
            string response = new WebClient().DownloadString(request);
            return handler.ParseResponse(response, item);
        }

        /// <summary>
        /// Installs the GameBanana one-click install handler
        /// </summary>
        /// <returns></returns>
        public static bool InstallGBHandler()
        {
            // TODO
            string protocol = "hedgemmforces";
            string protocolName = "HedgeModManager for Sonic Forces";
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
                var reg = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\{protocol}");
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
            string itemDLURL = split[0];
            int itemID = 0;
            if (!int.TryParse(split[2], out itemID))
            {
                // TODO: Show Error message here
                return;
            }
            var item = new GBAPIItemDataBasic(itemType, itemID);
            if (!GBAPI.RequestItemData(item))
            {
                // TODO: Show Error message here
                return;
            }
            var screenshots = JsonConvert.DeserializeObject<List<GBAPIScreenshotData>>(item.Screenshots);
            new GBModWindow(item, screenshots).ShowDialog();
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


    public class GBAPIField : Attribute
    {
        public string FieldName;

        public GBAPIField(string fieldName)
        {
            FieldName = fieldName;
        }
    }

    public class GBAPIFieldKeyArray : Attribute
    {
        public string KeyName;
        public string ArrayName;

        public GBAPIFieldKeyArray(string keyName, string arrayName)
        {
            KeyName = keyName;
            ArrayName = arrayName;
        }
    }


    public class GBAPICredit
    {
        public string MemberName;
        public string Role;
        public int MemberID;
    }

    public class GBAPICreditGroup
    {
        public string GroupName;
        public GBAPICredit[] Credits;
    }


    public class GBAPIItemDataBasic : GBAPIItemData
    {
        [GBAPIField("name")]
        public string ModName { get; set; }
        [GBAPIField("userid")]
        public int OwnerID { get; set; }
        [GBAPIField("Owner().name")]
        public string OwnerName { get; set; }
        [GBAPIField("screenshots")]
        public string Screenshots { get; set; }
        [GBAPIField("text")]
        public string Body { get; set; }
        [GBAPIField("description")]
        public string Subtitle { get; set; }
        [GBAPIField("Credits().aAuthorsAndGroups()")]
        [GBAPIFieldKeyArray("GroupName", "Credits")]
        public GBAPICreditGroup[] Credits { get; set; }

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