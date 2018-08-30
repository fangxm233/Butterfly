namespace Core.SyntacticAnalysis.Nodes
{
    public class WhileNode : AnalysisNode
    {
        public WhileNode()
        {
            NodeType = NodeType.While;
        }

        public ExpressionNode Condition { get; internal set; }
        public ChunkNode Chunk { get; internal set; }
    }
}