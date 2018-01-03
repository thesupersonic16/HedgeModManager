using HedgeModManager.Properties;
using SS16;
using System;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Windows.Input;

namespace HedgeModManager
{
    internal static class Program
    {
        //Variables/Constants
        public static string StartDirectory = Application.StartupPath;
        public static string ExecutableName = Path.GetFileName(Application.ExecutablePath);
        public static string HedgeModManagerPath = Application.ExecutablePath;
        public static Game CurrentGame = Games.Unknown;
        public const string ProgramName = "Hedge Mod Manager";
        public const string ProgramNameShort = "HedgeModManager";
        public const string VersionString = "6.1-016";
        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
        public static bool Restart = false;

        //Methods
        [STAThread]
        private static void Main(string[] args)
        {

            if (args.Length > 0)
            {
                // GameBanana Download Protocol
                if (args[0].ToLower().StartsWith(@"hedgemmgens:")
                    || args[0].ToLower().StartsWith(@"hedgemmlw:")
                    || args[0].ToLower().StartsWith(@"hedgemmforces:"))
                {
                    string url = args[0];
                    if (args[0].ToLower().StartsWith(@"hedgemmgens:"))
                        url = url.Substring(12);
                    if (args[0].ToLower().StartsWith(@"hedgemmlw:"))
                        url = url.Substring(10);
                    if (args[0].ToLower().StartsWith(@"hedgemmforces:"))
                        url = url.Substring(14);

                    // TODO:
                    string itemType = url.Split(',')[1];
                    string itemID = url.Split(',')[2];
                    url = url.Substring(0, url.IndexOf(","));
                    
                    if (!IsURL(url))
                    {
                        MessageBox.Show("Link Given is not a URL!");
                        return;
                    }
                    
                    var submittion = GameBanana.GameBananaItemSubmittion.ReadResponse(
                        GameBanana.GameBananaItemSubmittion.GetResponseFromGameBanana(itemType, itemID));
                    var user = GameBanana.GameBananaItemMember.ReadResponse(
                        GameBanana.GameBananaItemMember.GetResponseFromGameBanana(submittion.UserId));

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    
                    //TODO
                    string thumb = $"https://gamebanana.com/skins/embeddables/{itemID}?type=large_minimal_square";

                    Bitmap bitmap = null;
                    try
                    {
                        var client = new WebClient();
                        var stream = client.OpenRead(thumb);
                        bitmap = new Bitmap(stream);
                        stream.Close();
                        client.Dispose();
                    }catch
                    {
                        bitmap = null;
                    }

                    var download = new DownloadModForm(submittion.Name, user.Name, submittion.Description, url,
                        submittion.Credits, bitmap);
                    download.ShowDialog();
                    return;
                }
            }
            LogFile.Initialize();
            LogFile.AddMessage($"Starting {ProgramName} (v{VersionString})...");

#if DEBUG
            //StartDirectory = @"E:\SteamLibrary\steamapps\common\SonicForces\build\main\projects\exec";
#endif

            LogFile.AddMessage($"Running {ProgramName} in {StartDirectory}");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            while (Restart)
            {
                LogFile.Initialize(!Restart);
                Restart = false;
                LogFile.AddMessage($"Starting {ProgramName} (v{VersionString})...");
                Application.Run(new MainForm());
            }
        }

        public static bool IsURL(string URL)
        {
            return Uri.TryCreate(URL, UriKind.Absolute, out Uri uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        public static string GetString(int location, string mainString)
        {
            string substr = mainString.Substring(location).Replace("\\\"", "%22");
            if (!substr.Contains("\""))
                return "";
            else if (substr[0] == '\"')
                return substr.Substring(1, substr.IndexOf("\"", 2) - 1).Replace("%22", "\\\"");
            else
                return GetString(substr.IndexOf('\"'), substr).Replace("%22", "\\\"");
        }

        public static string GetStringAfter(string value, string mainString)
        {
            return GetString(mainString.IndexOf(value) + value.Length + 1, mainString);
        }

        public static string EscapeString(string value)
        {
            return value.Replace("\\\"", "\"").Replace("\\n", "\n").Replace("\\r", "\r");
        }

        private static string GetString(Stream stream)
        {
            stream.ReadByte();
            string buffer = "" + stream.ReadByte();
            char charBuffer = ';';
            while ((charBuffer = (char)stream.ReadByte()) != '\"')
                buffer += charBuffer;
            return buffer;
        }

        public static string SplitAfter(string s, string s2)
        {
            return s.Substring(s.IndexOf(s2) + s2.Length);
        }

        /// <summary>
        /// Computes a SHA256 Hash of bytes
        /// </summary>
        /// <param name="bytes">data to use to create the hash</param>
        /// <returns>A SHA256 Hash</returns>
        public static byte[] ComputeSHA256Hash(byte[] bytes)
        {
            return new SHA256Managed().ComputeHash(bytes);
        }

    }
}