using System;

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

        public override string Message => $"{Index}: {Description}";
    }
}
