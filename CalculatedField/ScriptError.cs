using System;

namespace CalculatedField
{
    class ScriptError : Exception
    {
        public readonly int Column;
        public readonly int Line;
        public readonly string Description;

        public ScriptError(int column, int line, string description) 
        {
            Column = column;
            Line = line;
            Description = description;
        }

        public override string Message => String.Format("Line {0}, Column {1}: {2}", Line, Column, Description);
    }
}
