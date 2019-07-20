using Core.LexicalAnalysis;

namespace Core.SyntacticAnalysis.Nodes
{
    public class ContinueNode : SyntaxNode
    {
        private Token _continue;
        private Token _semicolon;

        public SyntaxNode Loop { get; internal set; }

        public ContinueNode(Token @contniue, Token semicolon)
        {
            NodeType = NodeType.Continue;
            _continue = @contniue;
            _semicolon = semicolon;
        }
    }
}
