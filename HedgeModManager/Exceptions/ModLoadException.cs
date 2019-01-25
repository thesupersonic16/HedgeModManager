using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.Exceptions
{
    public class ModLoadException : Exception
    {
        public string ModPath;
        public string Reason;
        
        public ModLoadException(string modPath, string reason)
        {
            ModPath = modPath;
            Reason = reason;
        }
        public override string Message => $"Mod \"{ModPath}\" could not be loaded due to a read error! {Reason}";
    }
}
