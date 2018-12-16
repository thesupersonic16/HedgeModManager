using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
        
        public ExceptionWindow(Exception exception) : this()
        {
            _Exception = exception;
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
            body.AppendLine($"    Version: {Program.VersionString}");
            body.AppendLine($"    Path: {Program.HedgeModManagerPath}");
            body.AppendLine($"    Args: {string.Join(" ", Program.Args)}");
            body.AppendLine($"    StartDir: {Program.StartDirectory}");
            body.AppendLine($"    Process Level: " + (Program.RunningAsAdmin() ? "Administrator" : "User"));
            body.AppendLine($"    Game: {Program.CurrentGame.GameName}");
            if (useMarkdown) body.AppendLine("```");

            body.AppendLine("");

            if (_Exception != null)
            {
                body.AppendLine($"Exception:");
                if (useMarkdown) body.AppendLine("```");

                body.AppendLine($"    Type: {_Exception.GetType().Name}");
                body.AppendLine($"    Message: {_Exception.Message}");
                body.AppendLine($"    Source: {_Exception.Source}");
                body.AppendLine($"    Function: {_Exception.TargetSite}");
                body.AppendLine($"    StackTrace: \n    {_Exception.StackTrace.Replace("\n", "\n    ")}");
                body.AppendLine($"    InnerException: {_Exception.InnerException}");

                if (useMarkdown) body.AppendLine("```");
                body.AppendLine("");
            }


            return body.ToString();
        }

        public static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var window = new ExceptionWindow(e.ExceptionObject as Exception);
            window.ShowDialog();
        }

        public static void UnhandledExceptionEventHandler(object sender, ThreadExceptionEventArgs e)
        {
            var window = new ExceptionWindow(e.Exception);
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
            url += $"?title=[{Program.CurrentGame.GameName}] ";
            url += $"&body={Uri.EscapeDataString(GetReport())}";
            Process.Start(url);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox_ExceptionInfo.Text = GetReport();
        }
    }
}
