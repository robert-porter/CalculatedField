using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    abstract class ASTNode
    {
        public abstract void Accept(Visitor visitor);
    }

    class ProgramNode : ASTNode
    {

        public List<VarDeclarationStatement> VariableDeclarations;
        public List<FunctionDeclarationStatement> FunctionDeclarations;

        public ProgramNode()
        {
            VariableDeclarations = new List<VarDeclarationStatement>();
            FunctionDeclarations = new List<FunctionDeclarationStatement>();
        }

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        } 
    }

    abstract class Expression : ASTNode
    {

    }

    class IdentifierExpression : Expression
    {
        public IdentifierExpression(String name)
        {
            Name = name;
        }
        public String Name { get; set; }

        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }
    }

    class ArrayIndexExpression : IdentifierExpression
    {
        public Expression IndexExpression;
        public ArrayIndexExpression(String identifier, Expression indexExpression) : base(identifier)
        {
            IndexExpression = indexExpression;
        }

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
    class NumberExpression : Expression
    {
        public NumberExpression(String value)
        {
            Value = value;
        }

        public String Value { get; set; }
        
        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }
    }

    class BoolExpression : Expression
    {
        public bool Value;

        public BoolExpression(bool b)
        {
            Value = b;
        }

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class BinaryExpression : Expression
    {
        public BinaryExpression()
        {
        }

        public Expression Left { get; set; }
        public Expression Right { get; set; }
        public Operator Op { get; set; }
        public enum Operator
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            DoubleEqual,
            NotEqual,
            Greater,
            GreaterOrEqual,
            Less,
            LessOrEqual,
            And,
            Or
        }


        
        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }
    }

    class UnaryExpression : Expression
    {
        public UnaryExpression(Operator op)
        {
            Op = op;
        }
        public Expression Argument { get; set; }
        public Operator Op { get; set; }

        public enum Operator
        {
            Plus,
            Minus,
            Not
        }


        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }
    }

    class FunctionCallExpression : Expression 
    {
        public IdentifierExpression Identifier { get; set; }
        public List<Expression> Arguments { get; set; }

        public FunctionCallExpression()
        {
        }

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }

    abstract class Statement : ASTNode
    {

    }

    class ExpressionStatement : Statement
    {
        public Expression Expression;

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class ReturnStatement : Statement
    {
        public Expression Expression;

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }


    class AssignmentExpression : Expression
    {
        public IdentifierExpression Left { get; set; }
        public Expression Right { get; set; }

        
        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }

    }

    class VarDeclarationStatement : Statement
    {
        public String Identifier { get; set; }
        public Expression InitialValue { get; set; }
        public bool IsArray;
        public int ArraySize;

        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }
    }

    class FunctionDeclarationStatement : Statement
    {
        public String Name;
        public List<IdentifierExpression> Arguments { get; set; }
        public BlockStatement Body { get; set; }

        public FunctionDeclarationStatement()
        {
            Body = new BlockStatement();
        }

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }

    }


    class BlockStatement : Statement
    {
        public BlockStatement()
        {
            Statements = new List<Statement>();
        }
        
        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }

        public List<Statement> Statements { get; set; }
    }

    class IfStatement : Statement
    {
        public Expression Condition { get; set; }
        public Statement Consequent { get; set; }
        public Statement Alternate { get; set; }


        
        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }
    }

    class WhileStatement : Statement
    {
        public Expression Condition { get; set; }
        public Statement Body { get; set; }

        
        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }

    }
}
