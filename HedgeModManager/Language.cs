using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HedgeModManager
{
    public class Language
    {
        public static Language CurrentLanguage { get; set; }
        public static Dictionary<string, Language> Languages = new Dictionary<string, Language>();

        public string Name;
        public string ISO;
        public Dictionary<string, string> Strings = new Dictionary<string, string>();



        public Language()
        {
        }

        public Language(string name, string iso, string text)
        {
            Name = name;
            ISO = iso;
            if (text != null)
                LoadFromString(text);
            Languages.Add(iso, this);
        }

        public void LoadFromString(string text)
        {
            foreach (string line in text.Replace("\r", "").Split('\n'))
                if (!string.IsNullOrEmpty(line) && line.IndexOf("=") != -1 && line.Length > line.IndexOf("=") + 1)
                    Strings.Add(line.Substring(0, line.IndexOf("=")), line.Substring(line.IndexOf("=") + 1).Replace("\\n", "\n"));
        }

        public static string GetLocalizedString(string text)
        {
            if (CurrentLanguage == null)
                return text;
            string value = CurrentLanguage.Strings.FirstOrDefault(t => t.Key == text).Value;
            if (string.IsNullOrEmpty(value))
                return text;
            return value;
        }

        public override string ToString()
        {
            return Name;
        }

    }

    public class LanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && parameter is string)
                return Language.GetLocalizedString(parameter as string);
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
