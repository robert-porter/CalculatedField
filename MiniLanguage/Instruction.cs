using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MiniLanguage
{

    enum Instruction : int
    {
        Pop = 100,
        Dup,
        Add,
        Subtract,
        Multiply,
        Divide,
        Negate,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
        DoubleEqual,
        NotEqual,
        And, 
        Or,    
        Not,
        LoadTrue,
        LoadFalse,
        LoadNumber,
        LoadVariable,
        LoadOffsetVariable,
        StoreVariable,
        StoreOffsetVariable,
        NewVariable,
        NewArray,
        JumpOnFalse,
        Call,
        Return,
        Jump
    }

}
