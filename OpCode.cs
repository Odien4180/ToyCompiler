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
    CallPrint,

    Pop,

    LoadObject,
    CallMethod,
    GetProperty,
    SetProperty
}
