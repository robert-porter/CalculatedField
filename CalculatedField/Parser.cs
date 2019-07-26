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
                [TokenType.EqualEqual] = BinaryOperator.CompareEqual,
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

        public BlockExpression Parse(TokenStream ts)
        {
            var firstToken = ts.Peek();
            var expressions = new List<Expression>();
            while (!ts.EOF())
            {
                var statement = ParseExpression(ts);
                if (!ts.EOF())
                {
                    var token = ts.Current();
                    if(token.Type != TokenType.Newline)
                    {
                        throw new ScriptError(token.Column, token.Line, "Unexpected token");
                    }
                    else
                    {
                        ts.Read();
                    }
                }
                expressions.Add(statement);
            }
            return new BlockExpression(expressions, firstToken);
        }

        Expression ParseExpression(TokenStream ts)
        {
            return ParseBinaryExpression(ts, 0);
        }

        Expression ParseBinaryExpression(TokenStream ts, int precedence)
        {
            if (precedence >= BinaryOperatorTable.Count)
                return ParseUnaryExpression(ts);

            Expression left = ParseBinaryExpression(ts, precedence + 1);
            while (BinaryOperatorTable[precedence].ContainsKey(ts.PeekType()))
            {
                var binaryOperator = BinaryOperatorTable[precedence][ts.PeekType()];
                var token = ts.Read();
                var right = ParseBinaryExpression(ts, precedence + 1);
                if (BinaryOperatorTable[precedence].ContainsKey(ts.PeekType()))
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

        Expression ParseUnaryExpression(TokenStream ts)
        {
            if (!UnaryOperatorTable.ContainsKey(ts.PeekType()))
                return ParseFactor(ts);
            else 
            {
                var unaryOperator = UnaryOperatorTable[ts.PeekType()];
                var token = ts.Read();
                var right = ParseUnaryExpression(ts);
                return new UnaryExpression(unaryOperator, right, token);
            }
        }

        Expression ParseFunctionCall(TokenStream ts)
        {
            var token = ts.Read();
            ts.Expect(TokenType.OpenParen);
            List<Expression> arguments = new List<Expression>();
            while (!ts.Match(TokenType.CloseParen))
            {
                arguments.Add(ParseExpression(ts));
                if (!ts.Match(TokenType.CloseParen))
                    ts.Expect(TokenType.Comma);
            }
            ts.Expect(TokenType.CloseParen);
            return new FunctionExpression(token.Contents, arguments, token);
        }

        Expression ParseFactor(TokenStream ts)
        {
            if (ts.Match(TokenType.Identifier))
            {
                if (ts.Match(TokenType.OpenParen, 1))
                {
                    return ParseFunctionCall(ts);
                }
                if (ts.Match(TokenType.Equal, 1))
                {
                    var token = ts.Read();
                    ts.Expect(TokenType.Equal);
                    return new AssignmentExpression(token.Contents, ParseExpression(ts), token);
                }
                else
                {
                    var token = ts.Read();
                    return new IdentifierExpression(token.Contents, token);
                }
            }
            if(ts.Match(TokenType.Field))
            {
                var token = ts.Read();
                return new FieldExpression(token.Contents, token);
            }
            if (ts.Match(TokenType.If))
            {
                return ParseIf(ts);
            }
            else if (ts.Match(TokenType.OpenParen))
            {
                ts.Read();
                Expression parenthesizedExpression = ParseExpression(ts);
                ts.Expect(TokenType.CloseParen);
                return parenthesizedExpression;
            }
            else if (ts.Match(TokenType.DecimalLiteral))
            {
                var token = ts.Read();
                return new LiteralExpression(token.Contents, ScriptType.Number, token);
            }
            else if (ts.Match(TokenType.True) || ts.Match(TokenType.False))
            {
                var token = ts.Read();
                return new LiteralExpression(token.Contents, ScriptType.Bool, token);
            }
            else if (ts.Match(TokenType.StringLiteral))
            {
                var token = ts.Read();
                return new LiteralExpression(token.Contents, ScriptType.String, token);
            }
            else if(ts.Match(TokenType.DateTimeLiteral))
            {
                var token = ts.Read();
                return new LiteralExpression(token.Contents, ScriptType.DateTime, token);
            }
            else if (ts.Match(TokenType.Null))
            {
                var token = ts.Read();
                return new LiteralExpression(token.Contents, ScriptType.Null, token);
            }
            else 
            {
                throw new ScriptError(ts.Column, ts.Line, string.Format("Unexpected token ", ts.Current().Contents));
            }
        }

        Expression ParseIf(TokenStream ts)
        {
            var ifToken = ts.Expect(TokenType.If);
            Expression condition = ParseExpression(ts);
            var thenToken = ts.Expect(TokenType.Then);
            var thenBody = new List<Expression>();
            while (!(ts.Match(TokenType.Else) || ts.Match(TokenType.End)))
            {
                var thenExpression = ParseExpression(ts);
                thenBody.Add(thenExpression);
            }
            if (ts.Match(TokenType.Else))
            {
                var elseToken = ts.Read();
                if (ts.Match(TokenType.If))
                {
                    var elseBlockExpression = new BlockExpression(new List<Expression>() { ParseIf(ts) }, elseToken);
                    return new IfExpression(condition, new BlockExpression(thenBody, thenToken), elseBlockExpression, ifToken); 
                }
                List<Expression> elseBody = new List<Expression>();
                while (!ts.Match(TokenType.End))
                {
                    var elseExpression = ParseExpression(ts);
                    elseBody.Add(elseExpression);
                }
                ts.Expect(TokenType.End);
                return new IfExpression(condition, new BlockExpression(thenBody, thenToken), new BlockExpression(elseBody, elseToken), ifToken);
            }
            else
            {
                var endToken = ts.Expect(TokenType.End);
                return new IfExpression(condition, new BlockExpression(thenBody, endToken), ifToken, endToken);
            }
        }
    }
}

