namespace CalculatedField
{
    public enum Instruction : int
    {
        Pop,
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
        PushField,
        Store,
        JumpOnFalse,
        Jump,
        Call,
    }
}
