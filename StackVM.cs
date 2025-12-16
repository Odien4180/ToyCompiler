using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ToyCompiler.HostLayer;
using ToyCompiler.IR;

namespace ToyCompiler
{
    public sealed class StackVM
    {
        private readonly Stack<object?> _stack = new();
        private readonly IObjectGetter _objectHost;

        public StackVM(IObjectGetter objectHost)
        {
            _objectHost = objectHost;
        }

        public void Execute(StackIRProgram program)
        {
            // Build label map
            var labels = new Dictionary<string, int>();
            for (int i = 0; i < program.Instructions.Count; i++)
            {
                var instruction = program.Instructions[i];
                if (instruction.OpCode == OpCode.Label)
                {
                    labels[(string)instruction.Operand!] = i;
                }
            }

            // Execute with program counter
            int pc = 0;
            while (pc < program.Instructions.Count)
            {
                var instruction = program.Instructions[pc];
                var jump = ExecuteInstruction(instruction, labels);

                if (jump.HasValue)
                {
                    pc = jump.Value;
                }
                else
                {
                    pc++;
                }
            }
        }

        private int? ExecuteInstruction(StackIRInstruction instruction, Dictionary<string, int> labels)
        {
            switch (instruction.OpCode)
            {
                case OpCode.Label:
                    // Labels are markers, no execution
                    break;

                case OpCode.Jump:
                    {
                        var label = (string)instruction.Operand!;
                        return labels[label];
                    }

                case OpCode.JumpIfFalse:
                    {
                        var value = (int)_stack.Pop()!;
                        if (value == 0)
                        {
                            var label = (string)instruction.Operand!;
                            return labels[label];
                        }
                        break;
                    }

                case OpCode.PushString:
                    _stack.Push(instruction.Operand!);
                    break;

                case OpCode.PushInt:
                    _stack.Push((int)instruction.Operand!);
                    break;

                case OpCode.Add:
                    {
                        int b = (int)_stack.Pop()!;
                        int a = (int)_stack.Pop()!;
                        _stack.Push(a + b);
                        break;
                    }
                case OpCode.Sub:
                    {
                        int b = (int)_stack.Pop()!;
                        int a = (int)_stack.Pop()!;
                        _stack.Push(a - b);
                        break;
                    }
                case OpCode.Mul:
                    {
                        int b = (int)_stack.Pop()!;
                        int a = (int)_stack.Pop()!;
                        _stack.Push(a * b);
                        break;
                    }
                case OpCode.Div:
                    {
                        int b = (int)_stack.Pop()!;
                        int a = (int)_stack.Pop()!;
                        _stack.Push(a / b);
                        break;
                    }
                case OpCode.Mod:
                    {
                        int b = (int)_stack.Pop()!;
                        int a = (int)_stack.Pop()!;
                        _stack.Push(a % b);
                        break;
                    }

                case OpCode.GreaterThan:
                    {
                        int b = (int)_stack.Pop()!;
                        int a = (int)_stack.Pop()!;
                        _stack.Push(a > b ? 1 : 0);
                        break;
                    }
                case OpCode.LessThan:
                    {
                        int b = (int)_stack.Pop()!;
                        int a = (int)_stack.Pop()!;
                        _stack.Push(a < b ? 1 : 0);
                        break;
                    }
                case OpCode.GreaterThanOrEqual:
                    {
                        int b = (int)_stack.Pop()!;
                        int a = (int)_stack.Pop()!;
                        _stack.Push(a >= b ? 1 : 0);
                        break;
                    }
                case OpCode.LessThanOrEqual:
                    {
                        int b = (int)_stack.Pop()!;
                        int a = (int)_stack.Pop()!;
                        _stack.Push(a <= b ? 1 : 0);
                        break;
                    }
                case OpCode.EqualsEquals:
                    {
                        int b = (int)_stack.Pop()!;
                        int a = (int)_stack.Pop()!;
                        _stack.Push(a == b ? 1 : 0);
                        break;
                    }
                case OpCode.NotEquals:
                    {
                        int b = (int)_stack.Pop()!;
                        int a = (int)_stack.Pop()!;
                        _stack.Push(a != b ? 1 : 0);
                        break;
                    }

                case OpCode.PreIncrement:
                    {
                        var target = _stack.Pop();
                        if (target is null)
                            throw new Exception("Null target for pre-increment.");

                        var memberName = (string)instruction.Operand!;
                        var currentValue = (int)GetMemberValue(target, memberName)!;
                        var newValue = currentValue + 1;
                        SetMemberValue(target, memberName, newValue);
                        _stack.Push(newValue);
                        break;
                    }
                case OpCode.PreDecrement:
                    {
                        var target = _stack.Pop();
                        if (target is null)
                            throw new Exception("Null target for pre-decrement.");

                        var memberName = (string)instruction.Operand!;
                        var currentValue = (int)GetMemberValue(target, memberName)!;
                        var newValue = currentValue - 1;
                        SetMemberValue(target, memberName, newValue);
                        _stack.Push(newValue);
                        break;
                    }
                case OpCode.PostIncrement:
                    {
                        var target = _stack.Pop();
                        if (target is null)
                            throw new Exception("Null target for post-increment.");

                        var memberName = (string)instruction.Operand!;
                        var currentValue = (int)GetMemberValue(target, memberName)!;
                        var newValue = currentValue + 1;
                        SetMemberValue(target, memberName, newValue);
                        _stack.Push(currentValue);  // 후위는 이전 값을 반환
                        break;
                    }
                case OpCode.PostDecrement:
                    {
                        var target = _stack.Pop();
                        if (target is null)
                            throw new Exception("Null target for post-decrement.");

                        var memberName = (string)instruction.Operand!;
                        var currentValue = (int)GetMemberValue(target, memberName)!;
                        var newValue = currentValue - 1;
                        SetMemberValue(target, memberName, newValue);
                        _stack.Push(currentValue);  // 후위는 이전 값을 반환
                        break;
                    }

                case OpCode.Pop:
                    if (_stack.Count > 0)
                        _ = _stack.Pop();
                    break;

                case OpCode.LoadObject:
                    {
                        var name = (string)instruction.Operand!;
                        if (!_objectHost.GetObject(name, out var obj) || obj is null)
                            throw new Exception($"Object '{name}' not found in host.");

                        _stack.Push(obj);
                        break;
                    }

                case OpCode.GetProperty:
                    {
                        var target = _stack.Pop();
                        if (target is null)
                            throw new Exception("Null target for property access.");

                        var value = GetMemberValue(target, (string)instruction.Operand!);
                        _stack.Push(value);
                        break;
                    }

                case OpCode.SetProperty:
                    {
                        var value = _stack.Pop();
                        var target = _stack.Pop();

                        if (target is null)
                            throw new Exception("Null target for property assignment.");

                        SetMemberValue(target, (string)instruction.Operand!, value);
                        _stack.Push(value);
                        break;
                    }

                case OpCode.CallMethod:
                    {
                        var call = (MethodCallInfo)instruction.Operand!;
                        var args = new object?[call.ArgumentCount];

                        for (int i = call.ArgumentCount - 1; i >= 0; i--)
                            args[i] = _stack.Pop();

                        var target = _stack.Pop();
                        if (target is null)
                            throw new Exception("Null target for method call.");

                        var (wasVoid, result) = InvokeMethod(target, call, args);

                        if (!wasVoid)
                            _stack.Push(result);

                        break;
                    }

                case OpCode.CallPrint:
                    {
                        var value = _stack.Pop();
                        Console.WriteLine(value);
                        break;
                    }

                default:
                    throw new Exception($"Unknown opcode: {instruction.OpCode}");
            }

            return null;
        }

        private static bool HasExposure(MemberInfo member)
        {
            return member.GetCustomAttribute<ScriptExposeAttribute>() != null ||
                   member.DeclaringType?.GetCustomAttribute<ScriptExposeAttribute>() != null;
        }

        private static object? GetMemberValue(object target, string name)
        {
            var type = target.GetType();
            var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (prop != null && HasExposure(prop))
                return prop.GetValue(target);

            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public);
            if (field != null && HasExposure(field))
                return field.GetValue(target);

            throw new Exception($"Member '{name}' is not accessible or not found on type '{type.Name}'.");
        }

        private static void SetMemberValue(object target, string name, object? value)
        {
            var type = target.GetType();
            var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (prop != null && prop.CanWrite && HasExposure(prop))
            {
                var converted = ConvertValue(value, prop.PropertyType);
                prop.SetValue(target, converted);
                return;
            }

            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public);
            if (field != null && HasExposure(field))
            {
                var converted = ConvertValue(value, field.FieldType);
                field.SetValue(target, converted);
                return;
            }

            throw new Exception($"Member '{name}' is not writable or not found on type '{type.Name}'.");
        }

        private static (bool wasVoid, object? value) InvokeMethod(object target, MethodCallInfo call, object?[] args)
        {
            var type = target.GetType();
            var candidates = type
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.Name == call.Name && m.GetParameters().Length == call.ArgumentCount && HasExposure(m))
                .ToList();

            if (candidates.Count == 0)
                throw new Exception($"Method '{call.Name}' with {call.ArgumentCount} args is not accessible on type '{type.Name}'.");

            foreach (var method in candidates)
            {
                var paramInfos = method.GetParameters();
                var converted = new object?[paramInfos.Length];
                bool convertible = true;

                for (int i = 0; i < paramInfos.Length; i++)
                {
                    try
                    {
                        converted[i] = ConvertValue(args[i], paramInfos[i].ParameterType);
                    }
                    catch
                    {
                        convertible = false;
                        break;
                    }
                }

                if (!convertible)
                    continue;

                var result = method.Invoke(target, converted);
                return (method.ReturnType == typeof(void), result);
            }

            throw new Exception($"No overload of '{call.Name}' accepted provided arguments on type '{type.Name}'.");
        }

        private static object? ConvertValue(object? value, Type targetType)
        {
            if (value == null)
            {
                if (targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null)
                    throw new Exception($"Cannot assign null to non-nullable '{targetType.Name}'.");

                return null;
            }

            var nonNullable = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (nonNullable.IsInstanceOfType(value))
                return value;

            return Convert.ChangeType(value, nonNullable);
        }
    }
}
