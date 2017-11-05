using System;
using System.IO;
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
        public const string ProgramName = "Hedge Mod Manager";
        public const string ProgramNameShort = "HedgeModManager";
        public const string VersionString = "6.1-002";
        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
        public static bool Restart = false;

        //Methods
        [STAThread]
        private static void Main(string[] args)
        {
            LogFile.Initialize();
            LogFile.AddMessage($"Starting {ProgramName} (v{VersionString})...");
            

            if (args.Length > 0)
            {
                // Tested with hedgemodmanager://installmod/https://drive.google.com/uc?export=download&confirm=no_antivirus&id=0BzGMWzGVT2c7NFFmbnhRYnFMbE0
                if (args[0].ToLower().StartsWith(@"hedgemodmanager://"))
                {
                    string url = SplitAfter(args[0], @"installmod/");
                    if (!IsURL(url))
                    {
                        MessageBox.Show("Link Given is not a URL!");
                        LogFile.Close();
                        return;
                    }
                    if (MessageBox.Show($"Install mod from:\n \"{url}\" ?", ProgramName,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        LogFile.Close();
                        return;
                    }
                    // Creates Console Window
                    AllocConsole();

                    // Downloads the mod
                    var client = new WebClient();
                    Console.Write("Downloading Mod... ");
                    byte[] bytes = client.DownloadData(url);
                    Console.Write("Done.\n");

                    // Creates the directories and writes the file
                    Console.Write("Preparing Mod for installation... ");
                    string tempFolder = Path.Combine(StartDirectory, "downloads");
                    if (!Directory.Exists(tempFolder))
                        Directory.CreateDirectory(tempFolder);
                    string filePath = Path.Combine(tempFolder, "download.bin");
                    File.WriteAllBytes(filePath, bytes);
                    Console.Write("Done.\n");

                    // Installs the mod
                    Console.Write("Installing Mod... ");
                    if (bytes[0] == 'P')
                        AddModForm.InstallFromZip(filePath); // Install from a zip
                    else
                        AddModForm.InstallFrom7zArchive(filePath); // Use 7Zip if its not a Zip
                    Console.Write("Done.\n");

                    // End
                    FreeConsole();
                    LogFile.Close();
                    return;
                }
            }

            #if DEBUG
            if (!(File.Exists(Path.Combine(StartDirectory, "slw.exe")) ||
                File.Exists(Path.Combine(StartDirectory, "SonicGenerations.exe"))))
            {
                // NOTE: The ModLoader Updating (UpdateForm.cs) doesn't use "StartDirectory"
                //StartDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Sonic Lost World";
                //if (!File.Exists(StartDirectory))
                    //StartDirectory = "D:\\SteamLibrary\\steamapps\\common\\Sonic Generations";
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