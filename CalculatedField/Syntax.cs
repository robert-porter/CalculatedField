using System;
using System.Collections.Generic;
using System.Reflection;

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
        public Type Type { get; set; }
        public Syntax(Token token)
        {
            Token = token;
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
        public readonly string StringValue;
        public readonly object Value;


        public int Location { get; set; }

        public LiteralExpression(string value, Token token) : base(token)
        {
            StringValue = value;
            switch (token.Type)
            {
                case TokenType.DecimalLiteral:
                    Value  = decimal.Parse(StringValue);
                    break;
                case TokenType.True:
                case TokenType.False:
                    Value = bool.Parse(StringValue);
                    break;
                case TokenType.DateTimeLiteral:
                   Value = DateTime.Parse(StringValue);
                    break;
                case TokenType.StringLiteral:
                   Value = StringValue;
                    break;
                default:
                   Value = null;
                    break;
            }
            switch (Token.Type)
            {
                case TokenType.DecimalLiteral:
                    Type = typeof(decimal?);
                    break;
                case TokenType.True:
                case TokenType.False:
                    Type = typeof(bool?);
                    break;
                case TokenType.DateTimeLiteral:
                    Type = typeof(DateTime?);
                    break;
                case TokenType.StringLiteral:
                    Type = typeof(string);
                    break;
                default:
                    Type = null;
                    break;

            }
        }


    }

    class BinaryExpression : Syntax
    {
        public readonly BinaryOperator Operator;
        public readonly Syntax Left;
        public readonly Syntax Right;
        public MethodInfo Method { get; set; }

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
        public MethodInfo Method { get; set; }

        public FunctionExpression(string name, List<Syntax> arguments, Token token) : base(token)
        {
            Name = name;
            Arguments = arguments;
        }
    }

}
