using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace HedgeModManager.Installer
{
    public static class Extensions
    {
        public static string GetStringValue(this RegistryKey key, string name, string defaultValue = null)
            => key.GetValue(name, defaultValue) as string;

        public static int? GetIntValue(this RegistryKey key, string name, int? defaultValue = null)
            => (int?)key.GetValue(name, defaultValue);

        public static bool? GetBoolValue(this RegistryKey key, string name, bool? defaultValue = null)
        {
            var value = key.GetIntValue(name);
            if (value == null)
                return defaultValue;

            return value != 0;
        }
    }
}
