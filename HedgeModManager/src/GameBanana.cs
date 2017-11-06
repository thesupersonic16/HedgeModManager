using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HedgeModManager
{
    public static class GameBanana
    {

        public class GameBananaItemSubmittion
        {
            public string Name, Description, UserId, Credits;

            public static string GetResponseFromGameBanana(string itemType, string ItemID)
            {
                return new WebClient().DownloadString($"https://api.gamebanana.com/Core/Item/Data?itemtype={itemType}&itemid={ItemID}&fields=name,description,text,userid,Credits().aAuthors()&format=xml");
            }

            public static GameBananaItemSubmittion ReadResponse(string s)
            {
                var item = new GameBananaItemSubmittion();
                var xml = XDocument.Parse(s);
                var values = xml.Root.Elements("value").ToList();
                item.Name = values[0].Value;
                item.Description = values[1].Value;
                item.Description += values[2].Value;
                item.UserId = values[3].Value;
                var credits = xml.Root.Elements("valueset").ToList()[0];
                /*bool indent = false;
                foreach (var values2 in credits.Elements("value"))
                {
                    item.Credits += values2.Value + "\n";
                    if (indent = !indent)
                        item.Credits += "    ";
                }*/
                return item;
            }
        }

        public class GameBananaItemMember
        {
            public string Name, AvatarURL;

            public static string GetResponseFromGameBanana(string UserID)
            {
                return new WebClient().DownloadString($"https://api.gamebanana.com/Core/Item/Data?itemtype=Member&itemid={UserID}&fields=name,Url().sGetAvatarUrl()&format=xml");
            }

            public static GameBananaItemMember ReadResponse(string s)
            {
                var item = new GameBananaItemMember();
                var xml = XDocument.Parse(s);
                var values = xml.Root.Elements("value").ToList();
                item.Name = values[0].Value;
                item.AvatarURL = values[1].Value;
                return item;
            }
        }
    }
}
