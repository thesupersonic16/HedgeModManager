using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HedgeModManager
{
    public static class Lang
    {

        public static string Localise(string key)
        {
            var resource = Application.Current.TryFindResource(key);
            if (resource is string str)
                return str;
            if (Debugger.IsAttached && Debugger.IsLogging())
                Console.WriteLine("Attempted to localise \"{0}\", but no such string exists!", key);
            return "UNLOCALISED";
        }
    }
}
