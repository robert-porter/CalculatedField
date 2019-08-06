using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculatedField
{
    public class ScriptError : Exception
    {
        public readonly Token Token;
        public readonly string Description;

        public ScriptError(Token token, string description)
        {
            Token = token;
            Description = description;
        }

        public (int, int) Range => (Token.Index, Token.Index + Token.Contents.Length);       

        public override string Message
        {
            get
            {
                if (Token != null)
                    return $"{Token.Index + 1}: {Description}";
                else
                    return Description;
            }
        }

        public static ScriptError UnrecognizedSymbol(Token token, string symbol)
        {
            return new ScriptError(token, $"Unrecognized symbol {symbol}.");
        }

        public static ScriptError UnexpectedEOF(Token token)
        {
            return new ScriptError(token, "Unexpected end of script.");
        }

        public static ScriptError UnexpectedToken(Token token, string contents)
        {
            return new ScriptError(token, $"Unexpected token {contents}.");
        }

        public static ScriptError UnresolvedFunction(Token token, string name, List<ScriptType> argumentTypes)
        {
            return new ScriptError(token, $"Function {name}({string.Join(", ", argumentTypes)}) is not defined.");
        }

        public static ScriptError UnresolvedOperator(Token token, string operatorString, ScriptType left, ScriptType right)
        {
            return new ScriptError(token, $"Operator {operatorString} is not defined on {left} and {right}");
        }

        public static ScriptError UnresolvedOperator(Token token, string operatorString, ScriptType right)
        {
            return new ScriptError(token, $"Operator {operatorString} is not defined on {right}");
        }

        public static ScriptError UnresolvedIdentifier(Token token, string name)
        {
            return new ScriptError(token, $"Constant {name} is not defined");
        }

        public static ScriptError UnresolvedField(Token token, string name)
        {
            return new ScriptError(token, $"Field {name} is not defined");
        }

    }
}
