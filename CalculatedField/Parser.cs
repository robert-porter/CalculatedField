using System.Collections.Generic;

namespace CalculatedField
{
    class Parser
    {
        List<TokenType[]> BinaryOperators;
        TokenType[] UnaryOperators;
        List<ScriptError> Errors;
        List<Token> Tokens;
        int Index;

        public Parser(List<Token> tokens) 
        {
            Tokens = tokens;
            Index = 0;

            BinaryOperators = new List<TokenType[]>();
            BinaryOperators.Add(new TokenType[] { TokenType.Or });
            BinaryOperators.Add(new TokenType[] { TokenType.And });
            BinaryOperators.Add(new TokenType[]
            {
                TokenType.Equal,
                TokenType.NotEqual 
            });
            BinaryOperators.Add(new TokenType[]
            {
                TokenType.LessThen,
                TokenType.LessThanOrEqual,
                TokenType.GreaterThan,
                TokenType.GreaterThenOrEqual
            });
            BinaryOperators.Add(new TokenType[]
            {
                TokenType.Plus,
                TokenType.Minus,
            });

            BinaryOperators.Add(new TokenType[]
            {
                TokenType.Multiply,
                TokenType.Divide,
            });

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
                                Expect(TokenType.Comma, TokenType.CloseParenthese);
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
                    throw new ScriptError(Index, $"Unexpected token {token.Contents}");
            }
        }

        public Token Read()
        {
            if (Index >= Tokens.Count - 1)
                throw new ScriptError(Index, "Unexpected end of script");
            return Tokens[Index++];
        }

        public bool Match(params TokenType[] types)
        {
            if (Index >= Tokens.Count)
                throw new ScriptError(Index, "Unexpected end of script");
            foreach (var type in types)
                if (Tokens[Index].Type == type)
                    return true;
            return false;
        }


        public Token Expect(params TokenType[] types)
        {
            if (Index >= Tokens.Count)
                throw new ScriptError(Index, "Unexpected end of script");

            foreach (var type in types)
            {
                if (Tokens[Index].Type == type)
                {
                    var token = Tokens[Index];
                    Index++;
                    return token;
                }
            }
            var typesString = string.Join(", ", types);
            throw new ScriptError(Index, $"Unexpected token {Tokens[Index].Contents}. ");
        }
    }
}

