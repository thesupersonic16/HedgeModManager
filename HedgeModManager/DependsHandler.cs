using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using static HedgeModManager.Lang;

namespace HedgeModManager
{
    public enum DependTypes
    {
        VS2019x86,
        VS2019x64
    }

    public class DependsHandler
    {

        public static bool CheckIfInstalled(DependTypes type)
        {
            switch (type)
            {
                case DependTypes.VS2019x86:
                    return CheckVCRuntime("x86");
                case DependTypes.VS2019x64:
                    return CheckVCRuntime("x64");
            }
            return false;
        }

        public static bool AskToInstallRuntime(string id, DependTypes type)
        {
            bool abort = false;
            // Define dependencies
            string dependName, downloadURL, fileName;
            switch (type)
            {
                case DependTypes.VS2019x86:
                    dependName = "VC++ Redistributable 2019 x86";
                    downloadURL = "https://aka.ms/vs/16/release/vc_redist.x86.exe";
                    fileName = "vc_redist.x86.exe";
                    break;
                case DependTypes.VS2019x64:
                    dependName = "VC++ Redistributable 2019 x64";
                    downloadURL = "https://aka.ms/vs/16/release/vc_redist.x64.exe";
                    fileName = "vc_redist.x64.exe";
                    break;
                default:
                    throw new Exception("Unknown dependency!");
            }
            if (HedgeApp.CurrentGame.AppID == id && !CheckIfInstalled(type))
            {
                var dialog = new HedgeMessageBox(Localise("MainUIRuntimeMissingTitle"), string.Format(Localise("MainUIRuntimeMissingMsg"), HedgeApp.CurrentGame.GameName, dependName));

                dialog.AddButton(Localise("CommonUINo"), () =>
                {
                    abort = true;
                    dialog.Close();
                });
                dialog.AddButton(Localise("CommonUIYes"), () =>
                {
                    dialog.Visibility = Visibility.Hidden;
                    DownloadWindow window = new DownloadWindow($"Downloading {dependName}...", downloadURL, fileName);
                    window.Start();
                    if (File.Exists(fileName))
                    {
                        switch (type)
                        {
                            case DependTypes.VS2019x86:
                            case DependTypes.VS2019x64:
                                var startInfo = new ProcessStartInfo();
                                startInfo.Verb = "runas";
                                startInfo.FileName = fileName;
                                startInfo.Arguments = "/norestart";
                                Process.Start(startInfo);
                                break;
                            default:
                                throw new Exception("Unknown dependency to install!");
                        }

                        // Clean if possible
                        try { File.Delete(fileName); } catch { }
                    }
                    dialog.Close();
                });

                dialog.ShowDialog();
            }
            return abort;
        }

        private static bool CheckVCRuntime(string platform)
        {
            var reg = Registry.LocalMachine.OpenSubKey($"Software\\WOW6432Node\\Microsoft\\VisualStudio\\14.0\\VC\\Runtimes\\{platform}");
            // If null then try get it from the 32-bit Registry
            if (reg == null)
                reg = Registry.LocalMachine.OpenSubKey($"Software\\Microsoft\\VisualStudio\\14.0\\VC\\Runtimes\\{platform}");

            if (reg == null || !((int)reg.GetValue("Installed", 0) != 0 && (int)reg.GetValue("Bld", 0) >= 28508))
            {
                reg?.Close();
                return false;
            }
            reg.Close();
            return true;
        }

    }
}
