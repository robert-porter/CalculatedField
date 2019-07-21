namespace CalculatedField
{
    enum Instruction : int
    {
        Add = 1000, 
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
        PushConstant,
        PushVariable,
        Store,
        JumpOnFalse,
        Jump,
        Call,
    }
}
