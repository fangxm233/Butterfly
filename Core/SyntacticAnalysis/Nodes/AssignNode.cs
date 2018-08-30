namespace Core.SyntacticAnalysis.Nodes
{
    public class AssignNode : AnalysisNode
    {
        public ElementNode Left;
        public ExpressionNode Right;

        public AssignNode()
        {
            NodeType = NodeType.Assign;
        }
    }
}