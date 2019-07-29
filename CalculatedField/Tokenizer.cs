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
        Multiply,
        Divide,
        NotEqual,
        LessThen,
        LessThanOrEqual,
        GreaterThan,
        GreaterThenOrEqual,
        Equal,
        OpenParen,
        CloseParen,
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

    class Token
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
            TokenDefinitions = new List<TokenDefinition>();
            TokenDefinitions.Add(new TokenDefinition("{[\\w\\s]*}", TokenType.Field));
            TokenDefinitions.Add(new TokenDefinition("\\+", TokenType.Plus));
            TokenDefinitions.Add(new TokenDefinition("\\-", TokenType.Minus));
            TokenDefinitions.Add(new TokenDefinition("\\*", TokenType.Multiply));
            TokenDefinitions.Add(new TokenDefinition("/", TokenType.Divide));
            TokenDefinitions.Add(new TokenDefinition("<>", TokenType.NotEqual));
            TokenDefinitions.Add(new TokenDefinition("<=", TokenType.LessThanOrEqual));
            TokenDefinitions.Add(new TokenDefinition("<", TokenType.LessThen));
            TokenDefinitions.Add(new TokenDefinition(">=", TokenType.GreaterThenOrEqual));
            TokenDefinitions.Add(new TokenDefinition(">", TokenType.GreaterThan));
            TokenDefinitions.Add(new TokenDefinition("=", TokenType.Equal));
            TokenDefinitions.Add(new TokenDefinition("\\(", TokenType.OpenParen));
            TokenDefinitions.Add(new TokenDefinition("\\)", TokenType.CloseParen));
            TokenDefinitions.Add(new TokenDefinition(",", TokenType.Comma));
            TokenDefinitions.Add(new TokenDefinition("\"([^\"\\\\]|\\\\.)*\"", TokenType.StringLiteral));
            TokenDefinitions.Add(new TokenDefinition("'([^'\\\\]|\\\\.)*'", TokenType.StringLiteral));
            TokenDefinitions.Add(new TokenDefinition("#(.*?)#", TokenType.DateTimeLiteral));
            TokenDefinitions.Add(new TokenDefinition("[0-9]+(\\.[0-9]+)?", TokenType.DecimalLiteral));
            TokenDefinitions.Add(new TokenDefinition("[0-9]+", TokenType.IntegerLiteral));
            TokenDefinitions.Add(new TokenDefinition("true|false", TokenType.BooleanLiteral));
            TokenDefinitions.Add(new TokenDefinition("not", TokenType.Not));
            TokenDefinitions.Add(new TokenDefinition("and", TokenType.And));
            TokenDefinitions.Add(new TokenDefinition("or", TokenType.Or));
            TokenDefinitions.Add(new TokenDefinition("null", TokenType.Null));
            TokenDefinitions.Add(new TokenDefinition("[a-zA-Z_]+[a-zA-Z_0-9]*", TokenType.Identifier));
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
                    errors.Add(new ScriptError(index, $"Unrecognized symbol {source[index]}"));
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
