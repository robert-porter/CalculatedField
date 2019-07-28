using System.Collections.Generic;

namespace CalculatedField
{
    class Parser
    {
        List<Dictionary<TokenType, BinaryOperator>> BinaryOperatorTable;
        Dictionary<TokenType, UnaryOperator> UnaryOperatorTable;

        public Parser() 
        {
            BinaryOperatorTable = new List<Dictionary<TokenType, BinaryOperator>>();
            BinaryOperatorTable.Add(new Dictionary<TokenType, BinaryOperator> { [TokenType.Or] = BinaryOperator.Or });
            BinaryOperatorTable.Add(new Dictionary<TokenType, BinaryOperator> { [TokenType.And] = BinaryOperator.And });
            BinaryOperatorTable.Add(new Dictionary<TokenType, BinaryOperator>
            {
                [TokenType.Equal] = BinaryOperator.CompareEqual,
                [TokenType.LessGreater] = BinaryOperator.CompareNotEqual 
            });
            BinaryOperatorTable.Add(new Dictionary<TokenType, BinaryOperator>
            {
                [TokenType.Less] = BinaryOperator.Less,
                [TokenType.LessEqual] = BinaryOperator.LessOrEqual,
                [TokenType.Greater] = BinaryOperator.Greater,
                [TokenType.GreaterEqual] = BinaryOperator.GreaterOrEqual
            });
            BinaryOperatorTable.Add(new Dictionary<TokenType, BinaryOperator>
            {
                [TokenType.Plus] = BinaryOperator.Add,
                [TokenType.Minus] = BinaryOperator.Subtract,
            });

            BinaryOperatorTable.Add(new Dictionary<TokenType, BinaryOperator>
            {
                [TokenType.Star] = BinaryOperator.Multiply,
                [TokenType.Slash] = BinaryOperator.Divide,
                [TokenType.Percent] = BinaryOperator.DivideAndTruncate, 
            });

            UnaryOperatorTable = new Dictionary<TokenType, UnaryOperator>
            {
                [TokenType.Plus] = UnaryOperator.Plus,
                [TokenType.Minus] = UnaryOperator.Minus,
                [TokenType.Not] = UnaryOperator.Not
            };
        }

        public Syntax Parse(TokenStream ts)
        {
            return ParseBinaryExpression(ts, 0);
        }

        Syntax ParseBinaryExpression(TokenStream ts, int precedence)
        {
            if (precedence >= BinaryOperatorTable.Count)
                return ParseUnaryExpression(ts);

            Syntax left = ParseBinaryExpression(ts, precedence + 1);
            while (BinaryOperatorTable[precedence].ContainsKey(ts.Peek().Type))
            {
                var binaryOperator = BinaryOperatorTable[precedence][ts.Peek().Type];
                var token = ts.Read();
                var right = ParseBinaryExpression(ts, precedence + 1);
                if (BinaryOperatorTable[precedence].ContainsKey(ts.Peek().Type))
                {
                    left = new BinaryExpression(binaryOperator, left, right, token);
                }
                else
                {
                    return new BinaryExpression(binaryOperator, left, right, token);
                }
            }
            return left;
        }

        Syntax ParseUnaryExpression(TokenStream ts)
        {
            var token = ts.Peek();
            if (!UnaryOperatorTable.ContainsKey(token.Type))
                return ParseFactor(ts);
            else 
            {
                var unaryOperator = UnaryOperatorTable[token.Type];
                ts.Read();
                var right = ParseUnaryExpression(ts);
                return new UnaryExpression(unaryOperator, right, token);
            }
        }

        Syntax ParseFactor(TokenStream ts)
        {
            var token = ts.Read();
            switch (token.Type)
            {
                case TokenType.Identifier:
                    ts.Expect(TokenType.OpenParen);
                    List<Syntax> arguments = new List<Syntax>();
                    while (!ts.Match(TokenType.CloseParen))
                    {
                        arguments.Add(ParseBinaryExpression(ts, 0));
                        if (!ts.Match(TokenType.CloseParen))
                            ts.Expect(TokenType.Comma);
                    }
                    ts.Expect(TokenType.CloseParen);
                    return new FunctionExpression(token.Contents, arguments, token);
                case TokenType.Field:
                    return new FieldExpression(token.Contents, token);
                case TokenType.OpenParen:
                    Syntax parenthesizedExpression = ParseBinaryExpression(ts, 0);
                    ts.Expect(TokenType.CloseParen);
                    return parenthesizedExpression;
                case TokenType.DecimalLiteral:
                case TokenType.True:
                case TokenType.False:
                case TokenType.StringLiteral:
                case TokenType.DateTimeLiteral:
                case TokenType.Null:
                    return new LiteralExpression(token.Contents, token);
                default:
                    throw new ScriptError(ts.Column, ts.Line, $"Unexpected token {ts.Current().Contents}");
            }
        }
    }
}

