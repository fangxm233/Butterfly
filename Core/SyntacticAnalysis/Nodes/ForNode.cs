using Core.LexicalAnalysis;

namespace Core.SyntacticAnalysis.Nodes
{
    public class ForNode : SyntaxNode
    {
        private Token _for;
        private Token _openParen;
        private Token _closeParen;
        private Token _openBrace;
        private Token _closeBrace;

        public ChunkNode Left { get; internal set; }
        public ChunkNode Right { get; internal set; }
        public ChunkNode Chunk { get; internal set; }
        public ExpressionNode Middle { get; internal set; }

        public ForNode(
            Token @for, 
            Token openParen, 
            Token closeParen, 
            Token openBrace, 
            Token closeBrace, 
            ChunkNode left, 
            ChunkNode right, 
            ChunkNode chunk, 
            ExpressionNode middle)
        {
            NodeType = NodeType.For;
            _for = @for;
            _openParen = openParen;
            _closeParen = closeParen;
            _openBrace = openBrace;
            _closeBrace = closeBrace;
            Left = left;
            Right = right;
            Chunk = chunk;
            Middle = middle;
        }
    }
}