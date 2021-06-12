using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HedgeModManager.Misc
{
    public static class Extensions
    {
        public static unsafe void Replace(this string str, int start, int end, char c)
        {
            fixed (char* ptr = str)
            {
                for (int i = start; i < end; i++)
                {
                    ptr[i] = c;
                }
            }
        }

        public static async Task DownloadFileAsync(this HttpClient client, string url, string filePath, IProgress<double?> progress = null)
        {
            using (var httpResponse = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                httpResponse.EnsureSuccessStatusCode();

                using (var outputFile = File.Create(filePath, 8192, FileOptions.Asynchronous))
                    await httpResponse.Content.CopyToAsync(outputFile, progress).ConfigureAwait(false);
            }
        }
    }

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
}
