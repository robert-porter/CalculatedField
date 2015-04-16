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
        Add,
        Subtract,
        Multiply,
        Divide,
        Less,
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
