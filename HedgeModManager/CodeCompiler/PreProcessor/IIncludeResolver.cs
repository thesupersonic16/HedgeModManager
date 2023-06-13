using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.CodeCompiler.PreProcessor
{
    public interface IIncludeResolver
    {
        public string Resolve(string name);
    }
}
