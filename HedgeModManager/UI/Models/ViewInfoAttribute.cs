using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.UI.Models
{
    public class ViewInfoAttribute : Attribute
    {
        public string Title { get; set; }
        public double? MinWidth { get; set; }
        public double? MinHeight { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }

        public ViewInfoAttribute(string title)
        {
            Title = HedgeApp.Current.Resources[title] as string ?? title;
        }

        public ViewInfoAttribute(string title, double minWidth) : this(title) => MinWidth = minWidth;

        public ViewInfoAttribute(string title, double minWidth, double minHeight) : this(title, minWidth) => MinHeight = minHeight;
    }
}
