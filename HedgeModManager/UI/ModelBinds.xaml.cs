using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace HedgeModManager.UI
{
    public partial class ModelBinds : ResourceDictionary
    {
        private void LogBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBoxBase;

            box?.ScrollToEnd();
        }
    }
}
