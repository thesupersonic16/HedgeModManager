using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.UI.Models
{
    public static class ModelExtensions
    {
        public static void SetTitle(this IViewModel model, string title)
        {
            if (model.View != null)
                model.View.Title = title;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IView CreateView(this IViewModel model)
        {
            return Singleton.GetInstance<IWindowService>().CreateView(model);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ShowDialog(this IViewModel model) => model.CreateView().ShowDialog();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Show(this IViewModel model) => model.CreateView().Show();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Close(this IViewModel model) => model.View?.Close();

        public static ViewInfoAttribute GetViewInfo(this IViewModel model)
        {
            return model.GetType().GetCustomAttributes(typeof(ViewInfoAttribute), true).FirstOrDefault() as ViewInfoAttribute;
        }

        public static void ApplyInfo(this IView view, ViewInfoAttribute info)
        {
            if (info == null)
                return;

            view.Title = info.Title;

            if (info.MinWidth.HasValue) view.MinWidth = info.MinWidth.Value;
            if (info.MinHeight.HasValue) view.MinHeight = info.MinHeight.Value;
            if (info.Width.HasValue) view.Width = info.Width.Value;
            if (info.Height.HasValue) view.Height = info.Height.Value;
        }
    }
}
