using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalculatedField
{
    class Function
    {
        public Function(Type type, MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            Name = MethodInfo.Name;
            ArgumentTypes = MethodInfo.GetParameters().Select(parameterInfo => CSharpTypeToScriptType(parameterInfo.ParameterType)).ToList();
            ReturnType = CSharpTypeToScriptType(MethodInfo.ReturnType);
        }

        public ScriptValue Call(ScriptValue[] arguments)
        {
            return new ScriptValue(ReturnType, MethodInfo.Invoke(null, arguments.Select(arg => arg.Value).ToArray()));
        }

        public string Name { get; protected set; }
        public List<ScriptType> ArgumentTypes { get; protected set; }
        public ScriptType ReturnType { get; protected set; }
        public MethodInfo MethodInfo { get; protected set; }


        ScriptType CSharpTypeToScriptType(Type type)
        {
            if (type == typeof(long?))
                return ScriptType.Integer;
            if (type == typeof(decimal?))
                return ScriptType.Decimal;
            if (type == typeof(string))
                return ScriptType.String;
            if (type == typeof(bool?))
                return ScriptType.Bool;
            if (type == typeof(DateTime?))
                return ScriptType.DateTime;
            return ScriptType.Null;
        }
    }
}
