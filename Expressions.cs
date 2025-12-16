namespace ToyCompiler
{
    public abstract class ExpressionSyntax : SyntaxNode { }

    public sealed class StringLiteralExpressionSyntax : ExpressionSyntax
    {
        public string Value { get; }

        public StringLiteralExpressionSyntax(string value)
        {
            Value = value;
        }
    }

    public sealed class NumberLiteralExpressionSyntax : ExpressionSyntax
    {
        public int Value { get; }

        public NumberLiteralExpressionSyntax(int value)
        {
            Value = value;
        }
    }

    public sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Left { get; }
        public ExpressionSyntax Right { get; }
        public TokenType Operator { get; }

        public BinaryExpressionSyntax(
            ExpressionSyntax left,
            TokenType op,
            ExpressionSyntax right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }
    }

    public sealed class IdentifierExpressionSyntax : ExpressionSyntax
    {
        public string Name { get; }

        public IdentifierExpressionSyntax(string name)
        {
            Name = name;
        }
    }

    public sealed class MemberAccessExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Target { get; }
        public string MemberName { get; }

        public MemberAccessExpressionSyntax(ExpressionSyntax target, string memberName)
        {
            Target = target;
            MemberName = memberName;
        }
    }

    public sealed class InvocationExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Callee { get; }
        public IReadOnlyList<ExpressionSyntax> Arguments { get; }

        public InvocationExpressionSyntax(ExpressionSyntax callee, IReadOnlyList<ExpressionSyntax> arguments)
        {
            Callee = callee;
            Arguments = arguments;
        }
    }

    public sealed class AssignmentExpressionSyntax : ExpressionSyntax
    {
        public MemberAccessExpressionSyntax Target { get; }
        public ExpressionSyntax Value { get; }

        public AssignmentExpressionSyntax(MemberAccessExpressionSyntax target, ExpressionSyntax value)
        {
            Target = target;
            Value = value;
        }
    }

    public sealed class IncrementDecrementExpressionSyntax : ExpressionSyntax
    {
        public MemberAccessExpressionSyntax Target { get; }
        public TokenType Operator { get; }  // PlusPlus or MinusMinus
        public bool IsPrefix { get; }

        public IncrementDecrementExpressionSyntax(MemberAccessExpressionSyntax target, TokenType op, bool isPrefix)
        {
            Target = target;
            Operator = op;
            IsPrefix = isPrefix;
        }
    }
}
