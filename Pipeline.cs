using ToyCompiler.IR;
using ToyCompiler.IR.Optimization;

namespace ToyCompiler
{
    public static class Pipeline
    {
        private static readonly IRCache IRCache = new();

        public static void Execute(string source)
        {
            if (IRCache.TryGet(source, out var cachedProgram) == false)
            {
                // 1. Lexer
                var lexer = new Lexer(source);

                // 2. Parser → AST (여러 문장 파싱)
                var parser = new Parser(lexer);
                var irGenerator = new StackIRGenerator();
                cachedProgram = new StackIRProgram();

                while (!parser.IsEnd)
                {
                    var ast = parser.ParseStatement();
                    var part = irGenerator.Generate(ast);
                    foreach (var instr in part.Instructions)
                        cachedProgram.Instructions.Add(instr);
                }

                var passes = new List<IOptimizationPass>
            {
                new ConstantFoldingPass(),
                // new DeadCodeEliminationPass(),
            };

                foreach (var pass in passes)
                {
                    cachedProgram = pass.Run(cachedProgram);
                }
                IRCache.Set(source, cachedProgram);
            }

            if (cachedProgram == null)
                throw new InvalidOperationException("IR 프로그램이 생성되지 않았습니다.");

            // (선택) IR 출력
            foreach (var instr in cachedProgram.Instructions)
                Console.WriteLine(instr);

            Console.WriteLine("---- VM Output ----");

            // 4. Stack VM 실행
            var host = new ObjectHost();
            var vm = new StackVM(host);
            vm.Execute(cachedProgram);
        }
    }
}