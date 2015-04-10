using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class FunctionContext
    {
        String Name { get; set; }
        int Location { get; set; }

        public FunctionContext(String name, int location)
        {
            Instructions = new List<Instruction>();
            Name = name;
            Location = location;
        }

        public List<Instruction> Instructions { get; set; }

    }
}
