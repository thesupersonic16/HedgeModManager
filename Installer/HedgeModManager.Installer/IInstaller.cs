namespace HedgeModManager.Installer;

public interface IInstaller
{
    Task Install();
    Task Uninstall();
}