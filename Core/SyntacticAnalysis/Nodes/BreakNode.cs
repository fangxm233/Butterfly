using Core.LexicalAnalysis;

namespace Core.SyntacticAnalysis.Nodes
{
    public class BreakNode : SyntaxNode
    {
        private Token _break;
        private Token _semicolon;

        public SyntaxNode Loop { get; internal set; }

        public BreakNode(Token @break, Token semicolon)
        {
            NodeType = NodeType.Break;
            _break = @break;
            _semicolon = semicolon;
        }
    }
}
