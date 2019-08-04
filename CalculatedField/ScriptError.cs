using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculatedField
{
    class ScriptError : Exception
    {
        public readonly int Index;
        public readonly string Description;

        public ScriptError(int index, string description) 
        {
            Index = index;
            Description = description;
        }

        public override string Message => $"{Index + 1}: {Description}";

        public static ScriptError UnrecognizedSymbol(int index, string symbol)
        {
            return new ScriptError(index, $"Unrecognized symbol {symbol}.");
        }

        public static ScriptError UnexpectedEOF(int index)
        {
            return new ScriptError(index, "Unexpected end of script.");
        }

        public static ScriptError UnexpectedToken(int index, string contents)
        {
            return new ScriptError(index, $"Unexpected token {contents}.");
        }

        public static ScriptError UnresolvedFunction(int index, string name, List<ScriptType> argumentTypes)
        {
            return new ScriptError(index, $"Function {name}({string.Join(", ", argumentTypes)}) is not defined.");
        }

        public static ScriptError UnresolvedOperator(int index, string operatorString, ScriptType left, ScriptType right)
        {
            return new ScriptError(index, $"Operator {operatorString} is not defined on {left} and {right}");
        }

        public static ScriptError UnresolvedOperator(int index, string operatorString, ScriptType right)
        {
            return new ScriptError(index, $"Operator {operatorString} is not defined on {right}");
        }

        public static ScriptError UnresolvedIdentifier(int index, string name)
        {
            return new ScriptError(index, $"Constant {name} is not defined");
        }

        public static ScriptError UnresolvedField(int index, string name)
        {
            return new ScriptError(index, $"Field {name} is not defined");
        }

    }
}
