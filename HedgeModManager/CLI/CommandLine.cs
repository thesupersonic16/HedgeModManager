using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.CLI
{
    public class CommandLine
    {

        public static Dictionary<CliCommandAttribute, Type> RegisteredCommands = new ();

        static CommandLine()
        {
            RegisterCommands(Assembly.GetExecutingAssembly());
        }

        public static void RegisterCommands(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(t =>
                t.GetCustomAttribute<CliCommandAttribute>() != null && typeof(ICliCommand).IsAssignableFrom(t));
            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<CliCommandAttribute>();
                RegisteredCommands[attribute] = type;
            }
        }

        public static void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine($"HedgeModManager {HedgeApp.VersionString}\n");

            Console.WriteLine("Commands:");

            foreach (var command in RegisteredCommands)
            {
                if (command.Key.Description != null)
                    Console.WriteLine("    --{0}: {1}", command.Key.Name, command.Key.Description.Replace("\n", "\n          "));
                else
                    Console.WriteLine("    --{0}", command.Key.Name);
                if (command.Key.Usage != null)
                    Console.WriteLine("        Usage: {0}", command.Key.Usage);
                if (command.Key.Example != null)
                    Console.WriteLine("        Example: {0}", command.Key.Example);
            }
        }

        public static List<Command> ParseArguments(string[] args)
        {
            var commands = new List<Command>();
            
            for (int i = 0; i < args.Length; ++i)
            {
                // Check if it is a command
                if (args[i].StartsWith("-"))
                {
                    var command = RegisteredCommands.FirstOrDefault(x => x.Key.Alias == args[i].Substring(1));
                    if (args[i].StartsWith("--"))
                        command = RegisteredCommands.FirstOrDefault(x => x.Key.Name == args[i].Substring(2));
                    // Check if deprecated name is used
                    if (command.Key == null)
                    {
                        foreach (var commandDef in RegisteredCommands)
                        {
                            foreach (var deprecatedName in 
                                commandDef.Value.GetCustomAttributes<CliDeprecatedCommandNameAttribute>())
                            {
                                if (deprecatedName.Name == args[i])
                                {
                                    command = commandDef;
                                    Console.WriteLine("Warning: Deprecated command name ({0}) was used, please use --{1} instead",
                                        deprecatedName.Name, commandDef.Key.Name);
                                }
                            }
                        }
                    }
                    if (command.Key == null)
                        continue;

                    // Check input count
                    if (i + command.Key.Inputs?.Length > args.Length)
                    {
                        Console.WriteLine("Too few inputs for --{0}", command.Key.Name);
                        continue;
                    }

                    // Read inputs
                    var inputs = new List<object>();
                    foreach (var input in command.Key.Inputs ?? new Type[] { })
                    {
                        i++;
                        // Break if new command is found
                        if (args[i].StartsWith("-"))
                        {
                            Console.WriteLine("Too few inputs for --{0}", command.Key.Name);
                            break;
                        }

                        var data = ReadTypeFromString(Type.GetTypeCode(input), args[i]);

                        if (data != null)
                            inputs.Add(data);
                        else
                            Console.WriteLine("Unknown type {0} for --{1}", input.Name, command.Key.Name);
                    }
                    commands.Add(new Command(command.Key, command.Value, inputs));
                }
                else
                {
                    var lastCommand = commands.LastOrDefault();
                    if (lastCommand != null)
                        lastCommand.Inputs.Add(args[i]);
                }
            }
            return commands;
        }

        public static object ReadTypeFromString(TypeCode typeCode, string data)
        {
            switch (typeCode)
            {
                case TypeCode.String:
                    return data;
                case TypeCode.Int32:
                    return int.Parse(data);
                case TypeCode.Boolean:
                    return bool.Parse(data);
                default:
                    return null;
            }
        }

        public static void ExecuteArguments(List<Command> commands)
        {
            foreach (var command in commands)
            {
                var commandInstance = (ICliCommand)Activator.CreateInstance(command.Type);
                commandInstance.Execute(commands, command);
            }
        }

        public class Command
        {
            public CliCommandAttribute CommandAttribute { get; set; }
            public Type Type { get; set; }
            public List<object> Inputs { get; set; }

            public Command(CliCommandAttribute cliCommandAttribute, Type type, List<object> inputs)
            {
                CommandAttribute = cliCommandAttribute;
                Type = type;
                Inputs = inputs;
            }
        }
    }
}
