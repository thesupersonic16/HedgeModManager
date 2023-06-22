namespace HedgeModManager.CodeCompiler;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

public static class SyntaxFactoryEx
{
    private static Dictionary<string, TypeSyntax> mTypeSyntaxes = new Dictionary<string, TypeSyntax>();
    private static Dictionary<string, SyntaxToken> mTokens = new Dictionary<string, SyntaxToken>();
    public static TypeSyntax GetTypeSyntax(string name)
    {
        if (mTypeSyntaxes.TryGetValue(name, out var value))
        {
            return value;
        }

        var type = SyntaxFactory.ParseTypeName(name);
        mTypeSyntaxes.Add(name, type);
        return type;
    }

    public static SyntaxToken GetToken(string name)
    {
        if (mTokens.TryGetValue(name, out var value))
        {
            return value;
        }

        var type = SyntaxFactory.ParseToken(name);
        mTokens.Add(name, type);
        return type;
    }

    public static MethodDeclarationSyntax MethodDeclaration(string identifier, string returnType, BlockSyntax body, params string[] modifiers)
    {
        var mods = new SyntaxToken[modifiers.Length];

        for (int i = 0; i < mods.Length; i++)
            mods[i] = GetToken(modifiers[i]);

        return SyntaxFactory.MethodDeclaration(new SyntaxList<AttributeListSyntax>(),
            SyntaxFactory.TokenList(mods), SyntaxFactory.PredefinedType(GetToken("void")), null, GetToken(identifier), null,
            SyntaxFactory.ParameterList(), new SyntaxList<TypeParameterConstraintClauseSyntax>(), body, null,
            SyntaxFactory.Token(SyntaxKind.None));
    }
}