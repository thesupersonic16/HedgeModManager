using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HedgeModManager
{
    /// <summary>
    /// Interaction logic for ExceptionWindow.xaml
    /// </summary>
    public partial class ExceptionWindow : Window
    {

        public Exception _Exception;
        public string _ExtraInfo;
        
        public ExceptionWindow(Exception exception, string extraInfo = "") : this()
        {
            _Exception = exception;
            _ExtraInfo = extraInfo;
        }

        public ExceptionWindow()
        {
            InitializeComponent();
        }

        public string GetReport(bool useMarkdown = false)
        {
            var body = new StringBuilder();

            body.AppendLine($"HMM Info:");

            if (useMarkdown) body.AppendLine("```");
            body.AppendLine($"    Version: {App.VersionString}");
            body.AppendLine($"    Args: {string.Join(" ", App.Args)}");
            body.AppendLine($"    StartDir: {App.StartDirectory}");
            body.AppendLine($"    Process Level: " + (App.RunningAsAdmin() ? "Administrator" : "User"));
            try
            {
                body.AppendLine($"    Game: {App.CurrentGame}");
                body.AppendLine($"    SteamGame: {App.GetSteamGame(App.CurrentGame)}");
            } catch { }
            if (useMarkdown) body.AppendLine("```");

            body.AppendLine("");

            if (!string.IsNullOrEmpty(_ExtraInfo))
            {
                body.AppendLine($"Extra Information: {_ExtraInfo}");
            }

            if (_Exception != null)
            {
                body.AppendLine($"Exception:");
                if (useMarkdown) body.AppendLine("```");

                body.AppendLine($"    Type: {_Exception.GetType().Name}");
                body.AppendLine($"    Message: {_Exception.Message}");
                body.AppendLine($"    Source: {_Exception.Source}");
                body.AppendLine($"    Function: {_Exception.TargetSite}");
                if(_Exception.StackTrace != null)
                    body.AppendLine($"    StackTrace: \n    {_Exception.StackTrace.Replace("\n", "\n    ")}");

                body.AppendLine($"    InnerException: {_Exception.InnerException}");

                if (useMarkdown) body.AppendLine("```");
                body.AppendLine("");
            }


            return body.ToString();
        }

        public static void UnhandledExceptionEventHandler(Exception e, bool fatal = false)
        {
            var window = new ExceptionWindow(e);
            if (fatal)
                window.Header.Content = "HedgeModManager has ran into a Fatal Error!";
            window.ShowDialog();
        }

        private void Button_CopyLog_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(GetReport(true));
        }

        private void Button_Ignore_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Report_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/thesupersonic16/HedgeModManager/issues/new";
            url += $"?title=[{App.CurrentGame?.GameName}] ";
            url += $"&body={Uri.EscapeDataString(GetReport())}";
            Process.Start(url);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox_ExceptionInfo.Text = GetReport();
        }
    }
}
