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

    class DecimalLiteralExpression : Expression
    {
        public readonly string StringValue;
        public readonly decimal DecimalValue;

        public DecimalLiteralExpression(string value)
        {
            StringValue = value;
            DecimalValue = decimal.Parse(StringValue);
        }
        
    }

    class BoolLiteralExpression : Expression
    {
        public readonly string StringValue;
        public readonly bool BoolValue;

        public BoolLiteralExpression(string value)
        {
            StringValue = value;
            BoolValue = bool.Parse(StringValue); 
        }

    }

    class StringLiteralExpression : Expression
    {
        public readonly string Value;
        public StringLiteralExpression(string value)
        {
            Value = value;
        }
    }

    class IntegerLiteralExpression : Expression
    {
        public readonly string Value;
        public readonly long IntegerValue;
        public IntegerLiteralExpression(string value)
        {
            Value = value;
            IntegerValue = int.Parse(value);
        }
    }

    class DateTimeLiteralExpression : Expression
    {
        public readonly string Value;
        public readonly DateTime DateTimeValue;
        public DateTimeLiteralExpression(string value)
        {
            Value = value;
            DateTimeValue = DateTime.Parse(value);
        }
    }

    class NullLiteralExpression : Expression
    {
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

    class FunctionCallExpression : Expression 
    {
        public readonly string Name;
        public readonly List<Expression> Arguments;
        public int Location { get; set; }

        public FunctionCallExpression(string name, List<Expression> arguments)
        {
            Name = name;
            Arguments = arguments;
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
    }

    class IfExpression : Expression
    {
        public readonly Expression Condition;
        public readonly Expression ThenExpression;
        public readonly Expression ElseExpression;

        public IfExpression(Expression condition, Expression thenBody, Expression elseBody)
        {
            Condition = condition;
            ThenExpression = thenBody;
            ElseExpression = elseBody;
        }

        public IfExpression(Expression condition, Expression thenBody)
        {
            Condition = condition;
            ThenExpression = thenBody;
            ElseExpression = null;
        }
    }
}
