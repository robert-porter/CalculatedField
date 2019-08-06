using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatedField
{
    public class Field
    {
        public string FieldId { get; set; }
        public Type Type { get; set; }
        public string Name { get; set; }
        public string Script { get; set; }
        public string Alias => "{" + Name + "}";
    }
}
