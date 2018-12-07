using HedgeModManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            public List<KeyValuePair<string, FieldInfo>> Fields = new List<KeyValuePair<string, FieldInfo>>();
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
                foreach (var field in type.GetFields())
                {
                    var GBField = (GBAPIField)field.GetCustomAttributes(typeof(GBAPIField), true).FirstOrDefault();
                    if (GBField != null)
                    {
                        Fields.Add(new KeyValuePair<string, FieldInfo>(GBField.FieldName, field));
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
                    var list = Fields.Select(t => t.Key).ToList();
                    list.Insert(0, response);
                    MainForm.AddMessage("Exception thrown while Parsing a GBAPI Response!",
                        new Exception("GameBanana Returned less values than requested! Parsing must abort!"), list.ToArray());
                    return false;
                }

                foreach (var field in Fields)
                {
                    var element = elements.First();
                    var GBFieldKey = (GBAPIFieldKeyArray)field.Value.GetCustomAttributes(typeof(GBAPIFieldKeyArray), true).FirstOrDefault();
                    if (GBFieldKey != null)
                    {
                        var arrayElements = element.Elements().ToArray();
                        var array = Array.CreateInstance(field.Value.FieldType.GetElementType(), arrayElements.Length);
                        var keyInfo = field.Value.FieldType.GetElementType().GetField(GBFieldKey.KeyName);
                        var arrayInfo = field.Value.FieldType.GetElementType().GetField(GBFieldKey.ArrayName);
                        for (int i = 0; i < array.Length; ++i)
                        {
                            object obj = Activator.CreateInstance(field.Value.FieldType.GetElementType());
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
                        field.Value.SetValue(item, XMLtoObject(field.Value.FieldType, elements.First()));
                    }
                    else
                    {
                        var arrayElements = element.Elements().ToArray();
                        var array = Array.CreateInstance(field.Value.FieldType.GetElementType(), arrayElements.Length);
                        for (int i = 0; i < array.Length; ++i)
                            array.SetValue(XMLtoObject(field.Value.FieldType.GetElementType(), arrayElements[i]), i);
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

        public static bool RequestItemData(GBAPIItemData item)
        {
            var handler = new GBAPIRequestHandler();
            handler.ProcessItemData(item);
            string request = handler.Build();
            string response = new WebClient().DownloadString(request);
            return handler.ParseResponse(response, item);
        }

    }
    public class GBAPIItemData
    {
        public string ItemType;
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
        public string ModName;
        [GBAPIField("userid")]
        public int    OwnerID;
        [GBAPIField("Owner().name")]
        public string OwnerName;
        [GBAPIField("Preview().sStructuredDataFullsizeUrl()")]
        public string ScreenshotURL;
        [GBAPIField("text")]
        public string Body;
        [GBAPIField("description")]
        public string Subtitle;
        [GBAPIField("Credits().aAuthorsAndGroups()")]
        [GBAPIFieldKeyArray("GroupName", "Credits")]
        public GBAPICreditGroup[] Credits;

        public GBAPIItemDataBasic(string itemType, int itemID) : base(itemType, itemID)
        {
        }

    }
}
