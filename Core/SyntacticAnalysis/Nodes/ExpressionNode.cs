using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class ExpressionNode : AnalysisNode
    {
        public CustomDefinition Type { get; internal set; }
    }
}