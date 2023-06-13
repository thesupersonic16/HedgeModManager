﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeModManager.CodeCompiler
{
    public enum SyntaxTokenKind
    {
        None,
        BeginTriviaCount,
        EndOfFileToken,
        LoadDirectiveTrivia,
        LibDirectiveTrivia,
        ImportDirectiveTrivia,
        WhitespaceTrivia,
        SingleLineCommentTrivia,
        MultiLineCommentTrivia,
        EndTriviaCount,
        IdentifierToken,
        NumericLiteralToken,
        StringLiteralToken,
        HashToken,
        OpenBraceToken,
        CloseBraceToken,
        OpenBracketToken,
        CloseBracketToken,
        OpenParenToken,
        CloseParenToken,
        SemicolonToken,
        CommaToken,
        DotToken,
        ColonToken,
        QuestionToken,
        PlusToken,
        MinusToken,
        AsteriskToken,
        SlashToken,
        PercentToken,
        AmpersandToken,
        BarToken,
        CaretToken,
        ExclamationToken,
        TildeToken,
        EqualsToken,
        EqualsEqualsToken,
        LessThanToken,
        LessEqualsToken,
        GreaterThanToken,
        GreaterEqualsToken,
    }
}
