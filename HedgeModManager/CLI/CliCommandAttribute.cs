using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.CLI
{
    public class CliCommandAttribute : Attribute
    {
        public string Name;
        public string Alias;
        public Type[] Inputs;
        public string Description;
        public string Usage;
        public string Example;

        public CliCommandAttribute(string name, string alias = null, Type[] inputs = null, string description = null,
            string usage = null, string example = null)
        {
            Name = name;
            Alias = alias;
            Inputs = inputs;
            Description = description;
            Usage = usage;
            Example = example;
        }
    }
}
