using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MiniLanguage
{

    enum Instruction : int
    {
        Pop,
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
        StoreVariable,
        NewVariable,
        JumpOnFalse,
        Call,
        Return,
        Jump
    }

}
