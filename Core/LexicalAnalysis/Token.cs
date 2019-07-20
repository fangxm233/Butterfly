namespace Core.LexicalAnalysis
{
    public class Token
    {
        public TokenType Type;
        public string Content;

        public int Line;
        public int Column;
        public int File;

        public Token(TokenType tokenType, int line, int column)
        {
            Type = tokenType;
            Line = line;
            Column = column;
        }

        public Token(string content, int line, int column)
        {
            Content = content;
            Line = line;
            Column = column;
        }

        public override string ToString() => Content ?? Type.ToString();
    }
}