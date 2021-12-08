using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.Exceptions
{
    public class LanguageLoadException : Exception
    {
        public string LangName;
        public LanguageLoadException(string langName, Exception loadException) : base(null, loadException)
        {
            LangName = langName;
        }

        public override string Message => $"Failed to load language \"{LangName}\". Please make sure the file structure is correct.";
    }
}
