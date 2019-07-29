using System.Collections.Generic;

namespace CalculatedField
{
    class Parser
    {
        List<TokenType[]> BinaryOperators;
        TokenType[] UnaryOperators;
        List<ScriptError> Errors;

        public Parser() 
        {
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

        public (Syntax, List<ScriptError>) Parse(TokenStream ts)
        {
            Errors = new List<ScriptError>();
            var syntax = ParseBinaryExpression(ts, 0);
            return (syntax, Errors);
        }

        Syntax ParseBinaryExpression(TokenStream ts, int precedence)
        {
            if (precedence >= BinaryOperators.Count)
                return ParseUnaryExpression(ts);

            Syntax left = ParseBinaryExpression(ts, precedence + 1);
            while (ts.Match(BinaryOperators[precedence]))
            {
                var token = ts.Read();
                var right = ParseBinaryExpression(ts, precedence + 1);
                if (ts.Match(BinaryOperators[precedence]))
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

        Syntax ParseUnaryExpression(TokenStream ts)
        {
            if (ts.Match(UnaryOperators))
            {
                var token = ts.Read();
                var right = ParseUnaryExpression(ts);
                return new UnaryExpression(right, token);
            }
            else 
            {
                return ParseFactor(ts);
            }
        }

        Syntax ParseFactor(TokenStream ts)
        {
            var token = ts.Read();
            switch (token.Type)
            {
                case TokenType.Identifier:
                    if (ts.Match(TokenType.OpenParen))
                    {
                        ts.Expect(TokenType.OpenParen);
                        List<Syntax> arguments = new List<Syntax>();
                        while (!ts.Match(TokenType.CloseParen))
                        {
                            arguments.Add(ParseBinaryExpression(ts, 0));
                            if (!ts.Match(TokenType.CloseParen))
                                ts.Expect(TokenType.Comma, TokenType.CloseParen);
                        }
                        ts.Expect(TokenType.CloseParen);
                        return new FunctionExpression(token.Contents, arguments, token);
                    }
                    else
                    {
                        return new IdentifierExpression(token.Contents, token);
                    }
                case TokenType.Field:
                    return new FieldExpression(token.Contents, token);
                case TokenType.OpenParen:
                    Syntax parenthesizedExpression = ParseBinaryExpression(ts, 0);
                    ts.Expect(TokenType.CloseParen);
                    return parenthesizedExpression;
                case TokenType.DecimalLiteral:
                case TokenType.BooleanLiteral:
                case TokenType.StringLiteral:
                case TokenType.DateTimeLiteral:
                case TokenType.Null:
                    return new LiteralExpression(token.Contents, token);
                default:
                    Errors.Add(new ScriptError(ts.Index, $"Unexpected token {token.Contents}"));
                    return null;
            }
        }
    }
}

