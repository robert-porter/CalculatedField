using System.Collections.Generic;

namespace CalculatedField
{
    class Parser
    {
        readonly List<TokenType[]> BinaryOperators;
        readonly TokenType[] UnaryOperators;
        List<ScriptError> Errors;
        readonly List<Token> Tokens;
        int Index;

        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
            Index = 0;

            BinaryOperators = new List<TokenType[]>() {
            new TokenType[]
            {
                TokenType.Or
            },
            new TokenType[]
            {
                TokenType.And
            },
            new TokenType[]
            {
                TokenType.Equal,
                TokenType.NotEqual
            },
            new TokenType[]
            {
                TokenType.LessThen,
                TokenType.LessThenOrEqual,
                TokenType.GreaterThen,
                TokenType.GreaterThenOrEqual
            },
            new TokenType[]
            {
                TokenType.Plus,
                TokenType.Minus,
            },
            new TokenType[]
            {
                TokenType.Multiply,
                TokenType.Divide,
            }
            };

            UnaryOperators = new TokenType[]
            {
                TokenType.Plus,
                TokenType.Minus,
                TokenType.Not
            };
        }

        public (Syntax, List<ScriptError>) Parse()
        {
            Errors = new List<ScriptError>();
            try
            {
                var syntax = ParseBinaryExpression(0);
                Expect(TokenType.EOF);
                return (syntax, Errors);
            }
            catch (ScriptError error)
            {
                Errors.Add(error);
            }

            return (null, Errors);
        }

        Syntax ParseBinaryExpression(int precedence)
        {
            if (precedence >= BinaryOperators.Count)
                return ParseUnaryExpression();

            Syntax left = ParseBinaryExpression(precedence + 1);
            while (Match(BinaryOperators[precedence]))
            {
                var token = Read();
                var right = ParseBinaryExpression(precedence + 1);
                if (Match(BinaryOperators[precedence]))
                {
                    left = new BinaryExpression(left, right, token);
                }
                else
                {
                    return new BinaryExpression(left, right, token);
                }
            }
            return left;
        }

        Syntax ParseUnaryExpression()
        {
            if (Match(UnaryOperators))
            {
                var token = Read();
                var right = ParseUnaryExpression();
                return new UnaryExpression(right, token);
            }
            else 
            {
                return ParseFactor();
            }
        }

        Syntax ParseFactor()
        {
            var token = Read();
            switch (token.Type)
            {
                case TokenType.Identifier:
                    if (Match(TokenType.OpenParenthese))
                    {
                        Read();
                        List<Syntax> arguments = new List<Syntax>();
                        while (!Match(TokenType.CloseParenthese))
                        {
                            var argument = ParseBinaryExpression(0);
                            arguments.Add(argument);
                            if (!Match(TokenType.CloseParenthese))
                                Expect(TokenType.Comma);
                        }
                        Expect(TokenType.CloseParenthese);
                        return new FunctionExpression(token.Contents, arguments, token);
                    }
                    else
                    {
                        return new IdentifierExpression(token.Contents, token);
                    }
                case TokenType.Field:
                    return new FieldExpression(token.Contents, token);
                case TokenType.OpenParenthese:
                    Syntax parenthesizedExpression = ParseBinaryExpression(0);
                    Expect(TokenType.CloseParenthese);
                    return parenthesizedExpression;
                case TokenType.DecimalLiteral:
                case TokenType.BooleanLiteral:
                case TokenType.StringLiteral:
                case TokenType.DateTimeLiteral:
                case TokenType.Null:
                    return new LiteralExpression(token.Contents, token);
                default:
                    throw ScriptError.UnexpectedToken(token, token.Contents);
            }
        }

        public Token Read()
        {
            if (Index >= Tokens.Count - 1)
                throw ScriptError.UnexpectedEOF(null);
            return Tokens[Index++];
        }

        public bool Match(params TokenType[] types)
        {
            if (Index >= Tokens.Count)
                throw ScriptError.UnexpectedEOF(null);
            foreach (var type in types)
                if (Tokens[Index].Type == type)
                    return true;
            return false;
        }

        public Token Expect(params TokenType[] types)
        {
            if (Index >= Tokens.Count)
                throw ScriptError.UnexpectedEOF(null);

            foreach (var type in types)
            {
                if (Tokens[Index].Type == type)
                {
                    var token = Tokens[Index];
                    Index++;
                    return token;
                }
            }
            throw ScriptError.UnexpectedToken(Tokens[Index], Tokens[Index].Contents);
        }
    }
}

