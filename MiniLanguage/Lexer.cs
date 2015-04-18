using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{

    enum TokenType
    {
        Plus,
        Minus,
        Times,
        Slash,
        OpenParen,
        CloseParen,
        Bang,
        And,
        Or,
        DoubleEqual,
        NotEqual,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
        Equal,
        Semicolon,
        OpenCurlyBrace,
        CloseCurlyBrace,
        OpenSquareBracket,
        CloseSquareBracket,
        Comma,
        String,
        Number,
        If,
        Else,
        While,
        True,
        False,
        Var,
        Ref,
        Return,
        Function,
        Identifier
    }

    struct Token
    {
        public String Contents { get; set; }
        public TokenType Type { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
    }

    class Lexer
    {
        Dictionary<String, TokenType> Operators;
        Dictionary<String, TokenType> Keywords;
        String SourceCode;
        char[] Characters;
        int Index;
        int Line;
        int Column;
        public List<Token> Tokens;


        public Lexer(String sourceCode)
        {
            SourceCode = sourceCode;
            Characters = SourceCode.ToCharArray();
            Index = 0;
            Tokens = new List<Token>();

            Operators = new Dictionary<string, TokenType> 
            {
                {"+", TokenType.Plus},
                {"-", TokenType.Minus},
                {"*", TokenType.Times},
                {"/", TokenType.Slash},
                {"(", TokenType.OpenParen}, 
                {")", TokenType.CloseParen},
                {";", TokenType.Semicolon},
                {"==", TokenType.DoubleEqual},
                {"!=", TokenType.NotEqual},
                {"<=", TokenType.LessOrEqual},
                {">=", TokenType.GreaterOrEqual},
                {"<", TokenType.Less},
                {">", TokenType.Greater},
                {"=", TokenType.Equal},
                {",", TokenType.Comma},
                {"{", TokenType.OpenCurlyBrace},
                {"}", TokenType.CloseCurlyBrace},
                {"[", TokenType.OpenSquareBracket},
                {"]", TokenType.CloseSquareBracket},
                {"!", TokenType.Bang},
                {"&&", TokenType.And},
                {"||", TokenType.Or},

            };

            Keywords = new Dictionary<string, TokenType>
            {
                {"true", TokenType.True},
                {"false", TokenType.False},
                {"if", TokenType.If},
                {"else", TokenType.Else},
                {"while", TokenType.While},
                {"var", TokenType.Var},
                {"ref", TokenType.Ref},
                {"return", TokenType.Return},
                {"function", TokenType.Function},
            };
        }

        public void Lex()
        {
            while (Index < SourceCode.Length)
            {
                bool tokenRead = false;
                tokenRead = TryLexOperator();
                if (!tokenRead)
                    tokenRead = TryConsumeWhitespace();
                if (!tokenRead)
                    tokenRead = TryLexNumber();
                if (!tokenRead)
                    tokenRead = TryLexWord();
                if (!tokenRead)
                    tokenRead = TryLexQuote();
                if (!tokenRead)
                {
                    System.Console.WriteLine("Unreconized token");
                    return;
                }
            }
        }

        public bool TryLexQuote()
        {
            if (Index >= Characters.Length)
                return false;

            char ch = Characters[Index];
            if (ch != '\'' && ch != '"')
                return false;
            char quoteType;
            StringBuilder stringBuilder = new StringBuilder();
            Column++;
            Index++;
            quoteType = ch;
            while (Index < Characters.Length && Characters[Index] != quoteType)
            {
                stringBuilder.Append(Characters[Index]);
                Index++;
            }

            Index++; // eat close quote

            Token token = new Token();
            token.Column = Column;
            token.Contents = stringBuilder.ToString(); // strip the quotes off
            token.Line = Line;
            token.Type = TokenType.String;
            Tokens.Add(token);

            return true;
        }
        public bool TryConsumeWhitespace()
        {

            if (Index >= Characters.Length)
                return false;

            char ch = Characters[Index];

            if (ch == ' ' || ch == '\t')
            {
                Column++;
                Index++;
                return true;
            }
            else if (ch == '\r') // TODO: support multiple newlines?
            {
                Index++;
                Line = 0;
                Column = 0;
                return true;
            }
            else if (ch == '\n')
            {
                Index++;
                return true;
            }

            return false;
        }

        void ConsumeAndAddToken(TokenType type, int length)
        {
            Token token = new Token();
            token.Line = Line;
            token.Column = Column;
            token.Contents = new String(Characters, Index, length);
            token.Type = type;
            Index += length;
            Column += length;
            Tokens.Add(token);
        }

        public bool TryLexWord()
        {
            int length = 0;
            char ch = Characters[Index];

            if (Index >= Characters.Length || !((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch == '_'))
                return false;

            while (Index + length < Characters.Length && ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') || ch == '_'))
            {
                length++;
                if (Index + length < Characters.Length)
                    ch = Characters[Index + length];

            }

            String strToken = new String(Characters, Index, length);

            if (Keywords.ContainsKey(strToken))
            {
                ConsumeAndAddToken(Keywords[strToken], length);
            }
            else
            {
                ConsumeAndAddToken(TokenType.Identifier, length);
            }

            return true;
        }

        public bool TryLexNumber()
        {
            // Any minus sign will be lexed seperately.

            int length = 0;
            while (Index + length < Characters.Length && Characters[Index + length] >= '0' && Characters[Index + length] <= '9')
                length++;
            if (length == 0)
                return false;


            if (Index + length < Characters.Length && Characters[Index + length] == '.')
            {
                length++;

                if (Index + length >= Characters.Length || !(Characters[Index + length] >= '0' && Characters[Index + length] <= '9'))
                    return false; // [0-9].
            }

            while (Index + length < Characters.Length && Characters[Index + length] >= '0' && Characters[Index + length] <= '9')
                length++;

            ConsumeAndAddToken(TokenType.Number, length);
            return true;
        }

        public bool TryLexOperator()
        {

            if (Index >= Characters.Length)
                return false;

            foreach (var op in Operators)
            {
                char[] chars = op.Key.ToCharArray();
                bool match = false;
                int length = 0;

                do
                {
                    match = chars[length] == Characters[Index + length];
                    length++;
                } while (match && length < chars.Length && Index + length < SourceCode.Length);

                if (match)
                {
                    ConsumeAndAddToken(op.Value, length);
                    return true;
                }
            }

            return false;

        }
    }
}
