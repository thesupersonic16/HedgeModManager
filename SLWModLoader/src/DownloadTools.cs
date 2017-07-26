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
            result = GetDownloadableGoogleDriveURL(url);
            if (result != null) return result;
            result = GetDownloadableDropBoxURL(url);
            if (result != null) return result;
            result = GetDownloadableOneDriveZipURL(url);
            if (result != null) return result;
            result = GetDownloadableMediaFireURL(url);
            if (result != null) return result;
            return url;
        }

        public static string GetDownloadableGoogleDriveURL(string url)
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
        public static string GetDownloadableMediaFireURL(string url)
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

        // Hasn't been tested
        public static string GetDownloadableDropBoxURL(string url)
        {
            string finalURL = url;
            if (!url.Contains("dropbox"))
                return null;
            if (url.Contains("dl=0"))
                finalURL.Replace("dl=0", "dl=1");
            else if(!url.Contains("dl="))
                if (url.Contains("?"))
                    url += "&dl=1";
                else
                    url += "?dl=1";
            if (finalURL.Equals(url))
                return null;
            return finalURL;
        }

        // Hasn't been tested
        public static string GetDownloadableOneDriveZipURL(string url)
        {
            if (!url.Contains("/Zip?") || !url.Contains("cid=") || !url.Contains("authkey="))
                return null;

            string escapeURL = Uri.EscapeUriString(url);
            string cid = escapeURL.Substring(escapeURL.IndexOf("cid=") + 4, 16);
            string key = escapeURL.Substring(escapeURL.IndexOf("authkey=") + 8, 16);

            cid = Uri.UnescapeDataString(cid.ToLower());
            key = Uri.UnescapeDataString(cid);

            return string.Format(
                "https://cid-{0}.users.storage.live.com/downloadfiles/V1/Zip?authKey={1}",
                 cid, key); // TODO: Ensure this is correct
        }

    }
}
