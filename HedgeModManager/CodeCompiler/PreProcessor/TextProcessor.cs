namespace HedgeModManager.CodeCompiler.PreProcessor;
using System.Text;
using System.Runtime.CompilerServices;

public class TextProcessor
{
    public const string LineDefine = "__LINE__";
    public const string VariadicArgumentsDefine = "__VA_ARGS__";

    public Dictionary<string, TextMacro> Defines = new Dictionary<string, TextMacro>();
    public List<BasicLexer.DirectiveSyntax> Directives = new List<BasicLexer.DirectiveSyntax>();
    public List<Diagnostic> Diagnostics = new List<Diagnostic>();

    public IIncludeResolver IncludeResolver = DefaultIncludeResolver.Instance;

    public TextProcessor()
    {

    }

    public TextProcessor(IIncludeResolver includeResolver)
    {
        IncludeResolver = includeResolver;
    }

    public bool Defined(string name)
    {
        return Defines.ContainsKey(name);
    }

    public void UnDefine(string name)
    {
        Defines.Remove(name);
    }

    public void Define(string name)
    {
        var macro = TextMacro.FromText(name.AsMemory(), out name);
        if (Defines.ContainsKey(name))
        {
            Defines[name] = macro;
            return;
        }

        Defines.Add(name, macro);
    }

    public void Define(string name, string value)
    {
        var macro = TextMacro.FromText(name.AsMemory(), out name);
        macro.Value = value;

        if (Defines.ContainsKey(name))
        {
            Defines[name] = macro;
            return;
        }

        Defines.Add(name, macro);
    }

    public string Expand(string text)
    {
        TextMacro? value = null;
        TextMacro? lastValue = null;
        var name = text;
        var textMemory = text.AsMemory();

        // Grab the function name
        var nameToken = BasicLexer.FunctionLike(textMemory, ref name);
        var leftOver = textMemory.Slice(nameToken.Span.Length != 0 ? nameToken.Span.End : name.Length);

        while (Defines.TryGetValue(name, out value))
        {
            if (value.IsFunction)
            {
                var function = SyntaxParser.ParseFunctionCall(textMemory);

                var bodyMemory = value.Value.AsMemory();
                var builder = new StringBuilder();

                var pos = 0;

                var token = BasicLexer.ParseToken(bodyMemory, pos, true);
                pos = token.Span.End;

                while (!token.IsKind(SyntaxTokenKind.EndOfFileToken))
                {
                    if (token.IsKind(SyntaxTokenKind.IdentifierToken))
                    {
                        var macroName = token.Text.ToString();
                        if (macroName == VariadicArgumentsDefine)
                        {
                            var ellipsisIdx = value.Args.IndexOf("...");
                            if (ellipsisIdx != -1 && function.Arguments.Count > ellipsisIdx)
                            {
                                builder.Append(string.Join(", ", function.Arguments.Skip(ellipsisIdx).Select(Expand)));
                            }
                            else
                            {
                                builder.Append(string.Empty);
                            }
                        }
                        else
                        {
                            var argIdx = value.Args.IndexOf(macroName);
                            if (argIdx != -1)
                            {
                                if (argIdx < function.Arguments.Count)
                                {
                                    var arg = function.Arguments[argIdx];
                                    builder.Append(Expand(arg));
                                }
                            }
                            else
                            {
                                if (!BasicLexer.FunctionLike(bodyMemory, ref macroName, token.Span.Start).IsKind(SyntaxTokenKind.None) && Defines.ContainsKey(macroName))
                                {
                                    var macroFunc = SyntaxParser.ParseFunctionCall(bodyMemory, token.Span.Start);
                                    pos = token.Span.Start + macroFunc.WholeText.Length;

                                    for (int i = 0; i < macroFunc.Arguments.Count; i++)
                                    {
                                        if (i >= function.Arguments.Count)
                                        {
                                            break;
                                        }

                                        macroFunc.Arguments[i] = Expand(function.Arguments[i]);
                                    }

                                    builder.Append(Expand(macroFunc.ToString()));
                                }
                                else
                                {
                                    builder.Append(Expand(macroName));
                                }
                            }
                        }
                    }
                    else if (token.IsKind(SyntaxTokenKind.HashToken))
                    {
                        token = BasicLexer.ParseToken(bodyMemory, pos, true);
                        pos = token.Span.End;

                        if (token.IsKind(SyntaxTokenKind.IdentifierToken))
                        {
                            var macroName = token.Text.ToString();
                            var argIdx = value.Args.IndexOf(macroName);

                            if (argIdx != -1)
                            {
                                if (function.Arguments.Count > argIdx)
                                {
                                    builder.Append($"\"{SyntaxParser.EscapeString(function.Arguments[argIdx])}\"");
                                }
                            }
                        }
                        else if (token.IsKind(SyntaxTokenKind.HashToken))
                        {
                            token = BasicLexer.ParseToken(bodyMemory, pos, true);
                            pos = token.Span.End;

                            if (!token.IsKind(SyntaxTokenKind.EndOfFileToken))
                            {
                                var macroName = token.Text.ToString();
                                var argIdx = value.Args.IndexOf(macroName);

                                if (argIdx != -1)
                                {
                                    if (function.Arguments.Count > argIdx)
                                    {
                                        builder.Append(function.Arguments[argIdx]);
                                    }
                                }
                                else
                                {
                                    builder.Append(macroName);
                                }
                            }
                        }
                    }
                    else
                    {
                        builder.Append(token.Text);
                    }

                    token = BasicLexer.ParseToken(bodyMemory, pos, true);
                    pos = token.Span.End;
                }

                return builder.ToString();
            }

            if (lastValue == value)
            {
                break;
            }

            lastValue = value;
            name = value.Value;
        }

        return leftOver + (value?.Value ?? name);
    }

    public string Process(string text)
    {
        var builder = new StringBuilder();
        var readers = new Stack<MemoryLineReader>();
        readers.Push(new MemoryLineReader(BasicLexer.FilterComments(text.AsMemory())));

        var emitStack = 0;

        while (readers.Count > 0)
        {
            var reader = readers.Peek();
            if (reader.ReadLine(out var line))
            {
                var pos = 0;
                var token = BasicLexer.ParseToken(line, pos, true);
                if (token.IsKind(SyntaxTokenKind.HashToken))
                {
                    pos = token.Span.End;
                    var commandToken = BasicLexer.ParseToken(line, pos, true);

                    if (commandToken.IsKind(SyntaxTokenKind.IdentifierToken))
                    {
                        var command = commandToken.Text.ToString();
                        pos = commandToken.Span.End;

                        if (command is "endif" or "else" && emitStack > 0)
                        {
                            emitStack--;
                            continue;
                        }

                        if (command.StartsWith("if"))
                        {
                            if (emitStack > 0)
                            {
                                emitStack++;
                                continue;
                            }
                            var condition = line.Slice(pos).Trim();
                            if (condition.IsEmpty)
                            {
                                continue;
                            }

                            var emit = false;
                            if (command == "ifdef")
                            {
                                emit = Defined(condition.ToString());
                            }
                            else if (command == "ifndef")
                            {
                                emit = !Defined(condition.ToString());
                            }

                            if (!emit)
                            {
                                emitStack++;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (emitStack > 0)
                        {
                            continue;
                        }

                        switch (command)
                        {
                            case "define":
                            {
                                var nameToken = BasicLexer.ParseToken(line, pos, true);
                                if (nameToken.IsKind(SyntaxTokenKind.IdentifierToken))
                                {
                                    var name = nameToken.Text.ToString();
                                    var isFunction = BasicLexer.ParseToken(line, nameToken.Span.End).IsKind(SyntaxTokenKind.OpenParenToken);
                                    pos = nameToken.Span.End;

                                    if (isFunction)
                                    {
                                        name = SyntaxParser.ParseFunctionCallFast(line, nameToken.Span.Start).ToString();
                                        pos = nameToken.Span.Start + name.Length;
                                    }

                                    var value = line.Slice(pos);
                                    if (value.IsEmpty)
                                    {
                                        Define(name);
                                    }
                                    else
                                    {
                                        var macroBody = new StringBuilder();
                                        macroBody.Append(value);

                                        while (macroBody[macroBody.Length - 1] == '\\')
                                        {
                                            macroBody.Remove(macroBody.Length - 1, 1);
                                            if (!reader.ReadLine(out line))
                                            {
                                                continue;
                                            }

                                            macroBody.Append(line);
                                        }

                                        Define(name, macroBody.ToString());
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                                break;
                            }
                            case "undef":
                            {
                                var nameToken = BasicLexer.ParseToken(line, pos, true);
                                if (nameToken.IsKind(SyntaxTokenKind.IdentifierToken))
                                {
                                    UnDefine(nameToken.Text.ToString());
                                }
                                else
                                {
                                    continue;
                                }
                                break;
                            }

                            case "include":
                            {
                                var includeToken = BasicLexer.ParseToken(line, pos, true);
                                if (includeToken.IsKind(SyntaxTokenKind.StringLiteralToken))
                                {
                                    var includeText = IncludeResolver.Resolve(includeToken.ValueText.ToString());
                                    if (includeText != null)
                                    {
                                        readers.Push(new MemoryLineReader(BasicLexer.FilterComments(includeText.AsMemory())));
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                                break;
                            }

                            default:
                            {
                                var directive = new BasicLexer.DirectiveSyntax(commandToken.Text)
                                {
                                    Value = line.Slice(pos).Trim(' ', '\r', '\n', '\t', '"'),
                                };

                                Directives.Add(directive);

                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (emitStack > 0)
                    {
                        continue;
                    }
                    pos = 0;
                    while (true)
                    {
                        token = BasicLexer.ParseToken(line, pos);
                        pos = token.Span.End;
                        if (token.IsKind(SyntaxTokenKind.EndOfFileToken))
                        {
                            break;
                        }

                        if (token.IsKind(SyntaxTokenKind.IdentifierToken))
                        {
                            var name = token.Text.Span;

                            if (name.Equals(LineDefine.AsSpan(), StringComparison.Ordinal))
                            {
                                builder.Append(reader.Line);
                                continue;
                            }

                            if (Defines.ContainsKey(name.ToString()))
                            {
                                if (!BasicLexer.FunctionLike(line, ref Unsafe.NullRef<string>(), token.Span.Start).IsKind(SyntaxTokenKind.None))
                                {
                                    var func = SyntaxParser.ParseFunctionCallFast(line, token.Span.Start).ToString();
                                    pos = token.Span.Start + func.Length;
                                    builder.Append(Expand(func));
                                }
                                else
                                {
                                    builder.Append(Expand(name.ToString()));
                                }
                            }
                            else
                            {
                                builder.Append(token.Text);
                            }
                        }
                        else
                        {
                            builder.Append(token.Text);
                        }
                    }

                    builder.AppendLine();
                }
            }
            else
            {
                readers.Pop();
            }
        }

        return builder.ToString();
    }

    public struct Diagnostic
    {
        public Severity Level { get; set; }
        public string Message { get; set; }

        public static Diagnostic Create(Severity level, string message)
        {
            return new Diagnostic
            {
                Level = level,
                Message = message
            };
        }

        public static Diagnostic Info(string message) => Create(Severity.Info, message);
        public static Diagnostic Warning(string message) => Create(Severity.Warning, message);
        public static Diagnostic Error(string message) => Create(Severity.Error, message);

        public override string ToString()
        {
            return $"{Level}: {Message}";
        }

        public enum Severity
        {
            Info, Warning, Error
        }
    }
}