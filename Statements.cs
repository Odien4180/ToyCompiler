namespace ToyCompiler
{
    public abstract class StatementSyntax : SyntaxNode { }

    public sealed class PrintStatementSyntax : StatementSyntax
    {
        public ExpressionSyntax Argument { get; }

        public PrintStatementSyntax(ExpressionSyntax argument)
        {
            Argument = argument;
        }
    }

    public sealed class ExpressionStatementSyntax : StatementSyntax
    {
        public ExpressionSyntax Expression { get; }

        public ExpressionStatementSyntax(ExpressionSyntax expression)
        {
            Expression = expression;
        }
    }

    public sealed class BlockStatementSyntax : StatementSyntax
    {
        public IReadOnlyList<StatementSyntax> Statements { get; }

        public BlockStatementSyntax(IReadOnlyList<StatementSyntax> statements)
        {
            Statements = statements;
        }
    }

    public sealed class WhileStatementSyntax : StatementSyntax
    {
        public ExpressionSyntax Condition { get; }
        public StatementSyntax Body { get; }

        public WhileStatementSyntax(ExpressionSyntax condition, StatementSyntax body)
        {
            Condition = condition;
            Body = body;
        }
    }

    public sealed class ForStatementSyntax : StatementSyntax
    {
        public StatementSyntax? Initializer { get; }
        public ExpressionSyntax? Condition { get; }
        public ExpressionSyntax? Increment { get; }
        public StatementSyntax Body { get; }

        public ForStatementSyntax(StatementSyntax? initializer, ExpressionSyntax? condition, ExpressionSyntax? increment, StatementSyntax body)
        {
            Initializer = initializer;
            Condition = condition;
            Increment = increment;
            Body = body;
        }
    }

    public sealed class IfStatementSyntax : StatementSyntax
    {
        public ExpressionSyntax Condition { get; }
        public StatementSyntax ThenBranch { get; }
        public StatementSyntax? ElseBranch { get; }

        public IfStatementSyntax(ExpressionSyntax condition, StatementSyntax thenBranch, StatementSyntax? elseBranch = null)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }
    }
}