using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SLWModLoader
{
    public static class DownloadTools
    {

        public static string GetDirectDownloadURL(string url)
        {
            string result = null;
            result = GetDirectGoogleDriveURL(url);
            if (result != null) return result;
            result = GetDirectMediafireURL(url);
            if (result != null) return result;
            return url;
        }

        public static string GetDirectGoogleDriveURL(string url)
        {
            string id = "";
            if (!url.Contains("drive.google"))
                return null;
            if (url.Contains("id="))
            {
                int idPos = url.IndexOf("id=") + 3;
                if (url.Substring(idPos).Contains('&'))
                    id = url.Substring(idPos, url.Substring(idPos).IndexOf("&") - idPos);
                else
                    id = url.Substring(idPos);
            }
            if (id.Length == 0)
                return null;
            return $"https://drive.google.com/uc?export=download&confirm=no_antivirus&id={id}";
        }

        // Hasn't been tested
        public static string GetDirectMediafireURL(string url)
        {
            string dlURL = "";
            if (!url.Contains("mediafire"))
                return null;
            if (url.Contains("/file/") || url.Contains("/download/"))
            {
                string page = new WebClient().DownloadString(url);
                dlURL = Program.GetStringAfter("kNO = ", page);
            }
            if (dlURL.Length == 0)
                return null;
            return dlURL;
        }

    }
}
