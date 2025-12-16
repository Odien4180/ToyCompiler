namespace ToyCompiler.IR.Optimization
{
    public sealed class ConstantFoldingPass : IOptimizationPass
    {
        public StackIRProgram Run(StackIRProgram program)
        {
            var optimized = new StackIRProgram();
            var instrs = program.Instructions;

            for (int i = 0; i < instrs.Count; i++)
            {
                // PUSH_INT a, PUSH_INT b, ADD
                if (i + 2 < instrs.Count &&
                    instrs[i].OpCode == OpCode.PushInt &&
                    instrs[i + 1].OpCode == OpCode.PushInt &&
                    instrs[i + 2].OpCode == OpCode.Add)
                {
                    int a = (int)instrs[i].Operand!;
                    int b = (int)instrs[i + 1].Operand!;
                    optimized.Instructions.Add(
                        new StackIRInstruction(OpCode.PushInt, a + b)
                    );
                    i += 2;
                    continue;
                }

                // PUSH_INT a, PUSH_INT b, SUB
                if (i + 2 < instrs.Count &&
                    instrs[i].OpCode == OpCode.PushInt &&
                    instrs[i + 1].OpCode == OpCode.PushInt &&
                    instrs[i + 2].OpCode == OpCode.Sub)
                {
                    int a = (int)instrs[i].Operand!;
                    int b = (int)instrs[i + 1].Operand!;
                    optimized.Instructions.Add(
                        new StackIRInstruction(OpCode.PushInt, a - b)
                    );
                    i += 2;
                    continue;
                }

                // PUSH_INT a, PUSH_INT b, MUL
                if (i + 2 < instrs.Count &&
                    instrs[i].OpCode == OpCode.PushInt &&
                    instrs[i + 1].OpCode == OpCode.PushInt &&
                    instrs[i + 2].OpCode == OpCode.Mul)
                {
                    int a = (int)instrs[i].Operand!;
                    int b = (int)instrs[i + 1].Operand!;
                    optimized.Instructions.Add(
                        new StackIRInstruction(OpCode.PushInt, a * b)
                    );
                    i += 2;
                    continue;
                }

                // PUSH_INT a, PUSH_INT b, DIV
                if (i + 2 < instrs.Count &&
                    instrs[i].OpCode == OpCode.PushInt &&
                    instrs[i + 1].OpCode == OpCode.PushInt &&
                    instrs[i + 2].OpCode == OpCode.Div)
                {
                    int a = (int)instrs[i].Operand!;
                    int b = (int)instrs[i + 1].Operand!;
                    optimized.Instructions.Add(
                        new StackIRInstruction(OpCode.PushInt, a / b)
                    );
                    i += 2;
                    continue;
                }

                // PUSH_INT a, PUSH_INT b, MOD
                if (i + 2 < instrs.Count &&
                    instrs[i].OpCode == OpCode.PushInt &&
                    instrs[i + 1].OpCode == OpCode.PushInt &&
                    instrs[i + 2].OpCode == OpCode.Mod)
                {
                    int a = (int)instrs[i].Operand!;
                    int b = (int)instrs[i + 1].Operand!;
                    optimized.Instructions.Add(
                        new StackIRInstruction(OpCode.PushInt, a % b)
                    );
                    i += 2;
                    continue;
                }

                optimized.Instructions.Add(instrs[i]);
            }

            return optimized;
        }
    }
}
