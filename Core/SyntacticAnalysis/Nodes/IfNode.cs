using Core.LexicalAnalysis;

namespace Core.SyntacticAnalysis.Nodes
{
    public class IfNode : SyntaxNode
    {
        private Token _if;
        private Token _openParen;
        private Token _closeParen;
        private Token _openBrace;
        private Token _closeBrace;
        private Token _else;
        private Token _EopenBrace;
        private Token _EcloseBrace;

        public ExpressionNode Condition { get; internal set; }
        public ChunkNode Chunk { get; internal set; }
        public ChunkNode ElseChunk { get; internal set; }

        public IfNode(Token @if, Token openParen, Token closeParen, Token openBrace, Token closeBrace, 
            ExpressionNode condition, ChunkNode chunk, Token @else = null, Token EopenBrace = null, Token EcloseBrace = null, ChunkNode elseChunk = null)
        {
            NodeType = NodeType.If;
            _if = @if;
            _openParen = openParen;
            _closeParen = closeParen;
            _openBrace = openBrace;
            _closeBrace = closeBrace;
            _else = @else;
            _EopenBrace = EopenBrace;
            _EcloseBrace = EcloseBrace;
            Condition = condition;
            Chunk = chunk;
            ElseChunk = elseChunk;
        }
    }
}