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
    }

    class BlockExpression : Expression
    {
        public readonly List<Expression> Expressions;

        public BlockExpression(List<Expression> expressions)
        {
            Expressions = expressions;
        }
    }

    class IdentifierExpression : Expression
    {
        public readonly string Name;
        public int Location { get; set; }

        public IdentifierExpression(string name)
        {
            Name = name;
        }
    }

    class FieldExpression : Expression
    {
        public readonly string Name;
        public int Location { get; set; }

        public FieldExpression(string name)
        {
            Name = name;
        }
    }

    class LiteralExpression : Expression
    {
        public readonly ScriptType Type;
        public readonly string Value; 
        public int Location { get; set; }

        public LiteralExpression(string value, ScriptType type)
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

        public BinaryExpression(BinaryOperator op, Expression left, Expression right)
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

        public UnaryExpression(UnaryOperator op, Expression argument)
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

        public FunctionExpression(string name, List<Expression> arguments)
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

        public AssignmentExpression(string name, Expression right)
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

        public IfExpression(Expression condition, BlockExpression thenBody, BlockExpression elseBody)
        {
            Condition = condition;
            ThenExpression = thenBody;
            ElseExpression = elseBody;
        }

        public IfExpression(Expression condition, BlockExpression thenBody)
        {
            Condition = condition;
            ThenExpression = thenBody;
            ElseExpression = new BlockExpression(new List<Expression>() { new LiteralExpression("null", ScriptType.Null) });
        }
    }
}
