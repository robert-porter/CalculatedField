using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class ScopeChecker : Visitor
    {
        List<HashSet<String>> Identifiers; // acts as a stack of scopes

        // used to pass the argumnents to the function body.
        List<String> funcDeclArguments; 


        public ScopeChecker()
        {
            Identifiers = new List<HashSet<string>>();
            funcDeclArguments = new List<string>();
        }

        // checks that the variable was declared
        bool CheckDeclared(String identifier)
        {
            for (int i = Identifiers.Count - 1; i >= 0; i--)
            {
                if (Identifiers[i].Contains(identifier))
                    return true;
            }

            return false;
        }

        // checks that the variable is not double declared in the same scope.
        bool ExistInCurrentScope(String identifier)
        {
            return Identifiers[Identifiers.Count - 1].Contains(identifier);
        }

        void AddIdentifier(String identifier)
        {
            Identifiers[Identifiers.Count - 1].Add(identifier);
        }

        void PushScope()
        {
            Identifiers.Add(new HashSet<string>());
        }

        void PopScope()
        {
            Identifiers.RemoveAt(Identifiers.Count - 1);
        }

        public override void Visit(ProgramNode program)
        {
            // order does not matter at global scope.
            // add everything global before doing any checks.
            Identifiers.Add(new HashSet<string>());

            foreach (FunctionDeclarationStatement funcDecl in program.FunctionDeclarations)
            {
                if (ExistInCurrentScope(funcDecl.Name))
                    throw new Exception("function already declared");
                AddIdentifier(funcDecl.Name);
            }
            foreach (VarDeclarationStatement varDecl in program.VariableDeclarations)
            {
                if (ExistInCurrentScope(varDecl.Identifier))
                    throw new Exception("variable already declared");
                AddIdentifier(varDecl.Identifier);
            }

            foreach (FunctionDeclarationStatement funcDecl in program.FunctionDeclarations)
            {
                funcDecl.Accept(this);
            }
            foreach (VarDeclarationStatement varDecl in program.VariableDeclarations)
            {
                varDecl.Accept(this);
            }

 
        }
        public override void Visit(IdentifierExpression identifier)
        {
            if (!CheckDeclared(identifier.Name))
                throw new Exception("undeclared identifier");

        }

        public override void Visit(ArrayIndexExpression arrayIndexExpression)
        {
            if (!CheckDeclared(arrayIndexExpression.Name))
                throw new Exception("undeclared identifier");
            arrayIndexExpression.IndexExpression.Accept(this);

        }
        public override void Visit(NumberExpression number) 
        { 
            // always a leaf
        }
        public override void Visit(BoolExpression expression)
        {
            // always a leaf
        }

        public override void Visit(StringExpression expression)
        {
            // always a leaf
        }
        public override void Visit(BinaryExpression binaryExpression) 
        {
            binaryExpression.Left.Accept(this);
            binaryExpression.Right.Accept(this);
        }
        public override void Visit(UnaryExpression unaryExpression) 
        {
            unaryExpression.Argument.Accept(this);
        }
        public override void Visit(AssignmentExpression assignmentStatement) 
        {
            if (!CheckDeclared(assignmentStatement.Left.Name))
                throw new Exception("undeclared identifier");
            assignmentStatement.Right.Accept(this);

        }
        public override void Visit(VarDeclarationStatement varDeclStatement) 
        {
            // declarations are already checked at global scope(Identifiers.Count == 1)
            if (Identifiers.Count != 1 && ExistInCurrentScope(varDeclStatement.Identifier))
                throw new Exception("variable already declared");

            AddIdentifier(varDeclStatement.Identifier);
            if(varDeclStatement.InitialValue != null)
                varDeclStatement.InitialValue.Accept(this);
        }

        public override void Visit(RefDeclarationStatement refDeclStatement)
        {
            // declarations are already checked at global scope(Identifiers.Count == 1)
            if (Identifiers.Count != 1 && ExistInCurrentScope(refDeclStatement.RefIdentifier))
                throw new Exception("variable already declared");

            AddIdentifier(refDeclStatement.RefIdentifier);

            refDeclStatement.ReferencedVariable.Accept(this);
        }

        public override void Visit(BlockStatement blockStatement) 
        {
            PushScope();
            
            if(funcDeclArguments.Count != 0)
            {
                foreach (String identifier in funcDeclArguments)
                {

                    if (ExistInCurrentScope(identifier))
                        throw new Exception("Variable redifined in same scope.");
                    AddIdentifier(identifier);
                }
                funcDeclArguments.Clear();
            }
            
            foreach(Statement statement in blockStatement.Statements)
            {
                statement.Accept(this);
            }
            PopScope();

        }
        public override void Visit(IfStatement ifStatement) 
        {
            ifStatement.Condition.Accept(this);
            ifStatement.Consequent.Accept(this);
            if(ifStatement.Alternate != null)
                ifStatement.Alternate.Accept(this);
        }
        public override void Visit(WhileStatement whileStatement) 
        {
            whileStatement.Condition.Accept(this);
            whileStatement.Body.Accept(this);
        }
        public override void Visit(FunctionCallExpression funcCallExpression) 
        {
            funcCallExpression.Identifier.Accept(this);
            foreach(Expression argument in funcCallExpression.Arguments)
            {
                argument.Accept(this);
            }

        }
        public override void Visit(FunctionDeclarationStatement funcDeclStatement)
        {
            // declarations are already checked at global scope(Identifiers.Count == 1)
            if (Identifiers.Count != 1 && ExistInCurrentScope(funcDeclStatement.Name))
                throw new Exception("variable already declared");

            AddIdentifier(funcDeclStatement.Name);

            if (funcDeclStatement.Arguments != null)
            {
                foreach (IdentifierExpression idExpr in funcDeclStatement.Arguments)
                {
                    funcDeclArguments.Add(idExpr.Name);
                }
            }

            funcDeclStatement.Body.Accept(this);
        }
        public override void Visit(ExpressionStatement expressionStatement) 
        {
            expressionStatement.Expression.Accept(this); 
        }
        public override void Visit(ReturnStatement returnStatement) 
        {
            returnStatement.Expression.Accept(this);
        }

    }
}
