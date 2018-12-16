using System;
using System.Collections.Generic;
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
    /// Interaction logic for ChangeLogWindow.xaml
    /// </summary>
    public partial class ChangeLogWindow : Window
    {

        public string SoftwareName;
        public string SoftwareVersion;
        public string SoftwareChangeLog;
        public bool Update = false;

        public ChangeLogWindow(string softwareName, string softwareVersion, string softwareChangeLog) : this()
        {
            SoftwareName = softwareName;
            SoftwareVersion = softwareVersion;
            SoftwareChangeLog = softwareChangeLog;
        }
        
        public ChangeLogWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO: Convert Markdown
            SoftwareChangeLog = SoftwareChangeLog.Replace("\n- ", "\n\u2022 ");
            if (SoftwareChangeLog.StartsWith("- "))
                SoftwareChangeLog = "\u2022" + SoftwareChangeLog.Substring(1);
            if (SoftwareChangeLog.StartsWith(" + "))
                SoftwareChangeLog = "\u2022" + SoftwareChangeLog.Substring(2);

            Label_ChangeLog.Content = SoftwareChangeLog;
            if (Label_Title.Content is string)
                Label_Title.Content = string.Format(Label_Title.Content as string, SoftwareName, SoftwareVersion);
        }

        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            Update = true;
            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
