using System.Collections.Generic;

namespace CalculatedField
{
    public class CompiledScript
    {
        public List<Instruction> Instructions { get; set; }
        public List<ScriptValue> Constants { get; set; }
        public List<Function> Functions { get; set; }
        public List<Field> Fields { get; set; }
        public int NumVariables { get; set; }
        public Field Field { get; set; }
    }
}
