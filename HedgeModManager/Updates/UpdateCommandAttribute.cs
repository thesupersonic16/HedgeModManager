using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.Updates
{
    public class UpdateCommandAttribute : Attribute
    {
        public string[] Names { get; }
        public string Name => Names.First();

        public UpdateCommandAttribute(params string[] names)
        {
            Names = names;

            if (Names.Length == 0)
            {
                Names = new string[1];
                Names[0] = string.Empty;
            }
        }
    }
}
