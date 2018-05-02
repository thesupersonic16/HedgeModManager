using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Interaction logic for GBModDownloadWindow.xaml
    /// </summary>
    public partial class GBModDownloadWindow : Window
    {
        public string _Title, Author, Description, URL;

        public GBModDownloadWindow(string title, string author, string description, string url, string credits, string image)
        {
            InitializeComponent();
            _Title = title;
            Author = author;
            Description = description;
            URL = url;
            ModTitleLabel.Content = title;
            SubmitterLabel.Content = new TextBlock() { Text = credits, TextWrapping = TextWrapping.Wrap };

            
            // TODO: Don't do this, Actually get HTML to work 
            // Formatting
            description = description.Replace("<ul>", "").Replace("</ul>", "");
            description = description.Replace("<li>", "\n\u25A0").Replace("</li>", "\n");
            description = description.Replace("<br>", "\n");
            description = description.Replace("<i>", "").Replace("</i>", "");
            description = description.Replace("<b>", "").Replace("</b>", "");
            description = description.Replace("&nbsp;", "  ");

            // Description
            var doc = new FlowDocument();
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(description);
            doc.Blocks.Add(paragraph);
            DescriptionViewer.Document = doc;
            
            // Loads an Image if the submission contains one
            if (image != null)
            {
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.UriSource = new Uri(image);
                imageSource.EndInit();
                SubmissionImage.Source = imageSource;
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            // Hides the Window
            Hide();
            // Gets URL to file
            var request = (HttpWebRequest)WebRequest.Create(URL);
            var response = request.GetResponse();
            var URI = response.ResponseUri;
            response.Close();
            // Prepares the Update
            var modUpdate = new ModUpdater.ModUpdate() { Name = _Title };
            modUpdate.Files.Add(new ModUpdater.ModUpdateFile() { URL = URI.AbsoluteUri, FileName = URI.Segments.Last() });
            // Starts the Update
            new UpdateModForm(null, modUpdate).ShowDialog();
            // Closes the Window, allowing the program to close
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Closes the Window
            Close();
        }


    }
}
