namespace ToyCompiler.IR
{
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

    public sealed class LabelInfo
    {
        public string Name { get; }
        public int InstructionIndex { get; set; }

        public LabelInfo(string name)
        {
            Name = name;
            InstructionIndex = -1;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}