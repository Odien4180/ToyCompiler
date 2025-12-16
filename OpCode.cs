namespace ToyCompiler;

public enum OpCode
{
    PushString,
    PushInt,
    Add,
    Sub,
    Mul,
    Div,
    Mod,

    // Comparison operators
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    EqualsEquals,
    NotEquals,

    // Increment/Decrement operators
    PreIncrement,
    PreDecrement,
    PostIncrement,
    PostDecrement,

    CallPrint,

    Pop,

    // Jump instructions
    Jump,
    JumpIfFalse,
    Label,

    LoadObject,
    CallMethod,
    GetProperty,
    SetProperty
}
