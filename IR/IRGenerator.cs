namespace ToyCompiler.IR;

public sealed class StackIRGenerator
{
    public StackIRProgram Generate(StatementSyntax statement)
    {
        var program = new StackIRProgram();
        EmitStatement(statement, program);
        return program;
    }

    private void EmitStatement(StatementSyntax statement, StackIRProgram program)
    {
        switch (statement)
        {
            case PrintStatementSyntax print:
                EmitPrint(print, program);
                break;
            case ExpressionStatementSyntax exprStmt:
                EmitExpression(exprStmt.Expression, program);
                program.Instructions.Add(new StackIRInstruction(OpCode.Pop));
                break;

            default:
                throw new Exception($"Unsupported statement: {statement.GetType().Name}");
        }
    }

    private void EmitPrint(PrintStatementSyntax print, StackIRProgram program)
    {
        EmitExpression(print.Argument, program);
        program.Instructions.Add(
            new StackIRInstruction(OpCode.CallPrint)
        );
    }

    private void EmitExpression(ExpressionSyntax expr, StackIRProgram program)
    {
        switch (expr)
        {
            case NumberLiteralExpressionSyntax num:
                program.Instructions.Add(
                    new StackIRInstruction(OpCode.PushInt, num.Value));
                break;

            case StringLiteralExpressionSyntax str:
                program.Instructions.Add(
                    new StackIRInstruction(OpCode.PushString, str.Value));
                break;

            case IdentifierExpressionSyntax ident:
                program.Instructions.Add(
                    new StackIRInstruction(OpCode.LoadObject, ident.Name));
                break;

            case MemberAccessExpressionSyntax member:
                EmitExpression(member.Target, program);
                program.Instructions.Add(
                    new StackIRInstruction(OpCode.GetProperty, member.MemberName));
                break;

            case InvocationExpressionSyntax call:
                if (call.Callee is not MemberAccessExpressionSyntax calleeMember)
                    throw new Exception("Method calls must target a member (obj.method()).");

                EmitExpression(calleeMember.Target, program);

                foreach (var arg in call.Arguments)
                    EmitExpression(arg, program);

                program.Instructions.Add(
                    new StackIRInstruction(OpCode.CallMethod,
                        new MethodCallInfo(calleeMember.MemberName, call.Arguments.Count)));
                break;

            case AssignmentExpressionSyntax assign:
                EmitExpression(assign.Target.Target, program);
                EmitExpression(assign.Value, program);
                program.Instructions.Add(
                    new StackIRInstruction(OpCode.SetProperty, assign.Target.MemberName));
                break;

            case BinaryExpressionSyntax bin:
                EmitExpression(bin.Left, program);
                EmitExpression(bin.Right, program);

                switch (bin.Operator)
                {
                    case TokenType.Plus:
                        program.Instructions.Add(new StackIRInstruction(OpCode.Add));
                        break;
                    case TokenType.Minus:
                        program.Instructions.Add(new StackIRInstruction(OpCode.Sub));
                        break;
                    case TokenType.Asterisk:
                        program.Instructions.Add(new StackIRInstruction(OpCode.Mul));
                        break;
                    case TokenType.Slash:
                        program.Instructions.Add(new StackIRInstruction(OpCode.Div));
                        break;
                    case TokenType.Percent:
                        program.Instructions.Add(new StackIRInstruction(OpCode.Mod));
                        break;
                    default:
                        throw new Exception($"Unsupported operator: {bin.Operator}");
                }

                break;

            default:
                throw new Exception("Unsupported expression");
        }
    }
}
