namespace Compiler.LexicalAnalysis
{
    public enum TokenType
    {
        FloatC, StringC, Number, True, False, CharC,
        Float, String, Byte, Char, Int, Bool,
        If, While, For, Break, Continue, Class, Struct, Interface, NameSpace, Using, Func, Var, New, Override, Static,
        Sign, Assign, Parenthesis, SquareBracket, Semicolon, Braces, BasicType, BasicTypeI, Identifer,
        Space, EOL, EOF,
    }

    public class Token
    {
        public TokenType Type;
        public string Content;

        public int Line;
        public int File;

        public Token(TokenType tokenType)
        {
            Line = Lexer.Line;
            File = Lexer.NowFile;
            Type = tokenType;
        }

        public Token(string content)
        {
            Line = Lexer.Line;
            File = Lexer.NowFile;
            Content = content;
        }

        public override string ToString() => Content ?? Type.ToString();
    }
}