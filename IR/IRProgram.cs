namespace ToyCompiler.IR
{
    public sealed class StackIRProgram
    {
        public List<StackIRInstruction> Instructions { get; } = new();
    }
}