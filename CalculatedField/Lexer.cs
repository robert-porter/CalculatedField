using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CalculatedField
{
    enum TokenType
    {
        Plus = 1000,
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
        NumberLiteral,
        IntegerLiteral,
        True,
        False,
        Identifier,
        Not,
        And,
        Or,
        If,
        Then,
        Else,
        End,
        Null,
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

    



    class Lexer
    {
        Dictionary<string, TokenType> Operators;
        Dictionary<string, TokenType> Keywords;
        string SourceCode;
        char[] Characters;
        int Index;
        int Line;
        int Column;
        public List<Token> Tokens;
        public List<ScriptError> Errors;

        public Lexer(string sourceCode)
        {
            SourceCode = sourceCode;
            Characters = SourceCode.ToCharArray();
            Index = 0;
            Tokens = new List<Token>();
            Errors = new List<ScriptError>();

            Operators = new Dictionary<string, TokenType> 
            {
                {"+", TokenType.Plus},
                {"-", TokenType.Minus},
                {"*", TokenType.Star},
                {"/", TokenType.Slash},
                {"%", TokenType.Percent}, 
                {"(", TokenType.OpenParen}, 
                {")", TokenType.CloseParen},
                {"==", TokenType.EqualEqual},
                {"<>", TokenType.LessGreater},
                {"<=", TokenType.LessEqual},
                {">=", TokenType.GreaterEqual},
                {"<", TokenType.Less},
                {">", TokenType.Greater},
                {"=", TokenType.Equal},
                {",", TokenType.Comma},
            };
            
            Keywords = new Dictionary<string, TokenType>
            {
                {"true", TokenType.True},
                {"false", TokenType.False},
                {"if", TokenType.If},
                {"else", TokenType.Else},
                {"and", TokenType.And },
                {"or", TokenType.Or },
                {"not", TokenType.Not },
                {"null", TokenType.Null },
                {"then", TokenType.Then },
                {"end", TokenType.End }
            };

            Lex();
        }

        void Lex()
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
                    Errors.Add(new ScriptError(Column, Line, string.Format("Invalid character ({0}).", Characters[Index])));
                    Index++;
                }
            }

            Tokens.Add(new Token
            {
                Column = Column, 
                Line = Line, 
                Contents = "", 
                Type = TokenType.EOF
            });
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
            token.Contents = stringBuilder.ToString();
            token.Line = Line;
            token.Type = TokenType.StringLiteral;
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
            else if (ch == '\r') 
            {
                Token token = new Token();
                token.Column = Column;
                token.Line = Line;
                token.Contents = "\r";
                token.Type = TokenType.Newline;
                Index++;
                Line++;
                Column = 0;

                Tokens.Add(token);
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
            token.Contents = new string(Characters, Index, length);
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

            ConsumeAndAddToken(TokenType.NumberLiteral, length);
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
        
        void CreateTokenDefinitions()
        {
            TokenDefinitions.Add(new TokenDefinition("^if", TokenType.If));
        }

        public IEnumerable<Token> Tokenize(string source)
        {
            var endOfLineRegex = new Regex("\n");
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
                    throw new Exception(string.Format("Unrecognized symbol '{0}' at index {1} (line {2}, column {3}).", source[currentIndex], currentIndex, currentLine, currentColumn));
                }
                else
                {
                    var value = source.Substring(currentIndex, matchLength);

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
    }
}
