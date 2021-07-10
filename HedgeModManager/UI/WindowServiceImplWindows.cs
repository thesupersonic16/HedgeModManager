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
            model.View = new ViewImplWindows(model);
            model.View.ApplyInfo(model.GetViewInfo());
            return model.View;
        }
    }
}
