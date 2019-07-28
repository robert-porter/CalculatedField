using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalculatedField
{
    class Symbols
    {
        public Symbols(List<Field> entityFields)
        {
            EntityFields = entityFields;
        }

        public MethodInfo GetFunction(string searchName, List<Type> searchArguments)
        {
            for (var f = 0; f < Runtime.Functions.Count; f++)
            {
                var function = Runtime.Functions[f];
                var arguments = Runtime.Functions[f].GetParameters();
                if (function.Name == searchName && arguments.Count() == searchArguments.Count)
                {
                    bool argumentsMatch = true;
                    for (int a = 0; a < searchArguments.Count; a++)
                    {
                        if (searchArguments[a] != arguments[a].ParameterType)
                        {
                            argumentsMatch = false;
                            break;
                        }
                    }
                    if (argumentsMatch)
                    {
                        return function;
                    }
                }
            }
            return null;

        }

        public Field GetField(string name)
        {
            return EntityFields.Find(field => field.Name == name);
        }

        public void AddScriptField(Field field)
        {
            if (ScriptFields.Find(f => f.Name == field.Name) != null)
                ScriptFields.Add(field);
        }

        List<Field> EntityFields { get; set; }
        List<Field> ScriptFields { get; set; }
    }
}
   
