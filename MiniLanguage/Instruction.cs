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
        PushReference,
        StoreVariable,
        StoreReference,
        NewVariable,
        JumpOnFalse,
        Call,
        Return,
        Jump
    }

}
