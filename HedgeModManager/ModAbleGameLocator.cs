namespace HedgeModManager;
using Foundation;
using Steam;

public class ModAbleGameLocator
{
    public static Dictionary<string, GameInfo> SteamGameList = new()
    {
        { "71340", new GameInfo("SonicGenerations.exe") },
        { "329440", new GameInfo("slw.exe") },
        { "637100", new GameInfo(Path.Combine("build", "main", "projects", "exec", "Sonic Forces.exe")) },
        { "1259790", new GameInfo("PuyoPuyoTetris2.exe") },
        { "981890", new GameInfo("musashi.exe") },
        { "1794960", new GameInfo(Path.Combine("build", "main", "projects", "exec", "SonicOrigins.exe")) },
        { "1237320", new GameInfo("SonicFrontiers.exe") },
    };

    public static List<IModAbleGame> LocateGames()
    {
        var games = new List<IModAbleGame>();
        var steamLocator = new SteamLocator();

        foreach (var steamGame in steamLocator.LocateGames())
        {
            if (SteamGameList.TryGetValue(steamGame.ID, out var info))
            {
                games.Add(new ModAbleGameGeneric(steamGame)
                {
                    Executable = info.Executable,
                    Root = Path.GetDirectoryName(Path.Combine(steamGame.Root, info.Executable))!
                });
            }
        }

        return games;
    }

    public record struct GameInfo(string Executable)
    {
    }
}