using System;
using System.Collections.Generic;

namespace ToyCompiler.IR
{
    public sealed class IRCache : IDisposable
    {
        private readonly Dictionary<string, StackIRProgram> _cache = new();

        public bool TryGet(string key, out StackIRProgram? program)
        {
            return _cache.TryGetValue(key, out program);
        }

        public void Set(string key, StackIRProgram program)
        {
            _cache[key] = program;
        }

        public void Dispose()
        {
            _cache.Clear();
        }
    }
}