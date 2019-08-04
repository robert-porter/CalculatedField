using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CalculatedField
{
    class Function
    {
        public string Name { get; set; }
        public ScriptType ReturnType { get; set; }
        public List<ScriptType> ArgumentTypes { get; set; }
        public MethodInfo Method { get; set; }
    }
}
