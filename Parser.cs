namespace ToyCompiler;

public sealed partial class Parser
{
    private readonly Lexer _lexer;
    private Token _current;

    public Parser(Lexer lexer)
    {
        _lexer = lexer;
        _current = _lexer.NextToken();
    }

    public bool IsEnd => _current.Type == TokenType.EOF;

    public StatementSyntax ParseStatement()
    {
        if (_current.Type == TokenType.Identifier &&
            _current.Lexeme == "Print")
        {
            return ParsePrintStatement();
        }

        // 일반 표현식 문장 (메서드 호출, 할당 등)
        var expr = ParseExpression();
        return new ExpressionStatementSyntax(expr);
    }

    // -------------------------
    // Helpers
    // -------------------------

    private void Advance()
    {
        _current = _lexer.NextToken();
    }

    private void Consume(TokenType type, string errorMessage)
    {
        if (_current.Type != type)
            throw new Exception(errorMessage);

        Advance();
    }
}

public sealed partial class Parser
{
    /// <summary>
    /// Parses a Print statement.
    /// </summary>
    private PrintStatementSyntax ParsePrintStatement()
    {
        Consume(TokenType.Identifier, "Expected 'Print'");
        Consume(TokenType.LParen, "Expected '(' after Print");

        var argument = ParseExpression();

        Consume(TokenType.RParen, "Expected ')' after expression");

        return new PrintStatementSyntax(argument);
    }

    private ExpressionSyntax ParseExpression()
    {
        return ParseAssignment();
    }

    private ExpressionSyntax ParseAssignment()
    {
        var expr = ParseAddition();

        if (_current.Type == TokenType.Equal)
        {
            if (expr is not MemberAccessExpressionSyntax target)
                throw new Exception("Left side of assignment must be a member access.");

            Advance();
            var value = ParseAssignment();
            return new AssignmentExpressionSyntax(target, value);
        }

        return expr;
    }

    private ExpressionSyntax ParseAddition()
    {
        var expr = ParseMultiplication();

        while (_current.Type == TokenType.Plus || _current.Type == TokenType.Minus)
        {
            var op = _current.Type;
            Advance();
            var right = ParseMultiplication();
            expr = new BinaryExpressionSyntax(expr, op, right);
        }

        return expr;
    }

    private ExpressionSyntax ParseMultiplication()
    {
        var expr = ParsePrimary();

        while (_current.Type == TokenType.Asterisk || _current.Type == TokenType.Slash || _current.Type == TokenType.Percent)
        {
            var op = _current.Type;
            Advance();
            var right = ParsePrimary();
            expr = new BinaryExpressionSyntax(expr, op, right);
        }

        return expr;
    }

    private ExpressionSyntax ParsePrimary()
    {
        if (_current.Type == TokenType.NumberLiteral)
        {
            int value = int.Parse(_current.Lexeme);
            Advance();
            return new NumberLiteralExpressionSyntax(value);
        }

        if (_current.Type == TokenType.StringLiteral)
        {
            var value = _current.Lexeme;
            Advance();
            return new StringLiteralExpressionSyntax(value);
        }

        if (_current.Type == TokenType.Identifier)
        {
            var name = _current.Lexeme;
            Advance();
            return ParsePostfix(new IdentifierExpressionSyntax(name));
        }

        if (_current.Type == TokenType.LParen)
        {
            Advance();
            var inner = ParseExpression();
            Consume(TokenType.RParen, "Expected ')' after expression");
            return ParsePostfix(inner);
        }

        throw new Exception("Unexpected expression");
    }

    private ExpressionSyntax ParsePostfix(ExpressionSyntax expr)
    {
        while (true)
        {
            if (_current.Type == TokenType.Dot)
            {
                Advance();
                if (_current.Type != TokenType.Identifier)
                    throw new Exception("Expected identifier after '.'.");

                var memberName = _current.Lexeme;
                Advance();
                expr = new MemberAccessExpressionSyntax(expr, memberName);
                continue;
            }

            if (_current.Type == TokenType.LParen)
            {
                Advance();
                var args = new List<ExpressionSyntax>();

                if (_current.Type != TokenType.RParen)
                {
                    while (true)
                    {
                        args.Add(ParseExpression());

                        if (_current.Type == TokenType.Comma)
                        {
                            Advance();
                            continue;
                        }

                        break;
                    }
                }

                Consume(TokenType.RParen, "Expected ')' after arguments");
                expr = new InvocationExpressionSyntax(expr, args);
                continue;
            }

            break;
        }

        return expr;
    }
}
