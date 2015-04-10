using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    abstract class Visitor
    {
        public abstract void Visit(IdentifierExpression identifier);
        public abstract void Visit(NumberExpression number);
        public abstract void Visit(BinaryExpression binaryExpression);
        public abstract void Visit(UnaryExpression unaryExpression);
        public abstract void Visit(AssignmentStatement assignmentStatement);
        public abstract void Visit(VarDeclarationStatement varDeclStatement);
        public abstract void Visit(BlockStatement blockStatement);
        public abstract void Visit(IfStatement ifStatement);
        public abstract void Visit(WhileStatement whileStatement);
        public abstract void Visit(FunctionCallExpression funcCallExpression);
        public abstract void Visit(FunctionDeclarationStatement funcDeclStatement);
        public abstract void Visit(ExpressionStatement expessionStatement);
        public abstract void Visit(ReturnStatement returnStatement);
    }
}
