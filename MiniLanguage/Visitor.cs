using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class Visitor
    {
        public virtual void Visit(Identifier identifier)
        {

        }

        public virtual void Visit(Number number)
        {

        }

        public virtual void Visit(BinaryExpression binaryExpression)
        {
        }

        public virtual void Visit(UnaryExpression unaryExpression)
        {

        }


        public virtual void Visit(AssignmentStatement assignmentStatement)
        {

        }

        public virtual void Visit(VarDeclarationStatement varDeclStatement)
        {
        }

        public virtual void Visit(BlockStatement blockStatement)
        {

        }


        public virtual void Visit(IfStatement ifStatement)
        {

        }

        public virtual void Visit(WhileStatement whileStatement)
        {

        }

        public virtual void Visit(FunctionCallExpression funcCallExpression)
        {

        }

        public virtual void Visit(FunctionDeclarationExpression funcDeclStatement)
        {

        }

        public virtual void Visit(ExpressionStatement expessionStatement)
        {

        }
    }
}
