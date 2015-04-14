using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class SExpressionPrinter : Visitor
    {
        int IndentLevel = 0;
        int IdentWidth = 3;
        bool EmptyLine = false; // so we don't double(or more) newline and indent.

        void WriteIndentation()
        {

            for (int i = 0; i < IndentLevel; i++)
            {
                for (int j = 0; j < IdentWidth; j++)
                {
                    Console.Write(" ");
                }
            }
        }
        void Write(String s)
        {
            EmptyLine = false;
            Console.Write(s);
        }

        void WriteLine()
        {
            if(!EmptyLine)
            {
                Console.WriteLine();
                WriteIndentation();
                EmptyLine = true; 
            }
        }

        public override void Visit(ProgramNode program)
        {
            foreach (VarDeclarationStatement varDecl in program.VariableDeclarations)
            {
                varDecl.Accept(this);
            }

            foreach (FunctionDeclarationStatement funcDecl in program.FunctionDeclarations)
            {
                funcDecl.Accept(this);
            }
        }
        public override void Visit(ExpressionStatement expressionStatement)
        {
            expressionStatement.Accept(this);
        }

        public override void Visit(IdentifierExpression identifier)
        {
            Write(identifier.Name);
        }

        public override void Visit(NumberExpression number)
        {
            Write(number.Value);
        }

        public override void Visit(BinaryExpression binaryExpression)
        {
            String strOp = "";
            if (binaryExpression.Op == BinaryExpression.Operator.Add)
                strOp = "+";
            else if (binaryExpression.Op == BinaryExpression.Operator.Subtract)
                strOp = "-";
            else if (binaryExpression.Op == BinaryExpression.Operator.Multiply)
                strOp = "*";
            else if (binaryExpression.Op == BinaryExpression.Operator.Divide)
                strOp = "/";
            else if (binaryExpression.Op == BinaryExpression.Operator.DoubleEqual)
                strOp = "==";
            else if (binaryExpression.Op == BinaryExpression.Operator.NotEqual)
                strOp = "!=";
            else if (binaryExpression.Op == BinaryExpression.Operator.Greater)
                strOp = ">";
            else if (binaryExpression.Op == BinaryExpression.Operator.GreaterOrEqual)
                strOp = ">=";
            else if (binaryExpression.Op == BinaryExpression.Operator.Less)
                strOp = "<";
            else if (binaryExpression.Op == BinaryExpression.Operator.LessOrEqual)
                strOp = "<=";
            else if (binaryExpression.Op == BinaryExpression.Operator.And)
                strOp = "&&";
            else if (binaryExpression.Op == BinaryExpression.Operator.Or)
                strOp = "||";

            Write("(");
            Write(strOp);
            Write(" ");
            binaryExpression.Left.Accept(this);
            Write(" ");
            binaryExpression.Right.Accept(this);
            Write(")");
        }

        public override void Visit(UnaryExpression unaryExpression)
        {
            Write("(");
            if (unaryExpression.Op == UnaryExpression.Operator.Plus)
                Write("+");
            else if (unaryExpression.Op == UnaryExpression.Operator.Minus)
                Write("-");
            else if (unaryExpression.Op == UnaryExpression.Operator.Not)
                Write("!");

            unaryExpression.Argument.Accept(this);
            Write(")");
        }


        public override void Visit(AssignmentStatement assignmentStatement)
        {
            Write("(set! ");
            assignmentStatement.Left.Accept(this);
            Write(" ");
            assignmentStatement.Right.Accept(this);
            Write(")");
        }

        public override void Visit(VarDeclarationStatement varDeclStatement)
        {
            Write("(var ");
            Write(varDeclStatement.Identifier);
            if (varDeclStatement.InitialValue != null)
            {
                Write(" ");
                varDeclStatement.InitialValue.Accept(this);
            }
            Write(")");
        }

        public override void Visit(BlockStatement blockStatement)
        {
            WriteLine();
            Write("(begin");
            IndentLevel++;
            foreach (Statement statement in blockStatement.Statements)
            {
                WriteLine();
                statement.Accept(this);
            }
            IndentLevel--;
            Write(")");
        }


        public override void Visit(IfStatement ifStatement)
        {
            WriteLine();
            Write("(if ");
            IndentLevel++;
            ifStatement.Condition.Accept(this);
            WriteLine();
            ifStatement.Consequent.Accept(this);

            if (ifStatement.Alternate != null)
            {
                WriteLine();
                ifStatement.Alternate.Accept(this);
            }
            IndentLevel--;
            Write(")");
        }

        public override void Visit(WhileStatement whileStatement)
        {
            WriteLine();
            Write("(while ");
            whileStatement.Condition.Accept(this);
            WriteLine();
            IndentLevel++;
            whileStatement.Body.Accept(this);
            IndentLevel--;
            Write(")");
        }

        public override void Visit(FunctionCallExpression funcCallExpression)
        {
            Write("(");
            Write(funcCallExpression.Identifier.Name);

            if (funcCallExpression.Arguments != null)
            {
                Write(" (");
                for (int i = 0; i < funcCallExpression.Arguments.Count - 1; i++)
                {
                    funcCallExpression.Arguments[i].Accept(this);
                    Write(" ");
                }

                funcCallExpression.Arguments[funcCallExpression.Arguments.Count - 1].Accept(this);
                Write(")");
            }
            Write(")");
        }

        public override void Visit(FunctionDeclarationStatement funcDeclStatement)
        {
            Write("(");
            Write(funcDeclStatement.Name);
            if (funcDeclStatement.Arguments != null)
            {
                Write(" (");
                for (int i = 0; i < funcDeclStatement.Arguments.Count - 1; i++)
                {
                    funcDeclStatement.Arguments[i].Accept(this);
                    Write(" ");
                }

                funcDeclStatement.Arguments[funcDeclStatement.Arguments.Count - 1].Accept(this);
                Write(")");
            }
            Write(")");
            IndentLevel++;
            funcDeclStatement.Body.Accept(this);
            IndentLevel--;
        }

        public override void Visit(ReturnStatement returnStatement)
        {
            Write("(return ");
            returnStatement.Expression.Accept(this);
            Write(")");
        }
    }
}
