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
using System.Security.Principal;
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
        public const string VersionString = "6.1-018";
        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
        public static bool Restart = false;
        public static bool UseDarkTheme = true;

        //Methods
        [STAThread]
        private static void Main(string[] args)
        {

            LogFile.Initialize(false);
            LogFile.AddMessage($"Starting {ProgramName} (v{VersionString})...");

#if DEBUG
            Steam.Init();
            var games = InstallForm.FindGames();
            if (games.Any(t => t.GameName == Games.SonicForces.GameName))
                StartDirectory = Path.GetDirectoryName(games.Find(t => t.GameName == Games.SonicForces.GameName).Path);
#endif

            LogFile.AddMessage($"Running {ProgramName} in {StartDirectory}");

            // Writes "cpkredir.ini" if it doesn't exists as HedgeModManager uses it to store its config
            if (!File.Exists(Path.Combine(StartDirectory, "cpkredir.ini")))
            {
                LogFile.AddMessage("Writing cpkredir.ini");
                File.WriteAllText(Path.Combine(StartDirectory, "cpkredir.ini"), Resources.cpkredirINI);
            }


            if (File.Exists(Path.Combine(StartDirectory, "cpkredir.ini")))
                MainForm.CPKREDIRIni = new IniFile(Path.Combine(StartDirectory, "cpkredir.ini"));

            if (args.Length > 0)
            {
                if (args[0] == "-dev")
                {
                    LogFile.AddMessage("Starting Dev Tools");
                    DevTools.Init();
                    return;
                }

                if (args[0] == "-gb")
                {
                    LogFile.AddMessage("Running GB Installer");
                    DownloadGameBananaItem(args[1]);
                    LogFile.Close();
                    return;
                }
            }


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

        public static void DownloadGameBananaItem(string arg)
        {
            try
            {

                // GameBanana Download Protocol
                if (arg.ToLower().StartsWith(@"hedgemmgens:")
                    || arg.ToLower().StartsWith(@"hedgemmlw:")
                    || arg.ToLower().StartsWith(@"hedgemmforces:"))
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    string url = arg;
                    if (url.ToLower().StartsWith(@"hedgemmgens:"))
                        url = url.Substring(12);
                    if (url.ToLower().StartsWith(@"hedgemmlw:"))
                        url = url.Substring(10);
                    if (url.ToLower().StartsWith(@"hedgemmforces:"))
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

                    var download = new DownloadModForm(submittion.Name, user.Name, submittion.Description, url,
                        submittion.Credits, submittion.ThumbURL);
                    download.ShowDialog();
                }
            }catch (Exception ex)
            {
                LogFile.Initialize(false);
                MainForm.AddMessage("Exception thrown on GameBanana Item Installation", ex, arg);
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

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
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

        // https://stackoverflow.com/questions/11660184/c-sharp-check-if-run-as-administrator
        public static bool RunningAsAdmin()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

    }
}