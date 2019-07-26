using System;
using System.Collections.Generic;

namespace CalculatedField
{
    public enum UnaryOperator
    {
        Plus,
        Minus,
        Not
    }

    public enum BinaryOperator
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        DivideAndTruncate,
        CompareEqual,
        CompareNotEqual,
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual,
        And,
        Or
    }

    abstract class Expression
    {
        public readonly Token Token;
        public Expression(Token token)
        {
            Token = token;
        }
    }

    class BlockExpression : Expression
    {
        public readonly List<Expression> Expressions;

        public BlockExpression(List<Expression> expressions, Token token) : base(token)
        {
            Expressions = expressions;
        }
    }

    class IdentifierExpression : Expression
    {
        public readonly string Name;
        public int Location { get; set; }

        public IdentifierExpression(string name, Token token) : base(token)
        {
            Name = name;
        }
    }

    class FieldExpression : Expression
    {
        public readonly string Name;
        public int Location { get; set; }

        public FieldExpression(string name, Token token) : base(token)
        {
            Name = name;
        }
    }

    class LiteralExpression : Expression
    {
        public readonly ScriptType Type;
        public readonly string Value; 
        public int Location { get; set; }

        public LiteralExpression(string value, ScriptType type, Token token) : base(token)
        {
            Value = value;
            Type = type;
        }

        public ScriptValue ScriptValue => ScriptValue.Parse(Value, Type); 
    }

    class BinaryExpression : Expression
    {
        public readonly BinaryOperator Operator;
        public readonly Expression Left;
        public readonly Expression Right;

        public BinaryExpression(BinaryOperator op, Expression left, Expression right, Token token) : base(token)
        {
            Operator = op;
            Left = left;
            Right = right;
        }
    }

    class UnaryExpression : Expression
    {
        public readonly Expression Right;
        public readonly UnaryOperator Operator;

        public UnaryExpression(UnaryOperator op, Expression argument, Token token) : base(token)
        {
            Operator = op;
            Right = argument;
        }
    }

    class FunctionExpression : Expression 
    {
        public readonly string Name;
        public readonly List<Expression> Arguments;
        public int Location { get; set; }

        public FunctionExpression(string name, List<Expression> arguments, Token token) : base(token)
        {
            Name = name;
            Arguments = arguments;
        }
    }

    class AssignmentExpression : Expression
    {
        public readonly string Name;
        public readonly Expression Right;
        public int Location { get; set; }

        public AssignmentExpression(string name, Expression right, Token token) : base(token)
        {
            Name = name;
            Right = right;
        }
    }

    class IfExpression : Expression
    {
        public readonly Expression Condition;
        public readonly BlockExpression ThenExpression;
        public readonly BlockExpression ElseExpression;

        public IfExpression(Expression condition, BlockExpression thenBody, BlockExpression elseBody, Token token) : base(token)
        {
            Condition = condition;
            ThenExpression = thenBody;
            ElseExpression = elseBody;
        }

        public IfExpression(Expression condition, BlockExpression thenBody, Token ifToken, Token endToken) : base(ifToken)
        {
            Condition = condition;
            ThenExpression = thenBody;
            ElseExpression = new BlockExpression(new List<Expression>() { new LiteralExpression("null", ScriptType.Null, endToken) }, endToken);
        }
    }
}
