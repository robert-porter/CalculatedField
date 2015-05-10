using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    abstract class Visitor
    {
        public virtual void Visit(IdentifierExpression identifier) { }
        public virtual void Visit(FloatLiteralExpression number) { }
        public abstract void Visit(BoolLiteralExpression expression);
        public virtual void Visit(BinaryExpression binaryExpression) { }
        public virtual void Visit(UnaryExpression unaryExpression) { }
        public virtual void Visit(AssignmentExpression assignmentStatement) { }
        public virtual void Visit(VarDeclarationStatement varDeclStatement) { }
        public virtual void Visit(BlockStatement blockStatement) { }
        public virtual void Visit(IfStatement ifStatement) { }
        public virtual void Visit(WhileStatement whileStatement) { }
        public virtual void Visit(FunctionCallExpression funcCallExpression) { }
        public virtual void Visit(FunctionDeclarationStatement funcDeclStatement) { }
        public virtual void Visit(ExpressionStatement expressionStatement) { }
        public virtual void Visit(ReturnStatement returnStatement) { }
        public abstract void Visit(ProgramNode program);
        public abstract void Visit(StringLiteralExpression stringExpression);
        public abstract void Visit(RefDeclarationStatement refDeclarationStatement);
        public virtual void Visit(ArrayIndexExpression arrayIndexExpression) { }
    }
}
