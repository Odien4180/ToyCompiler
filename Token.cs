namespace ToyCompiler;

public enum TokenType
{
    Identifier,
    StringLiteral,
    NumberLiteral,
    LParen,
    RParen,
    Plus,
    Minus,
    Asterisk,
    Slash,
    Percent,
    Dot,
    Comma,
    Equal,
    EOF
}


public sealed class Token
{
    public TokenType Type { get; }
    public string Lexeme { get; }

    public Token(TokenType type, string lexeme)
    {
        Type = type;
        Lexeme = lexeme;
    }

    public override string ToString()
    {
        return $"{Type} : {Lexeme}";
    }
}