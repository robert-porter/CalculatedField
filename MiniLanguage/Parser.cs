using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class Parser
    {

        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
        }



        List<Token> Tokens;
        int Index;

        Token Peek()
        {
            return Tokens[Index];
        }

        Token Peek(int ahead)
        {
            return Tokens[Index + ahead];
        }
        Token Read()
        {
            return Tokens[Index++];
        }

        bool Accept(TokenType type)
        {
            if (Peek().Type == type)
            {
                Read();
                return true;
            }
            return false;
        }

        bool Peek(TokenType type)
        {
            if (Index >= Tokens.Count)
                return false;

            return Peek().Type == type;
        }

        bool Peek(TokenType type, int ahead)
        {
            if (Index + ahead >= Tokens.Count)
                return false;

            return Peek(ahead).Type == type;
        }

        bool Expect(TokenType token)
        {
            if (Accept(token))
                return true;
            System.Console.WriteLine("Error Expected...");
            throw new Exception();
            //return false;
        }

        void Error(String error)
        {
            System.Console.WriteLine(error);
            throw new Exception();
        }


        List<Expression> ParseFunctionCallArguments()
        {
            List<Expression> arguments = new List<Expression>();
            while (!Peek(TokenType.CloseParen))
            {
                arguments.Add(ParseExpression());
                if (!Peek(TokenType.CloseParen))
                    Expect(TokenType.Comma);
            }

            return arguments;
        }

        Expression ParseFactor()
        {
            if (Peek(TokenType.Identifier))
            {
                if (Peek(TokenType.OpenParen, 1))
                {
                    FunctionCallExpression callExpression = new FunctionCallExpression();
                    callExpression.Identifier = new IdentifierExpression(Read().Contents);
                    Read();
                    callExpression.Arguments = ParseFunctionCallArguments();
                    Expect(TokenType.CloseParen);
                    return callExpression;
                }
                else 
                    return new IdentifierExpression(Read().Contents);
            }
            else if (Peek(TokenType.Number))
            {
                return new NumberExpression(Read().Contents);
            }
            else if (Accept(TokenType.OpenParen))
            {
                Expression e = ParseExpression();
                Expect(TokenType.CloseParen);

                return e;
            }
            else
            {
                Error("factor: syntax error");
            }

            return null;
        }

        Expression ParseTerm()
        {
            BinaryExpression rootBinaryExpression = new BinaryExpression();
            BinaryExpression binaryExpression = rootBinaryExpression;
            Expression rightFactor;

            rootBinaryExpression.Left = ParseFactor();
            while (Peek(TokenType.Times) || Peek(TokenType.Slash))
            {
                if (Peek(TokenType.Times))
                    binaryExpression.Op = BinaryExpression.Operator.Multiply;
                else if (Peek(TokenType.Slash))
                    binaryExpression.Op = BinaryExpression.Operator.Divide;

                Read();
                rightFactor = ParseFactor();

                if ((Peek(TokenType.Times) || Peek(TokenType.Slash)))
                {
                    // Make a*b*c into the ast (* a(* b c))
                    BinaryExpression rightExpression = new BinaryExpression();
                    rightExpression.Left = rightFactor;
                    binaryExpression.Right = rightExpression;
                    binaryExpression = rightExpression;
                }
                else
                {
                    binaryExpression.Right = rightFactor;
                }
            }

            if (rootBinaryExpression.Right == null)
                return rootBinaryExpression.Left;
            else
                return rootBinaryExpression;
        }

        Expression ParseArithmeticExpression()
        {
            UnaryExpression unaryExpression = null;
            BinaryExpression rootBinaryExpression = new BinaryExpression();
            BinaryExpression binaryExpression = rootBinaryExpression;

            if (Peek(TokenType.Plus))
            {
                unaryExpression = new UnaryExpression(UnaryExpression.Operator.Plus);
                Read();
            }
            else if (Peek(TokenType.Minus))
            {
                unaryExpression = new UnaryExpression(UnaryExpression.Operator.Minus);
                Read();
            }
            else if (Peek(TokenType.Bang))
            {
                unaryExpression = new UnaryExpression(UnaryExpression.Operator.Not);
                Read();
            }

            rootBinaryExpression.Left = ParseTerm();

            Expression rightTerm;

            while (Peek(TokenType.Plus) || Peek(TokenType.Minus))
            {
                if (Peek(TokenType.Plus))
                    binaryExpression.Op = BinaryExpression.Operator.Add;
                else if (Peek(TokenType.Minus))
                    binaryExpression.Op = BinaryExpression.Operator.Subtract;

                Read();
                rightTerm = ParseTerm();

                if (Peek(TokenType.Plus) || Peek(TokenType.Minus))
                {
                    // Make a+b+c into the ast (+ a(+ b c))
                    BinaryExpression rightExpression = new BinaryExpression();
                    rightExpression.Left = rightTerm;
                    binaryExpression.Right = rightExpression;
                    binaryExpression = rightExpression;
                }
                else
                {
                    binaryExpression.Right = rightTerm;
                }
            }

            Expression expression;
            if (rootBinaryExpression.Right == null)
                expression = rootBinaryExpression.Left;
            else expression = rootBinaryExpression;

            if (unaryExpression != null)
            {
                unaryExpression.Argument = expression;
                return unaryExpression;
            }

            return expression;
        }

        // put == and != before this
        Expression ParseConditionalExpression()
        {

            BinaryExpression binaryExpression = new BinaryExpression();
            binaryExpression.Left = ParseArithmeticExpression();

            if (Peek(TokenType.DoubleEqual))
                binaryExpression.Op = BinaryExpression.Operator.DoubleEqual;
            else if (Peek(TokenType.NotEqual))
                binaryExpression.Op = BinaryExpression.Operator.NotEqual;
            else if (Peek(TokenType.Less))
                binaryExpression.Op = BinaryExpression.Operator.Less;
            else if (Peek(TokenType.LessOrEqual))
                binaryExpression.Op = BinaryExpression.Operator.LessOrEqual;
            else if (Peek(TokenType.Greater))
                binaryExpression.Op = BinaryExpression.Operator.Greater;
            else if (Peek(TokenType.GreaterOrEqual))
                binaryExpression.Op = BinaryExpression.Operator.GreaterOrEqual;
            else
                return binaryExpression.Left;

            Read();

            binaryExpression.Right = ParseArithmeticExpression();

            return binaryExpression;
        }

        Expression ParseAndExpression()
        {
            BinaryExpression rootBinaryExpression = new BinaryExpression();
            BinaryExpression binaryExpression = rootBinaryExpression;

            rootBinaryExpression.Left = ParseConditionalExpression();

            Expression right;

            while (Peek(TokenType.And))
            {
                if (Peek(TokenType.And))
                    binaryExpression.Op = BinaryExpression.Operator.And;

                Read();
                right = ParseConditionalExpression();

                if (Peek(TokenType.And))
                {
                    BinaryExpression newRight = new BinaryExpression();
                    binaryExpression.Right = newRight;
                    newRight.Left = right;
                    binaryExpression = newRight;
                }
                else
                {
                    binaryExpression.Right = right;
                }

            }

            Expression expression;
            if (rootBinaryExpression.Right == null)
                expression = rootBinaryExpression.Left;
            else expression = rootBinaryExpression;

            return expression;
        }

        //top level operator, parses or expression

        Expression ParseExpression()
        {
            return ParseOr();
        }

        List<IdentifierExpression> ParseFunctionDeclarationArguments()
        {
            if (Peek(TokenType.CloseParen))
                return null;

            List<IdentifierExpression> arguments = new List<IdentifierExpression>();
            while (!Peek(TokenType.CloseParen))
            {
                if (Peek(TokenType.Identifier))
                    arguments.Add(new IdentifierExpression(Read().Contents));
                else Error("Identifier expected");

                if (!Peek(TokenType.CloseParen))
                    Expect(TokenType.Comma);
            }

            return arguments;
        }

        Expression ParseOr()
        {

            BinaryExpression rootBinaryExpression = new BinaryExpression();
            BinaryExpression binaryExpression = rootBinaryExpression;

            rootBinaryExpression.Left = ParseAndExpression();

            Expression right;

            while (Peek(TokenType.Or))
            {
                if (Peek(TokenType.Or))
                    binaryExpression.Op = BinaryExpression.Operator.Or;

                Read();
                right = ParseAndExpression();

                if (Peek(TokenType.Or))
                {
                    BinaryExpression newRight = new BinaryExpression();
                    newRight.Left = right;
                    binaryExpression.Right = newRight;
                    binaryExpression = newRight;
                }
                else
                {
                    binaryExpression.Right = right;
                }

            }

            Expression expression;
            if (rootBinaryExpression.Right == null)
                expression = rootBinaryExpression.Left;
            else expression = rootBinaryExpression;

            return expression;
        }

        Statement ParseStatement()
        {
            if (Peek(TokenType.Identifier) && Peek(TokenType.Equal, 1))
            {
                IdentifierExpression identifier = new IdentifierExpression(Read().Contents);

                AssignmentStatement assignmentStatement = new AssignmentStatement();
                assignmentStatement.Left = identifier;
                Expect(TokenType.Equal);
                assignmentStatement.Right = ParseExpression();

                Expect(TokenType.Semicolon);

                return assignmentStatement;
            }
            else if (Accept(TokenType.OpenBrace))
            {
                BlockStatement blockStatement = new BlockStatement();
                while (!Peek(TokenType.CloseBrace))
                {
                    Statement statement = ParseStatement();
                    blockStatement.Statements.Add(statement);
                }

                Expect(TokenType.CloseBrace);
                return blockStatement;
            }
            else if (Accept(TokenType.If))
            {
                IfStatement ifStatement = new IfStatement();
                Expect(TokenType.OpenParen);
                ifStatement.Condition = ParseExpression();
                Expect(TokenType.CloseParen);
                ifStatement.Consequent = ParseStatement();

                if (Peek(TokenType.Else))
                {
                    Read();
                    ifStatement.Alternate = ParseStatement();
                }
                return ifStatement;
            }
            else if (Accept(TokenType.While))
            {
                WhileStatement whileStatement = new WhileStatement();
                Expect(TokenType.OpenParen);
                whileStatement.Condition = ParseExpression();
                Expect(TokenType.CloseParen);
                whileStatement.Body = ParseStatement();

                return whileStatement;
            }
            else if (Accept(TokenType.Var))
            {
                VarDeclarationStatement varDeclStatement = new VarDeclarationStatement();
                if (!Peek(TokenType.Identifier))
                    Error("identifier expected");
                varDeclStatement.Identifier = Read().Contents;

                if (Peek(TokenType.Equal))
                {
                    Read();
                    varDeclStatement.InitialValue = ParseExpression();
                }
                Expect(TokenType.Semicolon);

                return varDeclStatement;
            }
            else if (Accept(TokenType.Function))
            {
                FunctionDeclarationStatement funcDecl = new FunctionDeclarationStatement();
                if (!Peek(TokenType.Identifier))
                    throw new Exception("Identifier expected");
                funcDecl.Name = Read().Contents;

                Expect(TokenType.OpenParen);

                funcDecl.Arguments = ParseFunctionDeclarationArguments();
                Expect(TokenType.CloseParen);
                Expect(TokenType.OpenBrace);
                while (!Peek(TokenType.CloseBrace))
                {
                    Statement statement = ParseStatement();
                    funcDecl.Body.Statements.Add(statement);
                }

                Expect(TokenType.CloseBrace);

                return funcDecl;

            }
            else if (Accept(TokenType.Return))
            {
                ReturnStatement returnStatement = new ReturnStatement();
                returnStatement.Expression = ParseExpression();
                Expect(TokenType.Semicolon);
                return returnStatement;
            }
            else
            {
                ExpressionStatement expressionStatement = new ExpressionStatement();
                expressionStatement.Expression = ParseExpression();
                Expect(TokenType.Semicolon);
                return expressionStatement;
            }
            return null;
        }

        public ProgramNode ParseProgram()
        {
            ProgramNode program = new ProgramNode();

            while (Index < Tokens.Count)
            {
                if (Accept(TokenType.Var))
                {
                    VarDeclarationStatement varDeclStatement = new VarDeclarationStatement();
                    if (!Peek(TokenType.Identifier))
                        Error("identifier expected");
                    varDeclStatement.Identifier = Read().Contents;

                    if (Peek(TokenType.Equal))
                    {
                        Read();
                        varDeclStatement.InitialValue = ParseExpression();
                    }
                    Expect(TokenType.Semicolon);

                    program.VariableDeclarations.Add(varDeclStatement);
                }

                else if (Accept(TokenType.Function))
                {
                    FunctionDeclarationStatement funcDecl = new FunctionDeclarationStatement();
                    if (!Peek(TokenType.Identifier))
                        throw new Exception("Identifier expected");
                    funcDecl.Name = Read().Contents;

                    Expect(TokenType.OpenParen);

                    funcDecl.Arguments = ParseFunctionDeclarationArguments();
                    Expect(TokenType.CloseParen);
                    Expect(TokenType.OpenBrace);
                    while (!Peek(TokenType.CloseBrace))
                    {
                        Statement statement = ParseStatement();
                        funcDecl.Body.Statements.Add(statement);
                    }

                    Expect(TokenType.CloseBrace);

                    program.FunctionDeclarations.Add(funcDecl);
                }
                else
                {
                    throw new Exception("Unexpected token on line: " + Peek().Line.ToString());
                }
            }
            return program;
        }
    }
}

