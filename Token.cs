namespace ToyCompiler;

public enum TokenType
{
    Identifier,
    StringLiteral,
    NumberLiteral,
    LParen,
    RParen,
    LBrace,
    RBrace,
    Plus,
    Minus,
    PlusPlus,
    MinusMinus,
    Asterisk,
    Slash,
    Percent,
    Dot,
    Comma,
    Semicolon,
    Equal,
    GreaterThan,

    // Keywords
    For,
    While,
    If,
    Else,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    EqualsEquals,
    NotEquals,
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