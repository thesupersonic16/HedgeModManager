using HedgeModManager;
using HedgeModManager.CodeCompiler;
using HedgeModManager.Foundation;
using HedgeModManager.Text;

CodeProvider.TryLoadRoslyn();

var games = ModAbleGameLocator.LocateGames();
if (!games.Any())
{
    Console.WriteLine("No games found");
    return;
}

IModAbleGame game = games.First();
await game.InitializeAsync();
var db = game.ModDatabase;

while (true)
{
    Console.Write("[HMM]$ ");
    var cmd = Console.ReadLine()?.Trim();
    var argv = TextUtilities.CommandLineToArgs(cmd.AsMemory());
    if (argv.Count == 0)
    {
        continue;
    }

    if (argv[0].Span.SequenceEqual("exit"))
    {
        break;
    }
    else if (argv[0].Span.SequenceEqual("ls"))
    {
        if (argv.Count > 1)
        {
            var name = argv[1].ToString();
            if (name == "codes")
            {
                PrintCodeList();
            }
            else if (name == "games")
            {
                PrintGames();
            }
        }
        else
        {
            PrintModList();
        }
    }
    else if (argv[0].Span.SequenceEqual("cd"))
    {
        if (argv.Count > 1)
        {
            var name = argv[1].ToString();
            await SetGame(name);
        }
        else
        {
            Console.WriteLine("Missing argument");
        }
    }
    else if (argv[0].Span.SequenceEqual("save"))
    {
        await db.Save();
    }
    else if (argv[0].Span.SequenceEqual("clear") || argv[0].Span.SequenceEqual("cls"))
    {
        Console.Clear();
    }
    else if (argv[0].Span.SequenceEqual("pwd"))
    {
        Console.WriteLine(game.Root);
    }
    else
    {
        Console.WriteLine("Unknown command");
    }
}

async Task SetGame(string name)
{
    var newGame = games.FirstOrDefault(x => x.Name == name);
    if (newGame == null)
    {
        Console.WriteLine("Game not found");
        return;
    }

    game = newGame;
    await game.InitializeAsync();
    db = game.ModDatabase;
}

void PrintModList()
{
    foreach (var mod in db.Mods)
    {
        Console.WriteLine($"[{(mod.Enabled ? 'x' : ' ')}] {mod.Title}{(mod.Attributes.HasFlag(ModAttribute.Favorite) ? " *" : "")}");
    }
}

void PrintCodeList()
{
    foreach (var code in db.Codes)
    {
        if (!code.IsExecutable())
        {
            continue;
        }

        Console.WriteLine($"[{(code.Enabled ? 'x' : ' ')}] {code.Name}");
    }
}

void PrintGames()
{
    foreach (var g in games)
    {
        Console.WriteLine(g == game ? $"{g.Name} *" : g.Name);
    }
}