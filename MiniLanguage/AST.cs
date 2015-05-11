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
        public readonly String Name;

        public IdentifierExpression(String name)
        {
            Name = name;
        }

        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }
    }

    class ArrayIndexExpression : IdentifierExpression
    {
        public readonly Expression IndexExpression;

        public ArrayIndexExpression(String identifier, Expression indexExpression) : base(identifier)
        {
            IndexExpression = indexExpression;
        }

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class FloatLiteralExpression : Expression
    {
        public readonly String StringValue;
        public float FloatValue { get { return float.Parse(StringValue); } }

        public FloatLiteralExpression(String value)
        {
            StringValue = value;
        }
        
        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }
    }

    class BoolLiteralExpression : Expression
    {
        public readonly String StringValue;
        public bool BoolValue { get { return bool.Parse(StringValue); } }

        public BoolLiteralExpression(String value)
        {
            StringValue = value;
        }

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class StringLiteralExpression : Expression
    {
        public readonly String Value;

        public StringLiteralExpression(String s)
        {
            Value = s;
        }

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class BinaryExpression : Expression
    {
        public readonly Operator Op;
        public readonly Expression Left;
        public readonly Expression Right;

        public BinaryExpression(Operator op, Expression left, Expression right)
        {
            Op = op;
            Left = left;
            Right = right;
        }

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
        public readonly Expression Argument;
        public readonly Operator Op;

        public UnaryExpression(Operator op, Expression argument)
        {
            Op = op;
            Argument = argument;
        }

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
        public readonly IdentifierExpression Identifier;
        public readonly List<Expression> Arguments;

        public FunctionCallExpression(IdentifierExpression identifier, List<Expression> arguments)
        {
            Identifier = identifier;
            Arguments = arguments;
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
        public readonly Expression Expression;

        public ExpressionStatement(Expression expression)
        {
            Expression = expression;
        }

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }

    class ReturnStatement : Statement
    {
        public readonly Expression Expression;

        public ReturnStatement(Expression expression)
        {
            Expression = expression;
        }

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }


    class AssignmentExpression : Expression
    {
        public readonly IdentifierExpression Left;
        public readonly Expression Right; 

        public AssignmentExpression(IdentifierExpression left, Expression right)
        {
            Left = left;
            Right = right;
        }
        
        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }

    }

    enum VariableType
    {
        Any, 
        Int,
        Float,
        String,
        Bool,
    }
    class TypeAnnotation : ASTNode
    {
        public readonly VariableType VariableType;
        public readonly bool IsArray;
        public readonly bool IsRef;

        public TypeAnnotation(VariableType variableType, bool isArray, bool isRef)
        {
            VariableType = variableType;
            IsArray = isArray;
            IsRef = isRef;
        }

        public override void Accept(Visitor visitor)
        {
            
        }
    }

    class VarDeclarationStatement : Statement
    {
        public readonly String Identifier;
        public readonly TypeAnnotation TypeAnnotation;
        public readonly Expression InitialValue;


        public VarDeclarationStatement(String identifier, TypeAnnotation typeAnnotation, Expression initialValue)
        {
            Identifier = identifier;
            TypeAnnotation = typeAnnotation;
            InitialValue = initialValue;
        }
        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }
    }

    class RefDeclarationStatement : Statement
    {
        public String RefIdentifier;
        public IdentifierExpression ReferencedVariable;

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);   
        }
    }

    class FunctionDeclarationStatement : Statement
    {
        public readonly String Name;
        public readonly List<VarDeclarationStatement> Arguments;
        public readonly BlockStatement Body;

        public FunctionDeclarationStatement(String name, List<VarDeclarationStatement> arguments, BlockStatement body)
        {
            Name = name;
            Arguments = arguments;
            Body = body;
        }

        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }

    }


    class BlockStatement : Statement
    {
        public readonly List<Statement> Statements;

        public BlockStatement(List<Statement> statements)
        {
            Statements = statements;
        }
        
        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }

    }

    class IfStatement : Statement
    {
        public readonly Expression Condition;
        public readonly Statement ThenBody;
        public readonly Statement ElseBody;

        public IfStatement(Expression condition, Statement thenBody, Statement elseBody)
        {
            Condition = condition;
            ThenBody = thenBody;
            ElseBody = elseBody;
        }

        public IfStatement(Expression condition, Statement thenBody)
        {
            Condition = condition;
            ThenBody = thenBody;
            ElseBody = null;
        }
        
        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }
    }

    class WhileStatement : Statement
    {
        public readonly Expression Condition;
        public readonly Statement Body;

        public WhileStatement(Expression condition, Statement body)
        {
            Condition = condition;
            Body = body;
        }

        public override void Accept(Visitor visitor)
        {
 	        visitor.Visit(this);
        }

    }
}
