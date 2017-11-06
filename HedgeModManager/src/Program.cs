using System;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HedgeModManager
{
    internal static class Program
    {
        //Variables/Constants
        public static string StartDirectory = Application.StartupPath;
        public static string ExecutableName = Path.GetFileName(Application.ExecutablePath);
        public static string HedgeModManagerPath = Application.ExecutablePath;
        //public static NamedPipeServerStream Server = new NamedPipeServerStream("hedgemodmanager", PipeDirection.InOut);
        public const string ProgramName = "Hedge Mod Manager";
        public const string ProgramNameShort = "HedgeModManager";
        public const string VersionString = "6.1-003";
        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
        public static bool Restart = false;

        //Methods
        [STAThread]
        private static void Main(string[] args)
        {
            /*string url_ = "https://drive.google.com/drive/folders/0B00I5RyzGDEGUWNsUWROZVg0VTQ";
            string data = new WebClient().DownloadString(url_);
            data = data.Substring(data.IndexOf("window['_DRIVE_ivd'] = '") + 24);
            data = data.Substring(0, data.IndexOf("'") + 1);
            data = data.Replace("\\n", "\n");
            data = data.Replace("\\x22", "\"");
            data = data.Replace("\\x5b", "[");
            data = data.Replace("\\x5d", "]");
            data = data.Replace("\\/", "/");
            File.WriteAllText("data.json", data);

            return;*/
            if (args.Length > 0)
            {
                // Tested with hedgemmgens://installmod/https://drive.google.com/uc?export=download&confirm=no_antivirus&id=0BzGMWzGVT2c7NFFmbnhRYnFMbE0
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
                        LogFile.Close();
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
                    WebClient client = new WebClient();
                    var stream = client.OpenRead(thumb);
                    var bitmap = new Bitmap(stream);
                    stream.Close();
                    client.Dispose();

                    var download = new DownloadModForm(submittion.Name, user.Name, submittion.Description, url, submittion.Credits, bitmap);
                    download.ShowDialog();
                    return;
                }
            }
            LogFile.Initialize();
            LogFile.AddMessage($"Starting {ProgramName} (v{VersionString})...");

            #if DEBUG
            if (!(File.Exists(Path.Combine(StartDirectory, "slw.exe")) ||
                File.Exists(Path.Combine(StartDirectory, "SonicGenerations.exe"))))
            {
                // NOTE: The ModLoader Updating (UpdateForm.cs) doesn't use "StartDirectory"
                StartDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Sonic Lost World";
                if (!File.Exists(StartDirectory))
                    StartDirectory = "D:\\SteamLibrary\\steamapps\\common\\Sonic Generations";
            }
#endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            while (Restart)
            {
                Restart = false;
                LogFile.Initialize();
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

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeConsole();

    }
}