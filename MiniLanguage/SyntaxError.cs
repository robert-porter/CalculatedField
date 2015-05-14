using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class SyntaxError : CompilerError
    {
        public SyntaxError(int column, int line, String fileName, String description) : base(column, line, fileName, description)
        {
        }
    }
}
