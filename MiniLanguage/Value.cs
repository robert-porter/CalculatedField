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
            String,
            Ref,
            Array
        }

        public Value(double d) : this()
        {
            Type = ValueType.Number;
            DoubleVal = d;
        }
        public Value(bool b)  : this()
        {
            if (b)
                Type = ValueType.True;
            else
                Type = ValueType.False;
        }
        public Value(String s)
            : this()
        {
            Type = ValueType.String;
            StringVal = s;
        }

        public Value(int pointer) : this()
        {
            Type = ValueType.Ref;
            PointerVal = pointer;
        }

        public bool AsBool()
        {
            if (Type == ValueType.True)
                return true;
            else if (Type == ValueType.False)
                return false;
            else
                throw new Exception("type exception");
        }

        [FieldOffset(0)]
        public double DoubleVal;
        [FieldOffset(0)]
        public int PointerVal;

        [FieldOffset(8)]
        List<Value> ArrayVal;
        [FieldOffset(8)]
        public String StringVal;

        [FieldOffset(16)]
        ValueType Type;
    }
}
