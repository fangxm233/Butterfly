using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class ExpressionNode : SyntaxNode
    {
        public ExpressionNode() { }
        public CustomDefinition Type { get; internal set; }
    }
}