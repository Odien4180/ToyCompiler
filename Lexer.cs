namespace ToyCompiler;

public sealed class Lexer
{
    private readonly string _source;
    private int _position;

    public Lexer(string source)
    {
        _source = source;
        _position = 0;
    }

    public Token NextToken()
    {
        SkipWhitespace();

        if (IsAtEnd())
            return new Token(TokenType.EOF, string.Empty);

        char current = Peek();

        // Identifier (Print)
        if (char.IsLetter(current))
            return ReadIdentifier();

        // String Literal ("Hellow world")
        if (current == '"')
            return ReadStringLiteral();

        // NextToken 내부
        if (char.IsDigit(current))
            return ReadNumber();

        // +, -, *, /, % 처리
        if (current == '+')
        {
            Advance();
            return new Token(TokenType.Plus, "+");
        }
        if (current == '-')
        {
            Advance();
            return new Token(TokenType.Minus, "-");
        }
        if (current == '*')
        {
            Advance();
            return new Token(TokenType.Asterisk, "*");
        }
        if (current == '/')
        {
            Advance();
            return new Token(TokenType.Slash, "/");
        }
        if (current == '%')
        {
            Advance();
            return new Token(TokenType.Percent, "%");
        }

        // ., ,, = 처리
        if (current == '.')
        {
            Advance();
            return new Token(TokenType.Dot, ".");
        }
        if (current == ',')
        {
            Advance();
            return new Token(TokenType.Comma, ",");
        }
        if (current == '=')
        {
            Advance();
            return new Token(TokenType.Equal, "=");
        }

        // Single-character tokens
        Advance();
        switch (current)
        {
            case '(':
                return new Token(TokenType.LParen, "(");
            case ')':
                return new Token(TokenType.RParen, ")");
        }

        throw new Exception($"Unexpected character: '{current}'");
    }

    // -------------------------
    // Helpers
    // -------------------------

    private Token ReadIdentifier()
    {
        int start = _position;

        while (!IsAtEnd() && char.IsLetterOrDigit(Peek()))
            Advance();

        string text = _source.Substring(start, _position - start);
        return new Token(TokenType.Identifier, text);
    }

    private Token ReadNumber()
    {
        int start = _position;

        while (!IsAtEnd() && char.IsDigit(Peek()))
            Advance();

        string text = _source.Substring(start, _position - start);
        return new Token(TokenType.NumberLiteral, text);
    }

    private Token ReadStringLiteral()
    {
        Advance(); // skip opening "

        int start = _position;

        while (!IsAtEnd() && Peek() != '"')
            Advance();

        if (IsAtEnd())
            throw new Exception("Unterminated string literal.");

        string value = _source.Substring(start, _position - start);
        Advance(); // skip closing "

        return new Token(TokenType.StringLiteral, value);
    }

    private void SkipWhitespace()
    {
        while (!IsAtEnd() && char.IsWhiteSpace(Peek()))
            Advance();
    }

    private bool IsAtEnd()
    {
        return _position >= _source.Length;
    }

    private char Peek()
    {
        return _source[_position];
    }

    private void Advance()
    {
        _position++;
    }
}