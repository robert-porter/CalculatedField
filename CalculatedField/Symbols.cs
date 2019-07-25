using System;
using System.Collections.Generic;

namespace CalculatedField
{
    class Symbols
    {
        public List<Field> GetScriptFields() => ScriptFields;
        public List<Function> GetScriptFunctions() => ScriptFunctions;
        public List<ScriptValue> GetScriptConstants() => ScriptConstants;
        public int ScriptVariablesCount => ScriptVariables.Count;
        public Field GetField() => Field;

        public Symbols(Field field, List<Field> fields)
        {
            Field = field;
            Fields = fields;
            ScriptVariables = new Dictionary<string, Variable>();
            ScriptConstants = new List<ScriptValue>();
            ScriptFunctions = new List<Function>();
            ScriptFields = new List<Field>();
        }
 
        public Variable GetVariable(string name)
        {
            if (!ScriptVariables.TryGetValue(name, out var value))
                return null;
            return value;
        }

        public int GetFunctionLocation(string name, List<ScriptType> arguments)
        {
            return IndexOfFunction(name, arguments, ScriptFunctions);
        }

        public Function GetScriptFunction(int location)
        {
            return ScriptFunctions[location];
        }

        public int GetConstantLocation(ScriptValue value)
        {
            for (int i = 0; i < ScriptConstants.Count; i++)
            {
                if (value.Equals(ScriptConstants[i]))
                    return i;
            }
            return -1;
        }
        public List<Field> GetFields()
        {
            return Fields;
        }

        public int GetScriptFieldLocation(string name)
        {
            for(int i = 0; i < ScriptFields.Count; i++)
            {
                if (ScriptFields[i].Name == name)
                    return i;
            }
            return -1;
        }

        public int AddScriptField(Field field)
        {
            int location = ScriptFields.Count;
            ScriptFields.Add(field);
            return location;
        }

        public int AddConstant(ScriptValue value)
        {
            var location = ScriptConstants.Count;
            ScriptConstants.Add(value);
            return location;
        }

        public int AddVariable(string name, ScriptType type)
        {
            var location = ScriptVariables.Count;
            ScriptVariables.Add(name, new Variable
            {
                Location = location,
                Type = type
            });
            return location;
        }

        public int AddFunction(string name, List<ScriptType> arguments)
        {
            var index = IndexOfFunction(name, arguments, Functions);
            if (index == -1) return -1;

            var location = ScriptFunctions.Count;
            ScriptFunctions.Add(Functions[index]);
            return location;
        }

        Dictionary<string, Variable> ScriptVariables { get; set; }
        List<ScriptValue> ScriptConstants { get; set; }
        List<Function> ScriptFunctions { get; set; }
        List<Field> Fields { get; set; }
        List<Field> ScriptFields { get; set; }
        Field Field { get; set; }
        public static List<Function> Functions => Runtime.Functions;


        int IndexOfFunction(string name, List<ScriptType> argumentTypes, List<Function> functions)
        {
            for (var f = 0; f < functions.Count; f++)
            {
                if (functions[f].Name == name && functions[f].ArgumentTypes.Count == argumentTypes.Count)
                {
                    bool argumentsMatch = true;
                    for (int a = 0; a < argumentTypes.Count; a++)
                    {
                        if (argumentTypes[a] != functions[f].ArgumentTypes[a])
                        {
                            argumentsMatch = false;
                            break;
                        }
                    }
                    if (argumentsMatch)
                    {
                        return f;
                    }
                }
            }
            return -1;
        }
    }
}
