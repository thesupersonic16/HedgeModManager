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
            return key;
        }

        public static string Localise(string key, string def)
        {
            var resource = Application.Current.TryFindResource(key);
            if (resource is string str)
                return str;
            return def;
        }

        public static string LocaliseFormat(string key, params object[] args)
        {
            return string.Format(Localise(key), args);
        }
    }
}
