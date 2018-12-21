using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    class CPKREDIRConfig : IniFile
    {
        // CPKREDIR
        public bool Enabled
        {
            get { return (int)Groups["Main"]["Enabled", typeof(int)] != 0; }
            set { Groups["Main"]["Enabled"] = (value ? "1" : "0"); }
        }

        public bool PlaceTocAtEnd
        {
            get { return (int)Groups["Main"]["PlaceTocAtEnd", typeof(int)] != 0; }
            set { Groups["Main"]["PlaceTocAtEnd"] = (value ? "1" : "0"); }
        }

        public bool HandleCpksWithoutExtFiles
        {
            get { return (int)Groups["Main"]["HandleCpksWithoutExtFiles", typeof(int)] != 0; }
            set { Groups["Main"]["HandleCpksWithoutExtFiles"] = (value ? "1" : "0"); }
        }

        public string LogFile
        {
            get { return Groups["Main"]["LogFile"]; }
            set { Groups["Main"]["LogFile"] = value; }
        }

        public int ReadBlockSizeKb
        {
            get { return (int)Groups["Main"]["ReadBlockSizeKB", typeof(int)]; } 
            set { Groups["Main"]["ReadBlockSizeKB"] = value.ToString(); }
        }

        public string ModsDbIni
        {
            get { return Groups["Main"]["ModsDbIni"]; }
            set { Groups["Main"]["ModsDbIni"] = value; }
        }

        public bool EnableSaveFileRedirection
        {
            get { return (int)Groups["Main"]["EnableSaveFileRedirection", typeof(int)] != 0; }
            set { Groups["Main"]["EnableSaveFileRedirection"] = (value ? "1" : "0"); }
        }

        public string SaveFileOverride
        {
            get { return Groups["Main"]["SaveFileOverride"]; }
            set { Groups["Main"]["SaveFileOverride"] = value; }
        }

        // HedgeModManager

        public bool CheckForUpdates
        {
            get { return (int)Groups["HedgeModManager"]["AutoCheckForUpdates", typeof(int)] != 0; }
            set { Groups["HedgeModManager"]["AutoCheckForUpdates"] = (value ? "1" : "0"); }
        }

        public bool KeepOpen
        {
            get { return (int)Groups["HedgeModManager"]["KeepModLoaderOpen", typeof(int)] != 0; }
            set { Groups["HedgeModManager"]["KeepModLoaderOpen"] = (value ? "1" : "0"); }
        }

        public bool CheckLoaderUpdates
        {
            get { return (int)Groups["HedgeModManager"]["CheckLoader", typeof(int)] != 0; }
            set { Groups["HedgeModManager"]["CheckLoader"] = (value ? "1" : "0"); }
        }
    }
}
