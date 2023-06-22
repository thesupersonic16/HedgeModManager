namespace HedgeModManager.Foundation;

public static class Extensions
{
    public static bool IsExecutable<TCode>(this TCode code) where TCode : ICode
    {
        return code.Type is CodeType.Code or CodeType.Patch;
    }
}