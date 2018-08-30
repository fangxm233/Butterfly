namespace Core.SyntacticAnalysis.Nodes
{
    public class IfNode : AnalysisNode
    {
        public ExpressionNode Condition { get; internal set; }
        public ChunkNode Chunk { get; internal set; }
        public ChunkNode Else { get; internal set; }

        public IfNode(ExpressionNode condition)
        {
            Condition = condition;
            NodeType = NodeType.If;
        }
    }
}