using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.Misc
{
    public class DllResource : IDisposable
    {
        public readonly IntPtr ModuleHandle;

        public DllResource(IntPtr handle)
        {
            ModuleHandle = handle;
        }

        public DllResource(string modulePath)
        {
            ModuleHandle = Win32.LoadLibraryEx(modulePath, IntPtr.Zero, LoadLibraryFlags.LOAD_LIBRARY_AS_DATAFILE);
        }

        public string GetString(uint id)
        {
            return Win32.LoadString(ModuleHandle, id);
        }

        private void ReleaseUnmanagedResources()
        {
            Win32.FreeLibrary(ModuleHandle);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~DllResource()
        {
            ReleaseUnmanagedResources();
        }
    }
}
