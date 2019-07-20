using Core.LexicalAnalysis;

namespace Core.SyntacticAnalysis.Nodes
{
    public class WhileNode : SyntaxNode
    {
        private Token _while;
        private Token _openParen;
        private Token _closeParen;
        private Token _openBrace;
        private Token _closeBrace;

        public WhileNode(Token @while, Token openParen, Token closeParen, Token openBrace, Token closeBrace, ExpressionNode condition, ChunkNode chunk)
        {
            NodeType = NodeType.While;
            _while = @while;
            _openParen = openParen;
            _closeParen = closeParen;
            _openBrace = openBrace;
            _closeBrace = closeBrace;
            Condition = condition;
            Chunk = chunk;
        }

        public ExpressionNode Condition { get; internal set; }
        public ChunkNode Chunk { get; internal set; }
    }
}