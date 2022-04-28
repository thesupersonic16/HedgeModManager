namespace HedgeModManager.Installer;
using IWshRuntimeLibrary;

public static class Utilities
{
    public static void CreateShortcut(string shortcutPath, string targetPath, string arguments = null)
    {
        string shortcutLocation = shortcutPath;
        WshShell shell = new WshShell();
        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

        shortcut.TargetPath = targetPath;
        shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
        shortcut.Arguments = arguments;
        shortcut.Save();
    }
}