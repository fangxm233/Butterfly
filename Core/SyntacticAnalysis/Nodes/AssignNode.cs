namespace Core.SyntacticAnalysis.Nodes
{
    public class AssignNode : AnalysisNode
    {
        public ElementNode Left { get; internal set; }
        public ExpressionNode Right { get; internal set; }

        public AssignNode()
        {
            NodeType = NodeType.Assign;
        }
    }
}