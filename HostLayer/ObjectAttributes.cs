using System;

namespace ToyCompiler.HostLayer
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct |
        AttributeTargets.Interface | AttributeTargets.Property |
        AttributeTargets.Field |
        AttributeTargets.Method)]
    public class ScriptExposeAttribute : Attribute { }
}