using HedgeModManager.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public class DevTools
    {

        public static bool Running = false;
        public static Menu CurrentMenu = Menu.MainMenu;
        public static int MenuIndex = 0;
        public static List<Tuple<string, Menu>> MenuItems = new List<Tuple<string, Menu>>();

        public static void Init()
        {
            Running = true;
            AllocConsole();

            MenuItems.Add(new Tuple<string, Menu>("Calculate Hashes", Menu.CalculateHash));
            MenuItems.Add(new Tuple<string, Menu>("Reset Config", Menu.ResetConfig));
            MenuItems.Add(new Tuple<string, Menu>("Exit", Menu.Exit));

            bool clear = true;
            while (Running)
            {
                clear = Loop(clear);
            }
        }

        public static bool Loop(bool clear)
        {
            Running = true;
            AllocConsole();
            if (clear)
                Console.Clear();
            Console.CursorTop = 0;
            Console.WriteLine();
            WriteCentredText($"=============== HedgeModManager DevTools ===============");
            Console.WriteLine();
            switch (CurrentMenu)
            {
                case Menu.MainMenu:
                {
                    return MainMenu();
                }
                case Menu.Exit:
                {
                    Running = false;
                    return false;
                }
                case Menu.CalculateHash:
                {
                    return CalculateHashes();
                }
                case Menu.ResetConfig:
                {
                    return ResetConfig();
                }
                default:
                {
                    CurrentMenu = Menu.MainMenu;
                    return true;
                }
            }
        }

        public static bool MainMenu()
        {
            Console.CursorTop = 3;
            for (int i = 0; i < MenuItems.Count; ++i)
            {
                string menuText = (
                    (i == MenuIndex) ? ">> " : "   ") + MenuItems[i].Item1 + ((i == MenuIndex) ? " <<" : "   ");
                WriteCentredText(menuText);
            }
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.UpArrow)
            {
                --MenuIndex;
                if (MenuIndex == -1)
                    MenuIndex = MenuItems.Count - 1;
            }
            if (key.Key == ConsoleKey.DownArrow)
            {
                ++MenuIndex;
                if (MenuIndex == MenuItems.Count)
                    MenuIndex = 0;
            }
            if (key.Key == ConsoleKey.Enter)
            {
                CurrentMenu = MenuItems[MenuIndex].Item2;
                return true;
            }
            return false;
        }

        // Params for ResetConfig
        public static int Progress = 0;
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Clear Console?</returns>
        public static bool ResetConfig()
        {
            // Arrays
            var delete = new char[] { 'D', 'E', 'L', 'E', 'T', 'E' };
            var deleteKeys = new ConsoleKey[] { ConsoleKey.D, ConsoleKey.E, ConsoleKey.L, ConsoleKey.E, ConsoleKey.T, ConsoleKey.E };
            WriteCentredText("Are you sure you want to reset your config files? NOTE: This will delete all config files");
            WriteCentredText("Including Codes / Patch.");
            // Spacer
            Console.WriteLine();
            Console.CursorLeft = 10;
            Console.WriteLine("To Continue, Type \"DELETE\" (case-insensitive), To Go Back Press \"ESC\"");
            Console.CursorLeft = 10;
            var oldColour = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            for (int ii = 0; ii < delete.Length; ++ii)
            {
                // Current Character
                if (Progress == ii)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(delete[ii] + " ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    continue;
                }
                Console.Write(delete[ii] + " ");
            }
            Console.ForegroundColor = oldColour;
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Escape)
            {
                CurrentMenu = Menu.MainMenu;
                return true;
            }
            if (key.Key == deleteKeys[Progress])
            {
                ++Progress;
                if (Progress == delete.Length)
                {
                    // Completed, Delete Files
                    DeleteConfig();
                    Console.ReadKey();
                    CurrentMenu = Menu.MainMenu;
                    Progress = 0;
                    return true;
                }
            }
            else
            {
                Progress = 0;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Clear Console?</returns>
        public static bool CalculateHashes()
        {
            // Spacer
            Console.WriteLine();
            var hashes = new Dictionary<string, byte[]>();
            if (File.Exists(Path.Combine(Program.StartDirectory, "d3d11.dll")))
            {
                Console.WriteLine("Calculating d3d11.dll");
                var hash = Program.ComputeSHA256Hash(File.ReadAllBytes(Path.Combine(Program.StartDirectory, "d3d11.dll")));
                hashes.Add("d3d11.dll", hash);
            }
            if (File.Exists(Path.Combine(Program.StartDirectory, "d3d9.dll")))
            {
                Console.WriteLine("Calculating d3d9.dll");
                var hash = Program.ComputeSHA256Hash(File.ReadAllBytes(Path.Combine(Program.StartDirectory, "d3d9.dll")));
                hashes.Add("d3d9.dll", hash);
            }
            if (File.Exists(Path.Combine(Program.StartDirectory, "cpkredir.dll")))
            {
                Console.WriteLine("Calculating cpkredir.dll");
                var hash = Program.ComputeSHA256Hash(File.ReadAllBytes(Path.Combine(Program.StartDirectory, "cpkredir.dll")));
                hashes.Add("cpkredir.dll", hash);
            }
            if (File.Exists(Path.Combine(Program.StartDirectory, "slw.exe")))
            {
                Console.WriteLine("Calculating slw.exe");
                var hash = Program.ComputeSHA256Hash(File.ReadAllBytes(Path.Combine(Program.StartDirectory, "slw.exe")));
                hashes.Add("slw.exe", hash);
            }
            if (File.Exists(Path.Combine(Program.StartDirectory, "sonicgenerations.exe")))
            {
                Console.WriteLine("Calculating sonicgenerations.exe");
                var hash = Program.ComputeSHA256Hash(File.ReadAllBytes(Path.Combine(Program.StartDirectory, "sonicgenerations.exe")));
                hashes.Add("sonicgenerations.exe", hash);
            }
            if (File.Exists(Path.Combine(Program.StartDirectory, "Sonic Forces.exe")))
            {
                Console.WriteLine("Calculating Sonic Forces.exe");
                var hash = Program.ComputeSHA256Hash(File.ReadAllBytes(Path.Combine(Program.StartDirectory, "Sonic Forces.exe")));
                hashes.Add("Sonic Forces.exe", hash);
            }
            Console.WriteLine("Hashes: ");
            foreach (var pair in hashes)
            {
                Console.Write("  {0}: ", pair.Key);
                Console.CursorLeft = 24;
                Console.Write("{0}\n", Program.ByteArrayToString(pair.Value));
                File.WriteAllBytes(Path.Combine(Program.StartDirectory, pair.Key + ".sha256"), pair.Value);
            }
            Console.ReadKey();
            CurrentMenu = Menu.MainMenu;
            return true;
        }

        // Actions
        public static void DeleteConfig()
        {
            // Replaces cpkredir.ini
            Console.WriteLine("INFO: Resetting cpkredir.ini");
            if (File.Exists(Path.Combine(Program.StartDirectory, "cpkredir.ini")))
                File.Delete(Path.Combine(Program.StartDirectory, "cpkredir.ini"));
            File.WriteAllText(Path.Combine(Program.StartDirectory, "cpkredir.ini"),
                Resources.cpkredirINI);
            if (Directory.Exists(Path.Combine(Program.StartDirectory, "mods")))
            { // Delete Config in mods
                Console.WriteLine("INFO: Deleting Codes.dat");
                if (File.Exists(Path.Combine(Program.StartDirectory, "mods\\Codes.dat")))
                    File.Delete(Path.Combine(Program.StartDirectory, "mods\\Codes.dat"));
                Console.WriteLine("INFO: Deleting Patches.dat");
                if (File.Exists(Path.Combine(Program.StartDirectory, "mods\\Patches.dat")))
                    File.Delete(Path.Combine(Program.StartDirectory, "mods\\Patches.dat"));
                Console.WriteLine("INFO: Deleting Mods Database");
                if (File.Exists(Path.Combine(Program.StartDirectory, "mods\\ModsDB.ini")))
                    File.Delete(Path.Combine(Program.StartDirectory, "mods\\ModsDB.ini"));
            }
            else
            { // Create Mods folder
                Console.WriteLine("WARN: Could not find mods folder, Creating one...");
                File.Create(Path.Combine(Program.StartDirectory, "mods"));
            }
        }

        public static void WriteText(string text, ConsoleColor color)
        {
            var oldCol = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = oldCol;
        }


        public static void WriteCentredText(string text)
        {
            int x = Console.BufferWidth / 2 - text.Length / 2;
            Console.CursorLeft = x;
            Console.WriteLine(text);
        }

        public enum Menu
        {
            MainMenu        = 0,
            Exit            = 1,
            CalculateHash   = 2,
            ResetConfig     = 3,
        }


        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

    }
}
