using System.Collections.Generic;
using System.Linq;

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
                        ts.ReadContents();
                    }
                }
                expressions.Add(statement);
            }
            return new BlockExpression(expressions);
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
            while (BinaryOperatorTable[precedence].ContainsKey(ts.PeekToken()))
            {
                var binaryOperator = BinaryOperatorTable[precedence][ts.PeekToken()];
                ts.ReadContents();
                var right = ParseBinaryExpression(ts, precedence + 1);
                if (BinaryOperatorTable[precedence].ContainsKey(ts.PeekToken()))
                {
                    left = new BinaryExpression(binaryOperator, left, right);
                }
                else
                {
                    return new BinaryExpression(binaryOperator, left, right);
                }
            }
            return left;
        }

        Expression ParseUnaryExpression(TokenStream ts)
        {
            if (!UnaryOperatorTable.ContainsKey(ts.PeekToken()))
                return ParseFactor(ts);
            else 
            {
                var unaryOperator = UnaryOperatorTable[ts.PeekToken()];
                ts.ReadContents();
                var right = ParseUnaryExpression(ts);
                return new UnaryExpression(unaryOperator, right);
            }
        }

        Expression ParseFunctionCall(TokenStream ts)
        {
            string name = ts.ReadContents();
            ts.Expect(TokenType.OpenParen);
            List<Expression> arguments = new List<Expression>();
            while (!ts.Match(TokenType.CloseParen))
            {
                arguments.Add(ParseExpression(ts));
                if (!ts.Match(TokenType.CloseParen))
                    ts.Expect(TokenType.Comma);
            }
            ts.Expect(TokenType.CloseParen);
            return new FunctionExpression(name, arguments);
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
                    var name = ts.ReadContents();
                    ts.Expect(TokenType.Equal);
                    return new AssignmentExpression(name, ParseExpression(ts));
                }
                else
                {
                    return new IdentifierExpression(ts.ReadContents());
                }
            }
            if(ts.Match(TokenType.Field))
            {
                return new FieldExpression(ts.ReadContents());
            }
            if (ts.Match(TokenType.If))
            {
                return ParseIf(ts);
            }
            else if (ts.MatchAndRead(TokenType.OpenParen))
            {
                Expression parenthesizedExpression = ParseExpression(ts);
                ts.Expect(TokenType.CloseParen);
                return parenthesizedExpression;
            }
            else if (ts.Match(TokenType.DecimalLiteral))
            {
                return new LiteralExpression(ts.ReadContents(), ScriptType.Decimal);
            }
            else if (ts.Match(TokenType.True) || ts.Match(TokenType.False))
            {
                return new LiteralExpression(ts.ReadContents(), ScriptType.Bool);
            }
            else if (ts.Match(TokenType.StringLiteral))
            {
                return new LiteralExpression(ts.ReadContents(), ScriptType.String);
            }
            else if (ts.Match(TokenType.IntegerLiteral))
            {
                return new LiteralExpression(ts.ReadContents(), ScriptType.Integer);
            }
            else if (ts.MatchAndRead(TokenType.Null))
            {
                return new LiteralExpression("", ScriptType.Null);
            }
            else 
            {
                throw new ScriptError(ts.Column, ts.Line, string.Format("Unexpected token ", ts.Current().Contents));
            }
        }

        Expression ParseIf(TokenStream ts)
        {
            ts.Expect(TokenType.If);
            Expression condition = ParseExpression(ts);
            ts.Expect(TokenType.Then);
            var thenBody = new List<Expression>();
            while (!(ts.Match(TokenType.Else) || ts.Match(TokenType.End)))
            {
                var thenExpression = ParseExpression(ts);
                thenBody.Add(thenExpression);
            }
            if (ts.MatchAndRead(TokenType.Else))
            {
                if (ts.Match(TokenType.If))
                {
                    return new IfExpression(condition, new BlockExpression(thenBody), new BlockExpression(new List<Expression>() { ParseIf(ts) }));
                }
                List<Expression> elseBody = new List<Expression>();
                while (!ts.Match(TokenType.End))
                {
                    var elseExpression = ParseExpression(ts);
                    elseBody.Add(elseExpression);
                }
                ts.Expect(TokenType.End);
                return new IfExpression(condition, new BlockExpression(thenBody), new BlockExpression(elseBody));
            }
            else
            {
                ts.Expect(TokenType.End);
                return new IfExpression(condition, new BlockExpression(thenBody));
            }
        }
    }
}

