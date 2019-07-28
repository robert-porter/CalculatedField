using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/*
namespace CalculatedField
{
    public class Function
    {
        public Function(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            Name = MethodInfo.Name;
            ArgumentTypes = MethodInfo.GetParameters().Select(parameterInfo => ScriptValue.CSharpTypeToScriptType(parameterInfo.ParameterType)).ToList();
            ReturnType = ScriptValue.CSharpTypeToScriptType(MethodInfo.ReturnType);
        }

        public string Name { get; protected set; }
        public List<ScriptType> ArgumentTypes { get; protected set; }
        public ScriptType ReturnType { get; protected set; }
        public MethodInfo MethodInfo { get; protected set; }

        public ScriptValue Call(ScriptValue[] arguments)
        {
            return new ScriptValue(ReturnType, MethodInfo.Invoke(null, arguments.Select(arg => arg.Value).ToArray()));
        }

    }
}
*/