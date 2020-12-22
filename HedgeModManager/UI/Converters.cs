using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace HedgeModManager
{
    [ValueConversion(typeof(string), typeof(string))]
    public class ModUpdateStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var server = (string)value;
            if (string.IsNullOrEmpty(server))
                return "No";
            return HedgeApp.NetworkConfiguration.URLBlockList.Any(t => server.ToLowerInvariant().Contains(t)) ? "Blocked" : "Yes";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
