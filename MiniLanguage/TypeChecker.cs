using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class TypeChecker : Visitor
    {

        List<Dictionary<String, TypeAnnotation>> Identifiers; // acts as a stack of scopes

        // used to pass the argumnents to the function body.
        List<VarDeclarationStatement> funcDeclArguments;
        TypeAnnotation ReturnValue; // holds the return value of the Visit function. 


        public TypeChecker()
        {
            Identifiers = new List<Dictionary<String, TypeAnnotation>>();
            funcDeclArguments = new List<VarDeclarationStatement>();
        }

        // checks that the variable was declared
        bool CheckDeclared(String identifier)
        {
            // start from topmost scope
            for (int i = Identifiers.Count - 1; i >= 0; i--)
            {
                if (Identifiers[i].ContainsKey(identifier))
                    return true;
            }

            return false;
        }

        // gets the TypeAnnotation that a variable was declared with.
        TypeAnnotation GetTypeAnnotation(String identifier)
        {
            // start from topmost scope
            for (int i = Identifiers.Count - 1; i >= 0; i--)
            {
                if (Identifiers[i].ContainsKey(identifier))
                    return Identifiers[i][identifier];
            }

            return null;
        }

        void AddIdentifier(String identifier, TypeAnnotation typeAnnotation)
        {
            Identifiers[Identifiers.Count - 1].Add(identifier, typeAnnotation);
        }


        bool CheckTypes(TypeAnnotation A, TypeAnnotation B)
        {
            if (A.VariableType == VariableType.Any || B.VariableType == VariableType.Any)
                return true;
            if (A.VariableType == B.VariableType && A.ArrayDimensions == B.ArrayDimensions)
                return true;
            return false;
        }

        // checks that the variable is not double declared in the same scope.
        bool ExistInCurrentScope(String identifier)
        {
            return Identifiers[Identifiers.Count - 1].ContainsKey(identifier);
        }

        void PushScope()
        {
            Identifiers.Add(new Dictionary<String, TypeAnnotation>());
        }

        void PopScope()
        {
            Identifiers.RemoveAt(Identifiers.Count - 1);
        }

        public override void Visit(ProgramNode program)
        {
            // order does not matter at global scope.
            // add everything global before doing any checks.
            PushScope();

            foreach (FunctionDeclarationStatement funcDecl in program.FunctionDeclarations)
            {
                if (ExistInCurrentScope(funcDecl.Name))
                    throw new Exception("function already declared");
                AddIdentifier(funcDecl.Name, null);
            }
            foreach (VarDeclarationStatement varDecl in program.VariableDeclarations)
            {
                if (ExistInCurrentScope(varDecl.Identifier))
                    throw new Exception("variable already declared");
                AddIdentifier(varDecl.Identifier, varDecl.TypeAnnotation);
            }

            foreach (FunctionDeclarationStatement funcDecl in program.FunctionDeclarations)
            {
                funcDecl.Accept(this);
            }
            foreach (VarDeclarationStatement varDecl in program.VariableDeclarations)
            {
                varDecl.Accept(this);
            }

            PopScope(); 
        }
        public override void Visit(IdentifierExpression identifier)
        {
            if (!CheckDeclared(identifier.Name))
                throw new Exception("undeclared identifier");

            ReturnValue = GetTypeAnnotation(identifier.Name);
        }

        

        public override void Visit(ArrayIndexExpression arrayIndexExpression)
        {
            if (!CheckDeclared(arrayIndexExpression.Name))
                throw new Exception("undeclared identifier");
            arrayIndexExpression.IndexExpression.Accept(this);

            TypeAnnotation typeAnnotation = GetTypeAnnotation(arrayIndexExpression.Name);
            // just subract 1 from the dimension of the array
            ReturnValue = new TypeAnnotation(typeAnnotation.VariableType, typeAnnotation.ArrayDimensions - 1, false);
        }
        public override void Visit(FloatLiteralExpression number) 
        { 
            ReturnValue = new TypeAnnotation(VariableType.Float, 0, false);
        }
        public override void Visit(BoolLiteralExpression expression)
        {
            ReturnValue = new TypeAnnotation(VariableType.Bool, 0, false);
        }

        public override void Visit(StringLiteralExpression expression)
        {
            ReturnValue = new TypeAnnotation(VariableType.String, 0, false);
        }
        public override void Visit(BinaryExpression binaryExpression) 
        {
            TypeAnnotation leftTypeAnnotation, rightTypeAnnotation;

            binaryExpression.Left.Accept(this);
            leftTypeAnnotation = ReturnValue;
            binaryExpression.Right.Accept(this);
            rightTypeAnnotation = ReturnValue;

            if (!CheckTypes(leftTypeAnnotation, rightTypeAnnotation))
            {
                throw new Exception("type error");
            }
        }
        public override void Visit(UnaryExpression unaryExpression) 
        {
            TypeAnnotation argumentTypeAnnotation;
            unaryExpression.Argument.Accept(this);
            argumentTypeAnnotation = ReturnValue;
            if (unaryExpression.Op == UnaryExpression.Operator.Not && (argumentTypeAnnotation.VariableType != VariableType.Bool || argumentTypeAnnotation.ArrayDimensions > 0))
            {
                throw new Exception("type error");
            }
        }
        public override void Visit(AssignmentExpression assignmentStatement) 
        {
            TypeAnnotation rightTypeAnnotation, leftTypeAnnotation;
            if (!CheckDeclared(assignmentStatement.Left.Name))
                throw new Exception("undeclared identifier");
            assignmentStatement.Right.Accept(this);
            rightTypeAnnotation = ReturnValue;
            leftTypeAnnotation = GetTypeAnnotation(assignmentStatement.Left.Name);
            if(!(CheckTypes(leftTypeAnnotation, rightTypeAnnotation)))
            {
                throw new Exception("type error");
            }
        }
        public override void Visit(VarDeclarationStatement varDeclStatement) 
        {
            // declarations are already checked at global scope(Identifiers.Count == 1)
            if (Identifiers.Count > 1 && ExistInCurrentScope(varDeclStatement.Identifier))
                throw new Exception("variable already declared");

            if(Identifiers.Count > 1) 
                AddIdentifier(varDeclStatement.Identifier, varDeclStatement.TypeAnnotation);
            
            if(varDeclStatement.InitialValue != null)
                varDeclStatement.InitialValue.Accept(this);
        }

        public override void Visit(RefDeclarationStatement refDeclStatement)
        {

        }

        public override void Visit(BlockStatement blockStatement) 
        {
            PushScope();
            
            if(funcDeclArguments.Count != 0)
            {
                foreach (VarDeclarationStatement varDecl in funcDeclArguments)
                {

                    if (ExistInCurrentScope(varDecl.Identifier))
                        throw new Exception("Variable redifined in same scope.");
                    AddIdentifier(varDecl.Identifier, varDecl.TypeAnnotation);
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
            ifStatement.ThenBody.Accept(this);
            if(ifStatement.ElseBody != null)
                ifStatement.ElseBody.Accept(this);
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

            if(Identifiers.Count > 1)
                AddIdentifier(funcDeclStatement.Name, null);

            if (funcDeclStatement.Arguments != null)
            {
                for (int i = 0; i < funcDeclStatement.Arguments.Count; i++ )
                {
                    funcDeclArguments.Add(funcDeclStatement.Arguments[i]);
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
