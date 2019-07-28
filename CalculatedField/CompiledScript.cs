using System;
using System.Collections.Generic;

namespace CalculatedField
{
    public class CompiledScript
    {
        public List<Field> FieldsUsed { get; set; }
        public Func<object> Calculate { get; set; }
    }
}
