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

    abstract class Syntax
    {
        public readonly Token Token;
        public Syntax(Token token)
        {
            Token = token;
        }
    }

    class ScriptExpression : Syntax
    {
        public readonly List<Syntax> Expressions;

        public ScriptExpression(List<Syntax> expressions, Token token) : base(token)
        {
            Expressions = expressions;
        }
    }

    class AssignmentStatement : Syntax
    {
        public readonly string Name;
        public readonly Syntax Right;
        public int Location { get; set; }

        public AssignmentStatement(string name, Syntax right, Token token) : base(token)
        {
            Name = name;
            Right = right;
        }
    }

    class IdentifierExpression : Syntax
    {
        public readonly string Name;
        public int Location { get; set; }

        public IdentifierExpression(string name, Token token) : base(token)
        {
            Name = name;
        }
    }

    class FieldExpression : Syntax
    {
        public readonly string Name;
        public int Location { get; set; }

        public FieldExpression(string name, Token token) : base(token)
        {
            Name = name;
        }
    }

    class LiteralExpression : Syntax
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

    class BinaryExpression : Syntax
    {
        public readonly BinaryOperator Operator;
        public readonly Syntax Left;
        public readonly Syntax Right;

        public BinaryExpression(BinaryOperator op, Syntax left, Syntax right, Token token) : base(token)
        {
            Operator = op;
            Left = left;
            Right = right;
        }
    }

    class UnaryExpression : Syntax
    {
        public readonly Syntax Right;
        public readonly UnaryOperator Operator;

        public UnaryExpression(UnaryOperator op, Syntax argument, Token token) : base(token)
        {
            Operator = op;
            Right = argument;
        }
    }

    class FunctionExpression : Syntax 
    {
        public readonly string Name;
        public readonly List<Syntax> Arguments;
        public int Location { get; set; }

        public FunctionExpression(string name, List<Syntax> arguments, Token token) : base(token)
        {
            Name = name;
            Arguments = arguments;
        }
    }

}
