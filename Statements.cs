namespace ToyCompiler;


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