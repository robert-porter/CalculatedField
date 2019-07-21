using System.Collections.Generic;
using System.Linq;

namespace CalculatedField
{
    class Parser
    {
        List<Token> Tokens;
        int Index;
        List<Dictionary<TokenType, BinaryOperator>> BinaryOperatorTable;
        Dictionary<TokenType, UnaryOperator> UnaryOperatorTable;
        public List<ScriptError> Errors;

        public Parser(List<Token> tokens) 
        {
            Tokens = tokens;
            Index = 0;
            Errors = new List<ScriptError>();
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

        Token Peek()
        {
            if (Index >= Tokens.Count) return null;
            return Tokens[Index];
        }
        Token Read()
        {
            if (Index >= Tokens.Count) return null;
            return Tokens[Index++];
        }

        bool MatchAndRead(TokenType type)
        {
            if (Tokens[Index].Type == type)
            {
                Read();
                return true;
            }
            return false;
        }

        bool Match(TokenType type, int ahead = 0)
        {
            if (Index + ahead >= Tokens.Count)
                return false;

            return Tokens[Index + ahead].Type == type;
        }

        bool Expect(TokenType token)
        {
            if (MatchAndRead(token))
                return true;
            Errors.Add(new ScriptError(Tokens[Index].Column, Tokens[Index].Line, string.Format("Expected {0}, found {1}", token, Tokens[Index].Type)));
            return false;
        }

        void UnexpectedToken()
        {
            Errors.Add(new ScriptError(Tokens[Index].Column, Tokens[Index].Line, string.Format("Unexpected token ", Tokens[Index].Contents)));
        }

        public BlockExpression Parse()
        {
            var expressions = new List<Expression>();
            while (Index < Tokens.Count && Tokens[Index].Type != TokenType.EOF)
            {                
                while( MatchAndRead(TokenType.Newline)) ;
                var statement = ParseExpression();
                if (Index < Tokens.Count - 1)
                    Expect(TokenType.Newline);                
                expressions.Add(statement);
            }
            return new BlockExpression(expressions);
        }


        Expression ParseExpression()
        {
            return ParseBinaryExpression(0);
        }

        Expression ParseBinaryExpression(int precedence)
        {
            if (precedence >= BinaryOperatorTable.Count)
                return ParseUnaryExpression();

            Expression left = ParseBinaryExpression(precedence + 1);
            if (Index >= Tokens.Count - 1) return left;
            while (BinaryOperatorTable[precedence].ContainsKey(Peek().Type))
            {
                var binaryOperator = BinaryOperatorTable[precedence][Peek().Type];
                Read();
                var right = ParseBinaryExpression(precedence + 1);
                if (BinaryOperatorTable[precedence].ContainsKey(Peek().Type))
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


        Expression ParseUnaryExpression()
        {
            if (!UnaryOperatorTable.ContainsKey(Peek().Type))
                return ParseFactor();
            else 
            {
                var unaryOperator = UnaryOperatorTable[Peek().Type];
                Read();
                var right = ParseUnaryExpression();
                return new UnaryExpression(unaryOperator, right);
            }
        }

        Expression ParseFunctionCall()
        {
            string name = Read().Contents;
            Read();
            List<Expression> arguments = new List<Expression>();
            while (!Match(TokenType.CloseParen))
            {
                arguments.Add(ParseExpression());
                if (!Match(TokenType.CloseParen))
                    Expect(TokenType.Comma);
            }
            Expect(TokenType.CloseParen);
            return new FunctionCallExpression(name, arguments);
        }

        Expression ParseIf()
        {
            Expect(TokenType.If);
            while (MatchAndRead(TokenType.Newline)) ;
            Expression condition = ParseExpression();
            while (MatchAndRead(TokenType.Newline)) ;
            Expect(TokenType.Then);
            while (MatchAndRead(TokenType.Newline)) ;
            var thenBody = new List<Expression>();
            while (!(Match(TokenType.Else) || Match(TokenType.End)))
            {
                while (MatchAndRead(TokenType.Newline)) ;
                var thenExpression = ParseExpression();
                thenBody.Add(thenExpression);
                if (!(Match(TokenType.Else) || Match(TokenType.End)))
                    Expect(TokenType.Newline);
                if (Match(TokenType.Else) || Match(TokenType.End))
                    break;
            }
            while (MatchAndRead(TokenType.Newline)) ;
            if (MatchAndRead(TokenType.Else))
            {
                List<Expression> elseBody = new List<Expression>();
                while (!Match(TokenType.End))
                {
                    while (MatchAndRead(TokenType.Newline)) ;
                    var elseExpression = ParseExpression();
                    elseBody.Add(elseExpression);
                    if (!Match(TokenType.End))
                        Expect(TokenType.Newline);
                }
                Expect(TokenType.End);
                return new IfExpression(condition, new BlockExpression(thenBody), new BlockExpression(elseBody));
            }
            else
            {
                Expect(TokenType.End);
                return new IfExpression(condition, new BlockExpression(thenBody));
            }
        }


        Expression ParseFactor()
        {
            if (Match(TokenType.Identifier))
            {
                if (Match(TokenType.OpenParen, 1))
                {
                    return ParseFunctionCall();
                }
                if (Match(TokenType.Equal, 1))
                {
                    IdentifierExpression identifier = new IdentifierExpression(Read().Contents);
                    Read();
                    return new AssignmentExpression(identifier, ParseExpression());
                }
                else
                {
                    return new IdentifierExpression(Read().Contents);
                }
            }
            if (Match(TokenType.If))
            {
                return ParseIf();
            }
            else if (MatchAndRead(TokenType.OpenParen))
            {
                Expression parenthesizedExpression = ParseExpression();
                Expect(TokenType.CloseParen);
                return parenthesizedExpression;
            }
            else if (Match(TokenType.NumberLiteral))
            {
                return new DecimalLiteralExpression(Read().Contents);
            }
            else if(Match(TokenType.True) || Match(TokenType.False))
            {
                return new BoolLiteralExpression(Read().Contents);
            }
            else if (Match(TokenType.StringLiteral))
            {
                return new StringLiteralExpression(Read().Contents);
            }
            else if(Match(TokenType.IntegerLiteral))
            {
                return new IntegerLiteralExpression(Read().Contents);
            }
            else
            {
                UnexpectedToken();
                Read();
                return new DecimalLiteralExpression("0");
            }
        }
    }
}

