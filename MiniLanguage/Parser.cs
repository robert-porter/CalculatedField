using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class Parser
    {
        List<Token> Tokens;
        int Index;

        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
            Index = 0;
        }

        Token Read()
        {
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
            while (!Match(TokenType.CloseParen))
            {
                arguments.Add(ParseExpression());
                if (!Match(TokenType.CloseParen))
                    Expect(TokenType.Comma);
            }

            return arguments;
        }

        Expression ParseFactor()
        {
            Expression factor = null;
            UnaryExpression.Operator unaryOperator = UnaryExpression.Operator.Plus;
            bool isUnary = false;
            if (Match(TokenType.Plus))
            {
                isUnary = true;
                unaryOperator = UnaryExpression.Operator.Plus;
                Read();
            }
            else if (MatchAndRead(TokenType.Minus))
            {
                isUnary = true;
                unaryOperator = UnaryExpression.Operator.Minus; 
            }
            else if (MatchAndRead(TokenType.Bang))
            {
                isUnary = true;
                unaryOperator = UnaryExpression.Operator.Not;
            }

            if (Match(TokenType.Identifier))
            {
                if (Match(TokenType.OpenParen, 1))
                {
                    IdentifierExpression identifier = new IdentifierExpression(Read().Contents);
                    Read();
                    List<Expression> arguments = ParseFunctionCallArguments();
                    Expect(TokenType.CloseParen);
                    factor = new FunctionCallExpression(identifier, arguments);
                }
                else if(Match(TokenType.OpenSquareBracket, 1))
                {
                    String identifier = Read().Contents;
                    Read();
                    Expression indexExpression = ParseExpression();
                    ArrayIndexExpression arrayIndexExpression = new ArrayIndexExpression(identifier, indexExpression);
                    Expect(TokenType.CloseSquareBracket);
                    factor = arrayIndexExpression;
                }
                else 
                    factor = new IdentifierExpression(Read().Contents);
            }
            else if (Match(TokenType.NumberLiteral))
            {
                factor = new FloatLiteralExpression(Read().Contents);
            }
            else if (MatchAndRead(TokenType.OpenParen))
            {
                Expression e = ParseExpression();
                Expect(TokenType.CloseParen);

                factor = e;
            }
            else if(Match(TokenType.True) || Match(TokenType.False))
            {
                Token token = Read();
                factor = new BoolLiteralExpression(token.Contents);
            }
            else if (Match(TokenType.StringLiteral))
            {
                factor = new StringLiteralExpression(Read().Contents);
            }
            else
            {
                Error("factor: syntax error");
            }

            if (isUnary)
            {
                return new UnaryExpression(unaryOperator, factor);
            }
            else
                return factor;
        }

        Expression ParseTerm()
        {
            Expression left, right;
            BinaryExpression.Operator op = BinaryExpression.Operator.Add;

            left = ParseFactor();
            while (Match(TokenType.Star) || Match(TokenType.Slash))
            {
                if (Match(TokenType.Star))
                    op = BinaryExpression.Operator.Multiply;
                else if (Match(TokenType.Slash))
                    op = BinaryExpression.Operator.Divide;

                Read();
                right = ParseFactor();

                if ((Match(TokenType.Star) || Match(TokenType.Slash)))
                {
                    left = new BinaryExpression(op, left, right);
                
                }
                else
                {
                    return new BinaryExpression(op, left, right);
                }
            }

            return left;
        }

        Expression ParseArithmeticExpression()
        {
            BinaryExpression.Operator op = BinaryExpression.Operator.Add;
            Expression left, right;
            left = ParseTerm();

            while (Match(TokenType.Plus) || Match(TokenType.Minus))
            {
                if (Match(TokenType.Plus))
                    op = BinaryExpression.Operator.Add;
                else if (Match(TokenType.Minus))
                    op = BinaryExpression.Operator.Subtract;

                Read();
                right = ParseTerm();

                if (Match(TokenType.Plus) || Match(TokenType.Minus))
                {
                    left = new BinaryExpression(op, left, right);

                }
                else
                {
                    return new BinaryExpression(op, left, right);
                }
            }

            return left;
        }

        // put == and != before this?
        Expression ParseConditionalExpression()
        {

            BinaryExpression.Operator op = BinaryExpression.Operator.Add;
            Expression left = ParseArithmeticExpression();

            if (Match(TokenType.DoubleEqual))
                op = BinaryExpression.Operator.DoubleEqual;
            else if (Match(TokenType.NotEqual))
                op = BinaryExpression.Operator.NotEqual;
            else if (Match(TokenType.Less))
                op = BinaryExpression.Operator.Less;
            else if (Match(TokenType.LessOrEqual))
                op = BinaryExpression.Operator.LessOrEqual;
            else if (Match(TokenType.Greater))
                op = BinaryExpression.Operator.Greater;
            else if (Match(TokenType.GreaterOrEqual))
                op = BinaryExpression.Operator.GreaterOrEqual;
            else
                return left;

            Read(); // read operator

            return new BinaryExpression(op, left, ParseArithmeticExpression());
        }

        Expression ParseAndExpression()
        {
         
            Expression left = ParseConditionalExpression();
            Expression right;

            while (Match(TokenType.And))
            {
                if (Match(TokenType.And))
                {
                    Read();
                    right = ParseConditionalExpression();

                    if (Match(TokenType.And))
                    {
                        left = new BinaryExpression(BinaryExpression.Operator.And, left, right);
                    }
                    else
                    {
                        return new BinaryExpression(BinaryExpression.Operator.Add, left, right);
                    }
                }
            }
            return left;
        }

        //top level operator, parses or expression

        Expression ParseExpression()
        {
            int oldIndex = Index; // need to backtrack if array indexing but not assignment.

            if (Match(TokenType.Identifier) && Match(TokenType.Equal, 1))
            {
                IdentifierExpression identifier = new IdentifierExpression(Read().Contents);
                Expect(TokenType.Equal);
                return new AssignmentExpression(identifier, ParseExpression());
            }
            else if (Match(TokenType.Identifier) && Match(TokenType.OpenSquareBracket, 1))
            {
                String identifier = Read().Contents;
                Read();
                ArrayIndexExpression indexExpression = new ArrayIndexExpression(identifier, ParseExpression());
                Expect(TokenType.CloseSquareBracket);
                if (MatchAndRead(TokenType.Equal))
                {
                    return new AssignmentExpression(indexExpression, ParseExpression());
                }
                else
                {
                    Index = oldIndex;
                }
            }
            
            return ParseOr();
        }

        List<String> ParseFunctionDeclarationArguments()
        {
            if (Match(TokenType.CloseParen))
                return null;

            List<String> arguments = new List<String>();
            while (!Match(TokenType.CloseParen))
            {
                if (Match(TokenType.Identifier))
                    arguments.Add(Read().Contents);
                else Error("Identifier expected");

                if (!Match(TokenType.CloseParen))
                    Expect(TokenType.Comma);
            }

            return arguments;
        }

        Expression ParseOr()
        {
            Expression left = ParseAndExpression();
            Expression right;

            while (Match(TokenType.Or))
            {
                if (Match(TokenType.Or))
                {
                    Read();
                    right = ParseAndExpression();

                    if (Match(TokenType.And))
                    {
                        left = new BinaryExpression(BinaryExpression.Operator.And, left, right);
                    }
                    else
                    {
                        return new BinaryExpression(BinaryExpression.Operator.Add, left, right);
                    }
                }
            }
            return left;
        }

        Statement ParseStatement()
        {
            if (MatchAndRead(TokenType.OpenCurlyBrace))
            {
                List<Statement> statements = new List<Statement>();
                while (!Match(TokenType.CloseCurlyBrace))
                {
                    Statement statement = ParseStatement();
                    statements.Add(statement);
                }

                Expect(TokenType.CloseCurlyBrace);
                return new BlockStatement(statements);
            }
            else if (MatchAndRead(TokenType.If))
            {
                Expect(TokenType.OpenParen);
                Expression condition = ParseExpression();
                Expect(TokenType.CloseParen);
                Statement thenBody = ParseStatement();

                if (Match(TokenType.Else))
                {
                    Read();
                    Statement elseBody = ParseStatement();

                    return new IfStatement(condition, thenBody, elseBody);
                }
                else
                    return new IfStatement(condition, thenBody);
            }
            else if (MatchAndRead(TokenType.While))
            {
                Expect(TokenType.OpenParen);
                Expression condition = ParseExpression();
                Expect(TokenType.CloseParen);
                Statement body = ParseStatement();

                return new WhileStatement(condition, body);
            }
            else if (Match(TokenType.Var))
            {
                return ParseValDeclarationStatment();
            }
            else if(MatchAndRead(TokenType.Ref))
            {
                if(Index + 3 >= Tokens.Count)
                    throw new Exception("unexpected token");
                if(!(Match(TokenType.Identifier) && Match(TokenType.Equal, 1) && Match(TokenType.Identifier)))
                    throw new Exception("unexpected token");
                
                RefDeclarationStatement refDeclarationStatement = new RefDeclarationStatement();
                
                refDeclarationStatement.RefIdentifier = Read().Contents;
                Read();
                refDeclarationStatement.ReferencedVariable = new IdentifierExpression(Read().Contents);
                Expect(TokenType.Semicolon);

                return refDeclarationStatement;
            }
            else if (Match(TokenType.Function))
            {
                return ParseFunctionDeclarationStatement();
            }
            else if (MatchAndRead(TokenType.Return))
            {
                ReturnStatement returnStatement = new ReturnStatement(ParseExpression());
                Expect(TokenType.Semicolon);
                return returnStatement;
            }
            else
            {
                ExpressionStatement expressionStatement = new ExpressionStatement(ParseExpression());
                Expect(TokenType.Semicolon);
                return expressionStatement;
            }
        }

        public FunctionDeclarationStatement ParseFunctionDeclarationStatement()
        {
            if (!Match(TokenType.Function))
                return null;
            Read();

            String name;
            List<String> arguments = new List<string>();
            List<Statement> statements = new List<Statement>();

            if (!Match(TokenType.Identifier))
                throw new Exception("Identifier expected");
            name = Read().Contents;

            Expect(TokenType.OpenParen);

            arguments = ParseFunctionDeclarationArguments();
            Expect(TokenType.CloseParen);
            Expect(TokenType.OpenCurlyBrace);


            while (!Match(TokenType.CloseCurlyBrace))
            {
                Statement statement = ParseStatement();
                statements.Add(statement);
            }

            Expect(TokenType.CloseCurlyBrace);

            return new FunctionDeclarationStatement(name, arguments, new BlockStatement(statements));
        }

        public VarDeclarationStatement ParseValDeclarationStatment()
        {
            if (!MatchAndRead(TokenType.Var))
                return null;

            VarDeclarationStatement varDeclStatement = new VarDeclarationStatement();
            if (!Match(TokenType.Identifier))
                Error("identifier expected");
            varDeclStatement.Identifier = Read().Contents;

            if (MatchAndRead(TokenType.OpenSquareBracket))
            {
                if (!Match(TokenType.NumberLiteral))
                {
                    Error("Array size expected");
                }
                int arraySize = int.Parse(Read().Contents);
                Expect(TokenType.CloseSquareBracket);
                varDeclStatement.IsArray = true;
                varDeclStatement.ArraySize = arraySize;
            }

            if(Match(TokenType.Colon))
            {
                Read();
                VariableType variableType = VariableType.Any;
                if (MatchAndRead(TokenType.Int))
                    variableType = VariableType.Int;
                else if (MatchAndRead(TokenType.Float))
                    variableType = VariableType.Float;
                else if (MatchAndRead(TokenType.String))
                    variableType = VariableType.String;
                else if (MatchAndRead(TokenType.Bool))
                    variableType = VariableType.Bool;

                varDeclStatement.Type = variableType;
            }

            if (Match(TokenType.Equal))
            {
                Read();
                varDeclStatement.InitialValue = ParseExpression();
            }
            Expect(TokenType.Semicolon);

            return varDeclStatement;
        }

        public ProgramNode ParseProgram()
        {
            ProgramNode program = new ProgramNode();

            while (Index < Tokens.Count)
            {
                if (Match(TokenType.Var))
                {
                    program.VariableDeclarations.Add(ParseValDeclarationStatment());
                }

                else if (Match(TokenType.Function))
                {
                    FunctionDeclarationStatement funcDecl = ParseFunctionDeclarationStatement();
                    if(funcDecl != null)
                        program.FunctionDeclarations.Add(funcDecl);
                }
                else
                {
                    throw new Exception("Unexpected token");
                }
            }
            return program;
        }
    }
}

