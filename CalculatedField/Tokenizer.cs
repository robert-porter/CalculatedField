using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CalculatedField
{
    public enum TokenType
    {
        Unknown,
        Plus,
        Minus,
        Multiply,
        Divide,
        NotEqual,
        LessThen,
        LessThenOrEqual,
        GreaterThen,
        GreaterThenOrEqual,
        Equal,
        OpenParenthese,
        CloseParenthese,
        Comma,
        StringLiteral,
        DecimalLiteral,
        IntegerLiteral,
        DateTimeLiteral,
        BooleanLiteral,
        Null,
        Identifier,
        Field,
        Not,
        And,
        Or,
        EOF
    }

    public class Token
    {
        public string Contents { get; set; }
        public TokenType Type { get; set; }
        public int Index { get; set; }
    }

    class Tokenizer
    {
        class TokenDefinition
        {
            public TokenDefinition() { }
            public TokenDefinition(string regEx, TokenType type)
            {
                Regex = new Regex(regEx);
                Type = type;
            }
            public Regex Regex { get; set; }
            public TokenType Type { get; set; }
        }

        List<TokenDefinition> TokenDefinitions;

        public void CreateTokenDefinitions()
        {
            // order matters here.
            TokenDefinitions = new List<TokenDefinition>
            {
                new TokenDefinition("{[\\w\\s]*}", TokenType.Field),
                new TokenDefinition("\\+", TokenType.Plus),
                new TokenDefinition("\\-", TokenType.Minus),
                new TokenDefinition("\\*", TokenType.Multiply),
                new TokenDefinition("/", TokenType.Divide),
                new TokenDefinition("<>", TokenType.NotEqual),
                new TokenDefinition("<=", TokenType.LessThenOrEqual),
                new TokenDefinition("<", TokenType.LessThen),
                new TokenDefinition(">=", TokenType.GreaterThenOrEqual),
                new TokenDefinition(">", TokenType.GreaterThen),
                new TokenDefinition("=", TokenType.Equal),
                new TokenDefinition("\\(", TokenType.OpenParenthese),
                new TokenDefinition("\\)", TokenType.CloseParenthese),
                new TokenDefinition(",", TokenType.Comma),
                new TokenDefinition("\"([^\"\\\\]|\\\\.)*\"", TokenType.StringLiteral),
                new TokenDefinition("'([^'\\\\]|\\\\.)*'", TokenType.StringLiteral),
                new TokenDefinition("#(.*?)#", TokenType.DateTimeLiteral),
                new TokenDefinition("[0-9]+(\\.[0-9]+)?", TokenType.DecimalLiteral),
                new TokenDefinition("[0-9]+", TokenType.IntegerLiteral),
                new TokenDefinition("true|false", TokenType.BooleanLiteral),
                new TokenDefinition("not", TokenType.Not),
                new TokenDefinition("and", TokenType.And),
                new TokenDefinition("or", TokenType.Or),
                new TokenDefinition("null", TokenType.Null),
                new TokenDefinition("[a-zA-Z_]+[a-zA-Z_0-9]*", TokenType.Identifier)
            };
        }

        public (List<Token>, List<ScriptError>) Tokenize(string source)
        {
            int index = 0;
            var tokens = new List<Token>();
            var wsRegex = new Regex("[\t ]");
            var errors = new List<ScriptError>();

            while (index < source.Length)
            {
                Token token = null;
                var match = wsRegex.Match(source, index);
                if (match.Success && (match.Index - index) == 0)
                {
                    index += match.Length;
                    continue;
                }             
                foreach (var definition in TokenDefinitions)
                {
                    match = definition.Regex.Match(source, index);
                    if (match.Success && (match.Index - index) == 0)
                    {
                        var value = source.Substring(index, match.Length);
                        if (definition.Type == TokenType.StringLiteral || 
                            definition.Type == TokenType.DateTimeLiteral || 
                            definition.Type == TokenType.Field)
                        {
                            value = value.Substring(1, value.Length - 2);
                        }

                        token = new Token
                        {
                            Type = definition.Type,
                            Contents = value,
                            Index = index,
                        };
                        index += match.Length;
                        break;
                    }
                }
                if (token == null)
                {
                    var errorToken = new Token
                    {
                        Type = TokenType.Unknown,
                        Contents = source[index].ToString(),
                        Index = index,

                    };
                    errors.Add(ScriptError.UnrecognizedSymbol(errorToken, source[index].ToString()));
                    index++;
                }
                else
                {
                    tokens.Add(token);
                }
            }

            var eof = new Token
            {
                Type = TokenType.EOF,
                Contents = "End of script",
                Index = index,

            };
            tokens.Add(eof);
            return (tokens, errors);
        }

       
    }
}
