using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public class FolderBrowserDialog
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        protected static extern IntPtr GetActiveWindow();

        public string Title { get; set; }

        public string SelectedFolder { get; private set; }

        public bool ShowDialog()
        {
            var dialog = new FileOpenDialog();
            dialog.SetTitle(Title);
            dialog.SetOptions(FOS.FOS_PICKFOLDERS | FOS.FOS_FORCEFILESYSTEM | FOS.FOS_FILEMUSTEXIST);
            HRESULT res = (HRESULT)dialog.Show(GetActiveWindow());
            if(res == HRESULT.S_OK)
            {
                dialog.GetResult(out var item);
                item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out var sel);
                SelectedFolder = sel;
                return true;
            }
            return false;
        }
    }
}
