namespace HedgeModManager.Foundation;

public interface IModDatabase
{
    IReadOnlyList<IMod> Mods { get; }
    IReadOnlyList<ICode> Codes { get; }

    Task Save();
}