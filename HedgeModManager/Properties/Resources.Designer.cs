﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HedgeModManager.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("HedgeModManager.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] DAT_LOADERS_ZIP {
            get {
                object obj = ResourceManager.GetObject("DAT_LOADERS_ZIP", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to body {
        ///    background-color: transparent;
        ///    /*color: #e2e2e2;*/
        ///    color: $FORECOLOR;
        ///    font-family: &apos;Open Sans&apos;, sans-serif;
        ///}
        ///
        ///h1
        ///{
        ///    font-size: 18px;
        ///    /*margin: 0 0 0.75em;
        ///    padding: 0 0 0.25em 0;*/
        ///}
        ///
        ///li
        ///{
        ///    list-style-type: disc;
        ///}
        ///
        ///span.RedColor
        ///{
        ///    color: #FF4E4E;
        ///}
        ///
        ///span.GreenColor
        ///{
        ///    color: #6EE16C;
        ///}
        ///
        ///table {
        ///    border-spacing: 5px;
        ///}
        ///
        ///table, th, td {
        ///    border: 1px solid darkgray;
        ///    border-collapse: collapse;
        ///}
        ///
        ///th, td {
        ///    padding:  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GBStyleSheet {
            get {
                return ResourceManager.GetString("GBStyleSheet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
        /// </summary>
        internal static System.Drawing.Icon ICO_HedgeModManager {
            get {
                object obj = ResourceManager.GetObject("ICO_HedgeModManager", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap IMG_HEDGEMODMANAGER {
            get {
                object obj = ResourceManager.GetObject("IMG_HEDGEMODMANAGER", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to namespace HMMCodes
        ///{
        ///    public enum Keys
        ///    {
        ///        /// &lt;summary&gt;
        ///        ///  The bit mask to extract a key code from a key value.
        ///        /// &lt;/summary&gt;
        ///        KeyCode = 0x0000FFFF,
        ///
        ///        /// &lt;summary&gt;
        ///        ///  The bit mask to extract modifiers from a key value.
        ///        /// &lt;/summary&gt;
        ///        Modifiers = unchecked((int)0xFFFF0000),
        ///
        ///        /// &lt;summary&gt;
        ///        ///  No key pressed.
        ///        /// &lt;/summary&gt;
        ///        None = 0x00,
        ///
        ///        /// &lt;summary&gt;
        ///        ///  The left mou [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Keys {
            get {
                return ResourceManager.GetString("Keys", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.Collections.Generic;
        ///using System.Linq;
        ///using System.Text;
        ///using System.Threading.Tasks;
        ///using System.Runtime.InteropServices;
        ///
        ///namespace HMMCodes
        ///{
        ///    public static unsafe class MemoryService
        ///    {
        ///        [DllImport(&quot;kernel32.dll&quot;)]
        ///        public static extern bool VirtualProtect(IntPtr lpAddress,
        ///                IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
        ///
        ///        [DllImport(&quot;kernel32.dll&quot;, CharSet = CharSet.Auto)]
        ///        public static extern  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string MemoryService {
            get {
                return ResourceManager.GetString("MemoryService", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HedgeModManager could not detect any supported games
        ///in the current directory!
        ///
        ///Do you want run the HMM Auto Installer?.
        /// </summary>
        internal static string STR_MSG_NOGAME {
            get {
                return ResourceManager.GetString("STR_MSG_NOGAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://raw.githubusercontent.com/hedge-dev/HMMCodes/build/SonicGenerations.hmm.
        /// </summary>
        internal static string URL_BLUEBLUR_CODES {
            get {
                return ResourceManager.GetString("URL_BLUEBLUR_CODES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://github.com/thesupersonic16/HedgeModManager/raw/rewrite/HedgeModManager/Resources/ModLoader/SonicGenerationsCodeLoader.dll.
        /// </summary>
        internal static string URL_GCL_DL {
            get {
                return ResourceManager.GetString("URL_GCL_DL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://github.com/thesupersonic16/HedgeModManager/raw/rewrite/HedgeModManager/Resources/ModLoader/HE1ML.dll.
        /// </summary>
        internal static string URL_HE1ML_DL {
            get {
                return ResourceManager.GetString("URL_HE1ML_DL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://github.com/thesupersonic16/HedgeModManager/raw/rewrite/HedgeModManager/Resources/ModLoader/HE2ModLoader.dll.
        /// </summary>
        internal static string URL_HE2ML_DL {
            get {
                return ResourceManager.GetString("URL_HE2ML_DL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://raw.githubusercontent.com/hedge-dev/HMMCodes/build/SonicOrigins.hmm.
        /// </summary>
        internal static string URL_HITE_CODES {
            get {
                return ResourceManager.GetString("URL_HITE_CODES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://github.com/thesupersonic16/HedgeModManager/raw/rewrite/HedgeModManager/Resources/ModLoader/HiteModLoader.zip.
        /// </summary>
        internal static string URL_HML_DL {
            get {
                return ResourceManager.GetString("URL_HML_DL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://nightly.link/thesupersonic16/HedgeModManager/suites/{0}/artifacts/{1}.
        /// </summary>
        internal static string URL_HMM_DEV {
            get {
                return ResourceManager.GetString("URL_HMM_DEV", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://github.com/thesupersonic16/HedgeModManager.
        /// </summary>
        internal static string URL_HMM_GITHUB {
            get {
                return ResourceManager.GetString("URL_HMM_GITHUB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://raw.githubusercontent.com/thesupersonic16/HedgeModManager/rewrite/HedgeModManager/Resources/ModLoader/Loaders.ini.
        /// </summary>
        internal static string URL_HMM_LOADERS {
            get {
                return ResourceManager.GetString("URL_HMM_LOADERS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://github.com/thesupersonic16/HedgeModManager/raw/rewrite/HedgeModManager/Resources/ModLoader/LostCodeLoader.dll.
        /// </summary>
        internal static string URL_LCL_DL {
            get {
                return ResourceManager.GetString("URL_LCL_DL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://raw.githubusercontent.com/thesupersonic16/HedgeModManager/rewrite/HedgeModManager/Resources/ModLoader/Loaders.ini.
        /// </summary>
        internal static string URL_LOADERS_INI {
            get {
                return ResourceManager.GetString("URL_LOADERS_INI", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://raw.githubusercontent.com/hedge-dev/HMMCodes/build/SonicXShadowGenerations.hmm.
        /// </summary>
        internal static string URL_MILLER_SHADOW_CODES {
            get {
                return ResourceManager.GetString("URL_MILLER_SHADOW_CODES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://raw.githubusercontent.com/hedge-dev/HMMCodes/build/Generations2024.hmm.
        /// </summary>
        internal static string URL_MILLER_SONIC_CODES {
            get {
                return ResourceManager.GetString("URL_MILLER_SONIC_CODES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://raw.githubusercontent.com/hedge-dev/HMMCodes/build/TokyoOlympics2020.hmm.
        /// </summary>
        internal static string URL_MUSASHI_CODES {
            get {
                return ResourceManager.GetString("URL_MUSASHI_CODES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://raw.githubusercontent.com/hedge-dev/HMMCodes/build/SonicColorsUltimate.hmm.
        /// </summary>
        internal static string URL_RAINBOW_CODES {
            get {
                return ResourceManager.GetString("URL_RAINBOW_CODES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://raw.githubusercontent.com/hedge-dev/HMMCodes/build/SonicFrontiers.hmm.
        /// </summary>
        internal static string URL_RANGERS_CODES {
            get {
                return ResourceManager.GetString("URL_RANGERS_CODES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://github.com/thesupersonic16/HedgeModManager/raw/rewrite/HedgeModManager/Resources/ModLoader/RainbowModLoader.dll.
        /// </summary>
        internal static string URL_RML_DL {
            get {
                return ResourceManager.GetString("URL_RML_DL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://raw.githubusercontent.com/hedge-dev/HMMCodes/build/SonicLostWorld.hmm.
        /// </summary>
        internal static string URL_SONIC2013_CODES {
            get {
                return ResourceManager.GetString("URL_SONIC2013_CODES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://raw.githubusercontent.com/hedge-dev/HMMCodes/build/PuyoPuyoTetris2.hmm.
        /// </summary>
        internal static string URL_TENPEX_CODES {
            get {
                return ResourceManager.GetString("URL_TENPEX_CODES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://raw.githubusercontent.com/hedge-dev/HMMCodes/build/SonicForces.hmm.
        /// </summary>
        internal static string URL_WARS_CODES {
            get {
                return ResourceManager.GetString("URL_WARS_CODES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string Version {
            get {
                return ResourceManager.GetString("Version", resourceCulture);
            }
        }
    }
}
