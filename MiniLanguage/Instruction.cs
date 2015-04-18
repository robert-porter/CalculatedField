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
        Over,
        Tuck,
        Add, 
        Subtract, 
        Multiply,
        Divide,
        Negate,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
        Equal,
        NotEqual,
        And, 
        Or,    
        Not,
        PushTrue,
        PushFalse,
        PushNumber,
        PushVariable,
        PushOffsetVariable,
        PushReference,
        StoreVariable,
        StoreOffsetVariable,
        StoreReference,
        NewVariable,
        NewReference,
        NewArray,
        JumpOnFalse,
        Call,
        Return,
        Jump
    }

}
