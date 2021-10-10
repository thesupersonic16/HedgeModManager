using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedgeModManager.UI.Models;

namespace HedgeModManager.UI
{
    internal class WindowServiceImplWindows : IWindowService
    {
        public IView CreateView(IViewModel model)
        {
            var view = new ViewImplWindows(model);
            view.ApplyInfo(model.GetViewInfo());
            model.View = view;
            return view;
        }
    }
}
