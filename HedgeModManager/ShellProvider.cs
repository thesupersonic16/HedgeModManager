using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CA1712

namespace HedgeModManager
{
    [ComImport(),
Guid(IIDGuid.IModalWindow),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IModalWindow
    {

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
        PreserveSig]
        int Show([In] IntPtr parent);
    }

    [ComImport(),
    Guid(IIDGuid.IFileDialog),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFileDialog : IModalWindow
    {
        // Defined on IModalWindow - repeated here due to requirements of COM interop layer
        // --------------------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
        PreserveSig]
        int Show([In] IntPtr parent);

        // IFileDialog-Specific interface members
        // --------------------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileTypes([In] uint cFileTypes, [In, MarshalAs(UnmanagedType.LPArray)] COMDLG_FILTERSPEC[] rgFilterSpec);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileTypeIndex([In] uint iFileType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileTypeIndex(out uint piFileType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Unadvise([In] uint dwCookie);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOptions([In] FOS fos);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetOptions(out FOS pfos);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, FDAP fdap);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Close([MarshalAs(UnmanagedType.Error)] int hr);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetClientGuid([In] ref Guid guid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ClearClientData();

        // Not supported:  IShellItemFilter is not defined, converting to IntPtr
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
    }

    [ComImport(),
   Guid(IIDGuid.IFileOpenDialog),
   InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFileOpenDialog : IFileDialog
    {
        // Defined on IFileDialog - repeated here due to requirements of COM interop layer
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileTypes([In] uint cFileTypes, [In] ref COMDLG_FILTERSPEC rgFilterSpec);

        // Defined by IFileOpenDialog
        // ---------------------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);
    }

    [ComImport,
Guid(IIDGuid.IShellItemArray),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellItemArray
    {
        // Not supported: IBindCtx
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, out IntPtr ppvOut);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyStore([In] int Flags, [In] ref Guid riid, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyDescriptionList([In] ref PROPERTYKEY keyType, [In] ref Guid riid, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAttributes([In] SIATTRIBFLAGS dwAttribFlags, [In] uint sfgaoMask, out uint psfgaoAttribs);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCount(out uint pdwNumItems);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        // Not supported: IEnumShellItems (will use GetCount and GetItemAt instead)
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
    }

    [ComImport,
  Guid(IIDGuid.IFileDialogEvents),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFileDialogEvents
    {
        // NOTE: some of these callbacks are cancelable - returning S_FALSE means that 
        // the dialog should not proceed (e.g. with closing, changing folder); to 
        // support this, we need to use the PreserveSig attribute to enable us to return
        // the proper HRESULT
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
        PreserveSig]
        HRESULT OnFileOk([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
        PreserveSig]
        HRESULT OnFolderChanging([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psiFolder);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnFolderChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnSelectionChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnShareViolation([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, out FDE_SHAREVIOLATION_RESPONSE pResponse);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnTypeChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnOverwrite([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, out FDE_OVERWRITE_RESPONSE pResponse);
    }

    [ComImport,
   Guid(IIDGuid.IShellItem),
   InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellItem
    {
        // Not supported: IBindCtx
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDisplayName([In] SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
    }

    internal static class IIDGuid
    {
        public const string IModalWindow = "b4db1657-70d7-485e-8e3e-6fcb5a5c1802";
        public const string IFileDialog = "42f85136-db7e-439c-85f1-e4075d135fc8";
        public const string IFileOpenDialog = "d57c7288-d4ad-4768-be02-9d969532d960";
        public const string IFileSaveDialog = "84bccd23-5fde-4cdb-aea4-af64b83d78ab";
        public const string IFileDialogEvents = "973510DB-7D7F-452B-8975-74A85828D354";
        public const string IFileDialogControlEvents = "36116642-D713-4b97-9B83-7484A9D00433";
        public const string IFileDialogCustomize = "e6fdd21a-163f-4975-9c8c-a69f1ba37034";
        public const string IShellItem = "43826D1E-E718-42EE-BC55-A1E261C37BFE";
        public const string IShellItemArray = "B63EA76D-1F85-456F-A19C-48159EFA858B";
        public const string IKnownFolder = "38521333-6A87-46A7-AE10-0F16706816C3";
        public const string IKnownFolderManager = "44BEAAEC-24F4-4E90-B3F0-23D258FBB146";
        public const string IPropertyStore = "886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99";
        public const string IProgressDialog = "EBBC7C04-315E-11d2-B62F-006097DF5BD4";
    }

    internal static class CLSIDGuid
    {
        public const string FileOpenDialog = "DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7";
        public const string FileSaveDialog = "C0B4E2F3-BA21-4773-8DBA-335EC946EB8B";
        public const string KnownFolderManager = "4df0c730-df9d-4ae3-9153-aa6b82e9795a";
        public const string ProgressDialog = "F8383852-FCD3-11d1-A6B9-006097DF5BD4";
    }

    public static class KFIDGuid
    {
        public const string ComputerFolder = "0AC0837C-BBF8-452A-850D-79D08E667CA7";
        public const string Favorites = "1777F761-68AD-4D8A-87BD-30B759FA33DD";
        public const string Documents = "FDD39AD0-238F-46AF-ADB4-6C85480369C7";
        public const string Profile = "5E6C858F-0E22-4760-9AFE-EA3317B67173";
    }

    [Flags]
    public enum FOS : uint
    {
        FOS_OVERWRITEPROMPT = 0x00000002,
        FOS_STRICTFILETYPES = 0x00000004,
        FOS_NOCHANGEDIR = 0x00000008,
        FOS_PICKFOLDERS = 0x00000020,
        FOS_FORCEFILESYSTEM = 0x00000040, // Ensure that items returned are filesystem items.
        FOS_ALLNONSTORAGEITEMS = 0x00000080, // Allow choosing items that have no storage.
        FOS_NOVALIDATE = 0x00000100,
        FOS_ALLOWMULTISELECT = 0x00000200,
        FOS_PATHMUSTEXIST = 0x00000800,
        FOS_FILEMUSTEXIST = 0x00001000,
        FOS_CREATEPROMPT = 0x00002000,
        FOS_SHAREAWARE = 0x00004000,
        FOS_NOREADONLYRETURN = 0x00008000,
        FOS_NOTESTFILECREATE = 0x00010000,
        FOS_HIDEMRUPLACES = 0x00020000,
        FOS_HIDEPINNEDPLACES = 0x00040000,
        FOS_NODEREFERENCELINKS = 0x00100000,
        FOS_DONTADDTORECENT = 0x02000000,
        FOS_FORCESHOWHIDDEN = 0x10000000,
        FOS_DEFAULTNOMINIMODE = 0x20000000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public struct COMDLG_FILTERSPEC
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszSpec;
    }

    public enum SIGDN : uint
    {
        SIGDN_NORMALDISPLAY = 0x00000000,           // SHGDN_NORMAL
        SIGDN_PARENTRELATIVEPARSING = 0x80018001,   // SHGDN_INFOLDER | SHGDN_FORPARSING
        SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,  // SHGDN_FORPARSING
        SIGDN_PARENTRELATIVEEDITING = 0x80031001,   // SHGDN_INFOLDER | SHGDN_FOREDITING
        SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,  // SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
        SIGDN_FILESYSPATH = 0x80058000,             // SHGDN_FORPARSING
        SIGDN_URL = 0x80068000,                     // SHGDN_FORPARSING
        SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,     // SHGDN_INFOLDER | SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
        SIGDN_PARENTRELATIVE = 0x80080001           // SHGDN_INFOLDER
    }

    public enum FDAP
    {
        FDAP_BOTTOM = 0x00000000,
        FDAP_TOP = 0x00000001,
    }

    public enum FDE_SHAREVIOLATION_RESPONSE
    {
        FDESVR_DEFAULT = 0x00000000,
        FDESVR_ACCEPT = 0x00000001,
        FDESVR_REFUSE = 0x00000002
    }

    public enum HRESULT : long
    {
        S_FALSE = 0x0001,
        S_OK = 0x0000,
        E_INVALIDARG = 0x80070057,
        E_OUTOFMEMORY = 0x8007000E,
        ERROR_CANCELLED = 0x800704C7
    }

    public enum FDE_OVERWRITE_RESPONSE
    {
        FDEOR_DEFAULT = 0x00000000,
        FDEOR_ACCEPT = 0x00000001,
        FDEOR_REFUSE = 0x00000002
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PROPERTYKEY
    {
        public Guid fmtid;
        public uint pid;
    }

    public enum SIATTRIBFLAGS
    {
        SIATTRIBFLAGS_AND = 0x00000001, // if multiple items and the attirbutes together.
        SIATTRIBFLAGS_OR = 0x00000002, // if multiple items or the attributes together.
        SIATTRIBFLAGS_APPCOMPAT = 0x00000003, // Call GetAttributes directly on the ShellFolder for multiple attributes
    }

    [ComImport,
    ClassInterface(ClassInterfaceType.None),
    TypeLibType(TypeLibTypeFlags.FCanCreate),
    Guid(CLSIDGuid.FileOpenDialog)]
    public class FileOpenDialogRCW
    {
    }

    [ComImport,
Guid(IIDGuid.IFileOpenDialog),
CoClass(typeof(FileOpenDialogRCW))]
    public interface FileOpenDialog : IFileOpenDialog
    {
    }
}
