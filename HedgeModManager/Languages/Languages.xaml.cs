using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.Languages
{
    public class LanguageList : List<LangEntry>
    {
        public static int TotalLines;
    }

    public class LangEntry
    {
        public string FileName { get; set; }
        public string Name { get; set; }
        public int Lines { get; set; }
        public bool Local { get; set; } = false;

        public override string ToString()
        {
            return Lines != LanguageList.TotalLines ? $"{Name} ({Math.Floor((float)Lines / LanguageList.TotalLines * 100)}%)" : Name;
        }
    }
}
