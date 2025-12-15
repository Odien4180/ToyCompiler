namespace ToyCompiler.IR.Optimization;

public interface IOptimizationPass
{
    StackIRProgram Run(StackIRProgram program);
}
