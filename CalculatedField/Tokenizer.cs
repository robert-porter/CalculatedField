using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CalculatedField
{
    enum TokenType
    {
        Plus,
        Minus,
        Star,
        Slash,
        Percent,
        EqualEqual,
        LessGreater,
        Less,
        LessEqual,
        Greater,
        GreaterEqual,
        Equal,
        OpenParen,
        CloseParen,
        Newline,
        Comma,
        StringLiteral,
        DecimalLiteral,
        IntegerLiteral,
        DateTimeLiteral, 
        Null,
        True,
        False,
        Identifier,
        Field,
        Not,
        And,
        Or,
        If,
        Then,
        Else,
        End,
        EOF
    }

    class Token
    {
        public string Contents { get; set; }
        public TokenType Type { get; set; }
        public int Index { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
    }

    class Tokenizer
    {
        public class TokenDefinition
        {
            public TokenDefinition() { }
            public TokenDefinition(string regEx, TokenType type)
            {
                Regex = new Regex(regEx);
                Type = type;
            }
            public bool IsIgnored { get; set; }
            public Regex Regex { get; set; }
            public TokenType Type { get; set; }
        }

        public List<TokenDefinition> TokenDefinitions;
        public void CreateTokenDefinitions()
        {
            // order matters here.
            TokenDefinitions = new List<TokenDefinition>();

            var ws = new TokenDefinition
            {
                Regex = new Regex("[\t ]"),
                IsIgnored = true
            };

            TokenDefinitions.Add(ws);
            TokenDefinitions.Add(new TokenDefinition("\r?\n", TokenType.Newline));
            TokenDefinitions.Add(new TokenDefinition("{[\\w\\s]*}", TokenType.Field));
            TokenDefinitions.Add(new TokenDefinition("\\+", TokenType.Plus));
            TokenDefinitions.Add(new TokenDefinition("\\-", TokenType.Minus));
            TokenDefinitions.Add(new TokenDefinition("\\*", TokenType.Star));
            TokenDefinitions.Add(new TokenDefinition("/", TokenType.Slash));
            TokenDefinitions.Add(new TokenDefinition("\\%", TokenType.Percent));
            TokenDefinitions.Add(new TokenDefinition("==", TokenType.EqualEqual));
            TokenDefinitions.Add(new TokenDefinition("<>", TokenType.LessGreater));
            TokenDefinitions.Add(new TokenDefinition("<=", TokenType.LessEqual));
            TokenDefinitions.Add(new TokenDefinition("<", TokenType.Less));
            TokenDefinitions.Add(new TokenDefinition(">=", TokenType.GreaterEqual));
            TokenDefinitions.Add(new TokenDefinition(">", TokenType.Greater));
            TokenDefinitions.Add(new TokenDefinition("=", TokenType.Equal));
            TokenDefinitions.Add(new TokenDefinition("\\(", TokenType.OpenParen));
            TokenDefinitions.Add(new TokenDefinition("\\)", TokenType.CloseParen));
            TokenDefinitions.Add(new TokenDefinition(",", TokenType.Comma));
            TokenDefinitions.Add(new TokenDefinition("\"([^\"\\\\]|\\\\.)*\"", TokenType.StringLiteral));
            TokenDefinitions.Add(new TokenDefinition("'([^'\\\\]|\\\\.)*'", TokenType.StringLiteral));
            TokenDefinitions.Add(new TokenDefinition("#(.*?)#", TokenType.DateTimeLiteral));
            TokenDefinitions.Add(new TokenDefinition("[0-9]+(\\.[0-9]+)?", TokenType.DecimalLiteral));
            TokenDefinitions.Add(new TokenDefinition("[0-9]+", TokenType.IntegerLiteral));
            TokenDefinitions.Add(new TokenDefinition("true", TokenType.True));
            TokenDefinitions.Add(new TokenDefinition("false", TokenType.False));
            TokenDefinitions.Add(new TokenDefinition("not", TokenType.Not));
            TokenDefinitions.Add(new TokenDefinition("and", TokenType.And));
            TokenDefinitions.Add(new TokenDefinition("or", TokenType.Or));
            TokenDefinitions.Add(new TokenDefinition("if", TokenType.If));
            TokenDefinitions.Add(new TokenDefinition("then", TokenType.Then));
            TokenDefinitions.Add(new TokenDefinition("else", TokenType.Else));
            TokenDefinitions.Add(new TokenDefinition("end", TokenType.End));
            TokenDefinitions.Add(new TokenDefinition("null", TokenType.Null));
            TokenDefinitions.Add(new TokenDefinition("[a-zA-Z_]+[a-zA-Z_0-9]*", TokenType.Identifier));
        }

        public IEnumerable<Token> Tokenize(string source)
        {
            var endOfLineRegex = new Regex("\r?\n");
            int currentIndex = 0;
            int currentLine = 1;
            int currentColumn = 0;

            while (currentIndex < source.Length)
            {
                TokenDefinition matchedDefinition = null;
                int matchLength = 0;

                foreach (var rule in TokenDefinitions)
                {
                    var match = rule.Regex.Match(source, currentIndex);

                    if (match.Success && (match.Index - currentIndex) == 0)
                    {
                        matchedDefinition = rule;
                        matchLength = match.Length;
                        break;
                    }
                }

                if (matchedDefinition == null)
                {
                    var errorDescription = string.Format("Unrecognized symbol '{0}'", source[currentIndex]); 
                    throw new ScriptError(currentColumn, currentLine, errorDescription);
                }
                else
                {
                    var value = source.Substring(currentIndex, matchLength);
                    if(matchedDefinition.Type == TokenType.StringLiteral || matchedDefinition.Type == TokenType.DateTimeLiteral)
                    {
                        value = value.Substring(1, value.Length - 2); // strip quotes
                    }

                    if (!matchedDefinition.IsIgnored)
                    {
                        var token = new Token
                        {
                            Type = matchedDefinition.Type,
                            Contents = value,
                            Index = currentIndex,
                            Line = currentLine,
                            Column = currentColumn
                        };
                        yield return token;
                    }
                    var endOfLineMatch = endOfLineRegex.Match(value);
                    if (endOfLineMatch.Success)
                    {
                        currentLine += 1;
                        currentColumn = value.Length - (endOfLineMatch.Index + endOfLineMatch.Length);
                    }
                    else
                    {
                        currentColumn += matchLength;
                    }

                    currentIndex += matchLength;
                }
            }

            yield return new Token
            {
                Type = TokenType.EOF,
                Contents = "",
                Index = currentIndex,
                Line = currentLine,
                Column = currentColumn
            };
        }

        public static IEnumerable<Token> FixNewlines(IEnumerable<Token> tokens)
        {
            var iterator = tokens.GetEnumerator();
            bool skip = true;
            while (iterator.MoveNext())
            {
                var token = iterator.Current;
                while (token.Type == TokenType.Newline && skip && iterator.MoveNext())
                {
                    token = iterator.Current;
                }
                switch (token.Type)
                {
                    case TokenType.Else:
                    case TokenType.Then:
                    case TokenType.Newline:
                        skip = true;
                        break;
                    default:
                        skip = false;
                        break;
                }
                if (token.Type == TokenType.Newline)
                {
                    iterator.MoveNext();
                    var nextToken = iterator.Current;
                    if (nextToken.Type == TokenType.Else || nextToken.Type == TokenType.End)
                    {
                        if (nextToken.Type == TokenType.Else)
                            skip = true;
                        yield return nextToken;
                    }
                    else
                    {
                        if (nextToken.Type != TokenType.Newline && nextToken.Type != TokenType.Else && nextToken.Type != TokenType.End)
                            yield return token;
                        if (!(skip && nextToken.Type == TokenType.Newline))
                            yield return nextToken;
                    }
                }
                else
                {
                    yield return token;
                }
            }
        }

    }
}
