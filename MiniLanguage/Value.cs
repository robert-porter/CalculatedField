using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    
    struct Value
    {
        public Value(double d)
        {
            DoubleVal = d;
            StringVal = "";
            BoolVal = false;
        }
        public Value(String s)
        {
            StringVal = s;
            DoubleVal = 0;
            BoolVal = false;
        }
        public Value(bool b) 
        {
            BoolVal = b;
            DoubleVal = 0;
            StringVal = "";
        } 
        public double DoubleVal;
        public String StringVal;
        public bool BoolVal;
    }
}
