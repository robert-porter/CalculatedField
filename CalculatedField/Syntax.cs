using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalculatedField
{

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
        public FieldExpression(string name, Token token) : base(token)
        {
            Name = name;
        }
    }

    class LiteralExpression : Syntax
    {
        public readonly string StringValue;
        public readonly object Value;
        public LiteralExpression(string value, Token token) : base(token)
        {
            StringValue = value;
            switch (token.Type)
            {
                case TokenType.DecimalLiteral:
                    Value  = decimal.Parse(StringValue);
                    break;
                case TokenType.BooleanLiteral:
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
                case TokenType.BooleanLiteral:
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

    class IdentifierExpression : Syntax
    {
        public readonly string Name;
        public PropertyInfo Property { get; set; }
        public IdentifierExpression(string name, Token token) : base(token)
        {
            Name = name;
        }
    }

    class BinaryExpression : Syntax
    {
        public readonly Syntax Left;
        public readonly Syntax Right;
        public BinaryExpression( Syntax left, Syntax right, Token token) : base(token)
        {
            Left = left;
            Right = right;
        }
    }

    class UnaryExpression : Syntax
    {
        public readonly Syntax Right;
        public UnaryExpression(Syntax argument, Token token) : base(token)
        {
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
