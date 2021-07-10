using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.UI.Models
{
    public interface IView
    {
        IViewModel Model { get; }
        double MinWidth { get; set; }
        double MinHeight { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        string Title { get; set; }

        void Show();
        bool? ShowDialog();
        void Hide();
        void Close();
    }
}
