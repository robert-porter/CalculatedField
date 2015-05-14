using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    class CompilerError : Exception
    {
        public readonly int Column;
        public readonly int Line;
        public readonly String FileName;
        public readonly String Description;

        public override string Message
        {
            get
            {
                return String.Format("{0} Line {1}, Column {2}: {3}", FileName, Line, Column, Description);
            }
        }

        public CompilerError(int column, int line, String fileName, String description) 
        {
            Column = column;
            Line = line;
            FileName = fileName;
            Description = description;
        }
    }
}
