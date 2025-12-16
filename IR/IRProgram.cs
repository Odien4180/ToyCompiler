using System.Collections.Generic;

namespace ToyCompiler.IR
{
    public sealed class StackIRProgram
    {
        public List<StackIRInstruction> Instructions { get; } = new();
    }
}