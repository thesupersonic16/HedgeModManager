using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.Languages
{
    public class LanguageList : List<LangEntry>
    {
        
    }

    public class LangEntry
    {
        public string FileName { get; set; }
        public string Name { get; set; }
    }
}
