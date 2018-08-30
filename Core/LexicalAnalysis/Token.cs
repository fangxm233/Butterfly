namespace Core.LexicalAnalysis
{
    public enum TokenType
    {
        Float, String, Number, True, False, Char,
        If, Else, While, For, Class, Struct, Interface, NameSpace, Using, Func, Var, Let, Inv, Cast, New, Override, Static, Public,
        Sign, Assign, Parenthesis, SquareBracket, Semicolon, LBraces, RBraces, BasicType, BasicTypeI, Identifer, Command,
        Blank, EOL, EOF,
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
            File = Lexer.FileIndex;
            Type = tokenType;
        }

        public Token(string content)
        {
            Line = Lexer.Line;
            File = Lexer.FileIndex;
            Content = content;
        }

        public override string ToString() => Content ?? Type.ToString();
    }
}