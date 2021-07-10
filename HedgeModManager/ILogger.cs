using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public interface ILogger
    {
        void Write(string str);
        void WriteLine(string str);
    }
}
