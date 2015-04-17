using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace MiniLanguage
{
    [StructLayout(LayoutKind.Explicit)]
    struct Value
    {
        enum ValueType
        {
            Number, 
            True,
            False,
            Array
        }

        public Value(double d) : this()
        {
            DoubleVal = d;
        }
        public Value(bool b)  : this()
        {
            BoolVal = b;
        } 
        [FieldOffset(0)]
        public double DoubleVal;
        [FieldOffset(0)]
        public bool BoolVal;

        [FieldOffset(8)]
        List<Value> ArrayVal;
        [FieldOffset(8)]
        String StringVal;

        [FieldOffset(16)]
        ValueType Type;
    }
}
