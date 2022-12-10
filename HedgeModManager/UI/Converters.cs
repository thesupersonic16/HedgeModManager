using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Markdig;

namespace HedgeModManager
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var supportsSave = (bool)value;
            return supportsSave ? Lang.Localise("CommonUIYes") : Lang.Localise("CommonUINo");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(string), typeof(Visibility))]
    public class EmptyStringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (string)value;

            return string.IsNullOrWhiteSpace(str) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(string), typeof(Visibility))]
    public class InverseEmptyStringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (string)value;

            return !string.IsNullOrWhiteSpace(str) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class MarkdownToHtmlConverter : IValueConverter
    {
        private static MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // HTML rendering currently does not work on Linux
            if (HedgeApp.IsLinux)
                return string.Empty;

            string md = value?.ToString();
            if (string.IsNullOrEmpty(md))
                return string.Empty;

            return $@"
            <html>
                <body>
                    <style>
                        {HedgeMessageBox.GetHtmlStyleSheet()}
                    </style>
                        {Markdown.ToHtml(md, Pipeline)}
                </body>
            </html>";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool val))
                return Visibility.Collapsed;

            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility vis))
                throw new ArgumentException("Invalid argument");

            return vis == Visibility.Visible;
        }
    }


    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool)
                return Visibility.Collapsed;

            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Visibility)
                throw new ArgumentException("Invalid argument");

            return (Visibility)value == Visibility.Collapsed;
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverterLinux : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool val) || !HedgeApp.IsLinux)
                return Visibility.Collapsed;

            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility vis))
                throw new ArgumentException("Invalid argument");

            return vis == Visibility.Visible;
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverterWin : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool val) || HedgeApp.IsLinux)
                return Visibility.Collapsed;

            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility vis))
                throw new ArgumentException("Invalid argument");

            return vis == Visibility.Visible;
        }
    }


    [ValueConversion(typeof(object), typeof(bool))]
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolToGridHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                parameter = 0;
            if (parameter is string strParam)
                parameter = double.Parse(strParam);
            if (value is not bool)
                value = true;
            return (bool)value ? new GridLength((double)parameter) : new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = Application.Current.TryFindResource("HMM.Menu.DisabledColor") as Brush;
            var brushDisabled = brush;
            if (parameter is string strParam)
            {
                try { brush = Application.Current.TryFindResource(strParam) as Brush; }
                catch { };
            }
            return (bool)value ? brush : brushDisabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(ModInfo), typeof(Brush))]
    public class ModUpdateToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = Application.Current.TryFindResource("HMM.Button.ForegroundBrush") as Brush;
            var brushDisabled = Application.Current.TryFindResource("HMM.Menu.DisabledColor") as Brush;
            var brushBlocked = Application.Current.TryFindResource("HMM.Common.Red") as Brush;
            var brushCompleted = Application.Current.TryFindResource("HMM.Common.Green") as Brush;
            var brushWorking = Application.Current.TryFindResource("HMM.Window.AccentBrush") as Brush;

            var modInfo = values[0] as ModInfo;
            var updateStatus = modInfo.UpdateStatus;
            if (modInfo == null)
                return brushDisabled;

            if (updateStatus != Updates.ModUpdateFetcher.Status.NoUpdates)
            {
                switch (updateStatus)
                {
                    case Updates.ModUpdateFetcher.Status.BeginCheck:
                        return brushWorking;
                    case Updates.ModUpdateFetcher.Status.Failed:
                    case Updates.ModUpdateFetcher.Status.Blocked:
                        return brushBlocked;
                    case Updates.ModUpdateFetcher.Status.Success:
                    case Updates.ModUpdateFetcher.Status.UpToDate:
                        return brushCompleted;
                    default:
                        break;
                }
            }    

            var server = modInfo.UpdateServer;

            if (string.IsNullOrEmpty(server))
                return brushDisabled;

            if (Singleton.GetInstance<NetworkConfig>() != null &&
                Singleton.GetInstance<NetworkConfig>().IsServerBlocked(server))
                brush = brushBlocked;

            return brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    [ValueConversion(typeof(ModInfo), typeof(string))]
    public class ModUpdateToDetailsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var modInfo = values[0] as ModInfo;
            if (modInfo == null)
                return Lang.Localise("ModsUIFeatureUpdateDisable");

            var server = modInfo.UpdateServer;

            if (string.IsNullOrEmpty(server))
                return Lang.Localise("ModsUIFeatureUpdateDisable");
            if (modInfo.UpdateStatus == Updates.ModUpdateFetcher.Status.Failed)
                return Lang.Localise("ModsUIFeatureUpdateFailed");

            if (Singleton.GetInstance<NetworkConfig>() != null &&
                Singleton.GetInstance<NetworkConfig>().IsServerBlocked(server))
                return Lang.Localise("ModsUIFeatureUpdateBlocked");

            return Lang.Localise("ModsUIFeatureUpdate");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
