using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.Exceptions
{
    public class ModInstallException : Exception
    {
        public string Reason;

        public ModInstallException(string reason)
        {
            Reason = reason;
        }
        public override string Message => Reason;
    }
}
