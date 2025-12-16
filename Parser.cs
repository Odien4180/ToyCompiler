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
        if (_current.Type == TokenType.LBrace)
        {
            return ParseBlockStatement();
        }

        if (_current.Type == TokenType.If)
        {
            return ParseIfStatement();
        }

        if (_current.Type == TokenType.While)
        {
            return ParseWhileStatement();
        }

        if (_current.Type == TokenType.For)
        {
            return ParseForStatement();
        }

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
    /// Parses an if statement.
    /// </summary>
    private IfStatementSyntax ParseIfStatement()
    {
        Consume(TokenType.If, "Expected 'if'");
        Consume(TokenType.LParen, "Expected '(' after 'if'");
        var condition = ParseExpression();
        Consume(TokenType.RParen, "Expected ')' after condition");
        var thenBranch = ParseStatement();

        StatementSyntax? elseBranch = null;
        if (_current.Type == TokenType.Else)
        {
            Advance();
            elseBranch = ParseStatement();
        }

        return new IfStatementSyntax(condition, thenBranch, elseBranch);
    }

    /// <summary>
    /// Parses a while statement.
    /// </summary>
    private WhileStatementSyntax ParseWhileStatement()
    {
        Consume(TokenType.While, "Expected 'while'");
        Consume(TokenType.LParen, "Expected '(' after 'while'");
        var condition = ParseExpression();
        Consume(TokenType.RParen, "Expected ')' after condition");
        var body = ParseStatement();
        return new WhileStatementSyntax(condition, body);
    }

    /// <summary>
    /// Parses a for statement.
    /// </summary>
    private ForStatementSyntax ParseForStatement()
    {
        Consume(TokenType.For, "Expected 'for'");
        Consume(TokenType.LParen, "Expected '(' after 'for'");

        // Initializer (optional)
        StatementSyntax? initializer = null;
        if (_current.Type != TokenType.Semicolon)
        {
            var expr = ParseExpression();
            initializer = new ExpressionStatementSyntax(expr);
        }
        Consume(TokenType.Semicolon, "Expected ';' after for initializer");

        // Condition (optional)
        ExpressionSyntax? condition = null;
        if (_current.Type != TokenType.Semicolon)
        {
            condition = ParseExpression();
        }
        Consume(TokenType.Semicolon, "Expected ';' after for condition");

        // Increment (optional)
        ExpressionSyntax? increment = null;
        if (_current.Type != TokenType.RParen)
        {
            increment = ParseExpression();
        }
        Consume(TokenType.RParen, "Expected ')' after for clauses");

        var body = ParseStatement();
        return new ForStatementSyntax(initializer, condition, increment, body);
    }

    /// <summary>
    /// Parses a block statement { ... }.
    /// </summary>
    private BlockStatementSyntax ParseBlockStatement()
    {
        Consume(TokenType.LBrace, "Expected '{'");

        var statements = new List<StatementSyntax>();
        while (_current.Type != TokenType.RBrace && _current.Type != TokenType.EOF)
        {
            statements.Add(ParseStatement());
        }

        Consume(TokenType.RBrace, "Expected '}'");

        return new BlockStatementSyntax(statements);
    }

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
        var expr = ParseComparison();

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

    private ExpressionSyntax ParseComparison()
    {
        var expr = ParseAddition();

        while (_current.Type == TokenType.GreaterThan ||
               _current.Type == TokenType.LessThan ||
               _current.Type == TokenType.GreaterThanOrEqual ||
               _current.Type == TokenType.LessThanOrEqual ||
               _current.Type == TokenType.EqualsEquals ||
               _current.Type == TokenType.NotEquals)
        {
            var op = _current.Type;
            Advance();
            var right = ParseAddition();
            expr = new BinaryExpressionSyntax(expr, op, right);
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
        var expr = ParseUnary();

        while (_current.Type == TokenType.Asterisk || _current.Type == TokenType.Slash || _current.Type == TokenType.Percent)
        {
            var op = _current.Type;
            Advance();
            var right = ParseUnary();
            expr = new BinaryExpressionSyntax(expr, op, right);
        }

        return expr;
    }

    private ExpressionSyntax ParseUnary()
    {
        // 전위 증감 연산자 처리
        if (_current.Type == TokenType.PlusPlus || _current.Type == TokenType.MinusMinus)
        {
            var op = _current.Type;
            Advance();
            var expr = ParseUnary();
            if (expr is not MemberAccessExpressionSyntax target)
                throw new Exception("Increment/decrement operator requires a member access expression.");
            return new IncrementDecrementExpressionSyntax(target, op, isPrefix: true);
        }

        return ParsePrimary();
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
            // 후위 증감 연산자 처리
            if (_current.Type == TokenType.PlusPlus || _current.Type == TokenType.MinusMinus)
            {
                if (expr is not MemberAccessExpressionSyntax target)
                    throw new Exception("Increment/decrement operator requires a member access expression.");
                var op = _current.Type;
                Advance();
                return new IncrementDecrementExpressionSyntax(target, op, isPrefix: false);
            }

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
