using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatedField
{
    public class Field
    {
        public Guid FieldId { get; set; }
        public ScriptType Type { get; set; }
        public string Name { get; set; }
        public string Script { get; set; }
    }
}
