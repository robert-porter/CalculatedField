using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MiniLanguage
{
    
    class Value
    {

    }

    class NumberValue : Value
    {
        public NumberValue(double d) { 
            DoubleVal = d;
        }
        public double DoubleVal;
    }

    class BoolValue : Value
    {
        public BoolValue(bool b)
        {
            BoolVal = b;
        }
        public bool BoolVal;
    }

    class StringValue : Value
    {
        public String StringVal;
    }
}
