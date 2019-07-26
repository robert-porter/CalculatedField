namespace CalculatedField
{
    public enum Instruction : byte
    {
        Pop = 128,
        Add, 
        Subtract, 
        Multiply,
        Divide,
        DivideAndTruncate, 
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
        Call,
    }
}
