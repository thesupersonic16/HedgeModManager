using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.UI.Models
{
    public interface IViewModel
    {
        IView View { get; set; }
    }
}
