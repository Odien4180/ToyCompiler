namespace ToyCompiler.IR;

public sealed class StackIRInstruction
{
    public OpCode OpCode { get; }
    public object? Operand { get; }

    public StackIRInstruction(OpCode opCode, object? operand = null)
    {
        OpCode = opCode;
        Operand = operand;
    }

    public override string ToString()
    {
        return Operand == null
            ? OpCode.ToString()
            : $"{OpCode} {Operand}";
    }
}

public sealed class MethodCallInfo
{
    public string Name { get; }
    public int ArgumentCount { get; }

    public MethodCallInfo(string name, int argumentCount)
    {
        Name = name;
        ArgumentCount = argumentCount;
    }

    public override string ToString()
    {
        return $"{Name}/{ArgumentCount}";
    }
}