using System.Collections.Generic;

namespace CalculatedField
{
    class CompiledScript
    {
        public List<Instruction> Instructions { get; set; }
        public List<ScriptValue> Constants { get; set; }
        public List<string> Functions { get; set; }
        public List<string> Variables { get; set; }
    }
}
