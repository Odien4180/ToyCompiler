namespace ToyCompiler.IR;

public sealed class StackIRGenerator
{
    private int _labelCounter = 0;

    public StackIRProgram Generate(StatementSyntax statement)
    {
        var program = new StackIRProgram();
        EmitStatement(statement, program);
        return program;
    }

    private string GenerateLabel(string prefix)
    {
        return $"{prefix}_{_labelCounter++}";
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
            case BlockStatementSyntax block:
                foreach (var stmt in block.Statements)
                {
                    EmitStatement(stmt, program);
                }
                break;
            case WhileStatementSyntax whileStmt:
                EmitWhile(whileStmt, program);
                break;
            case ForStatementSyntax forStmt:
                EmitFor(forStmt, program);
                break;
            case IfStatementSyntax ifStmt:
                EmitIf(ifStmt, program);
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

    private void EmitWhile(WhileStatementSyntax whileStmt, StackIRProgram program)
    {
        var startLabel = GenerateLabel("while_start");
        var endLabel = GenerateLabel("while_end");

        // Start label
        program.Instructions.Add(new StackIRInstruction(OpCode.Label, startLabel));

        // Condition
        EmitExpression(whileStmt.Condition, program);
        program.Instructions.Add(new StackIRInstruction(OpCode.JumpIfFalse, endLabel));

        // Body
        EmitStatement(whileStmt.Body, program);

        // Jump back to start
        program.Instructions.Add(new StackIRInstruction(OpCode.Jump, startLabel));

        // End label
        program.Instructions.Add(new StackIRInstruction(OpCode.Label, endLabel));
    }

    private void EmitFor(ForStatementSyntax forStmt, StackIRProgram program)
    {
        // Initializer
        if (forStmt.Initializer != null)
        {
            EmitStatement(forStmt.Initializer, program);
        }

        var startLabel = GenerateLabel("for_start");
        var endLabel = GenerateLabel("for_end");
        var continueLabel = GenerateLabel("for_continue");

        // Start label
        program.Instructions.Add(new StackIRInstruction(OpCode.Label, startLabel));

        // Condition (if exists)
        if (forStmt.Condition != null)
        {
            EmitExpression(forStmt.Condition, program);
            program.Instructions.Add(new StackIRInstruction(OpCode.JumpIfFalse, endLabel));
        }

        // Body
        EmitStatement(forStmt.Body, program);

        // Continue label (for increment)
        program.Instructions.Add(new StackIRInstruction(OpCode.Label, continueLabel));

        // Increment
        if (forStmt.Increment != null)
        {
            EmitExpression(forStmt.Increment, program);
            program.Instructions.Add(new StackIRInstruction(OpCode.Pop));
        }

        // Jump back to start
        program.Instructions.Add(new StackIRInstruction(OpCode.Jump, startLabel));

        // End label
        program.Instructions.Add(new StackIRInstruction(OpCode.Label, endLabel));
    }

    private void EmitIf(IfStatementSyntax ifStmt, StackIRProgram program)
    {
        var elseLabel = GenerateLabel("else");
        var endLabel = GenerateLabel("if_end");

        // Condition
        EmitExpression(ifStmt.Condition, program);
        program.Instructions.Add(new StackIRInstruction(OpCode.JumpIfFalse, elseLabel));

        // Then branch
        EmitStatement(ifStmt.ThenBranch, program);

        if (ifStmt.ElseBranch != null)
        {
            // Jump over else branch
            program.Instructions.Add(new StackIRInstruction(OpCode.Jump, endLabel));
        }

        // Else label
        program.Instructions.Add(new StackIRInstruction(OpCode.Label, elseLabel));

        // Else branch (if exists)
        if (ifStmt.ElseBranch != null)
        {
            EmitStatement(ifStmt.ElseBranch, program);
            program.Instructions.Add(new StackIRInstruction(OpCode.Label, endLabel));
        }
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

            case IncrementDecrementExpressionSyntax incDec:
                EmitExpression(incDec.Target.Target, program);

                OpCode opCode;
                if (incDec.IsPrefix)
                {
                    opCode = incDec.Operator == TokenType.PlusPlus ? OpCode.PreIncrement : OpCode.PreDecrement;
                }
                else
                {
                    opCode = incDec.Operator == TokenType.PlusPlus ? OpCode.PostIncrement : OpCode.PostDecrement;
                }

                program.Instructions.Add(
                    new StackIRInstruction(opCode, incDec.Target.MemberName));
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
                    case TokenType.GreaterThan:
                        program.Instructions.Add(new StackIRInstruction(OpCode.GreaterThan));
                        break;
                    case TokenType.LessThan:
                        program.Instructions.Add(new StackIRInstruction(OpCode.LessThan));
                        break;
                    case TokenType.GreaterThanOrEqual:
                        program.Instructions.Add(new StackIRInstruction(OpCode.GreaterThanOrEqual));
                        break;
                    case TokenType.LessThanOrEqual:
                        program.Instructions.Add(new StackIRInstruction(OpCode.LessThanOrEqual));
                        break;
                    case TokenType.EqualsEquals:
                        program.Instructions.Add(new StackIRInstruction(OpCode.EqualsEquals));
                        break;
                    case TokenType.NotEquals:
                        program.Instructions.Add(new StackIRInstruction(OpCode.NotEquals));
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
