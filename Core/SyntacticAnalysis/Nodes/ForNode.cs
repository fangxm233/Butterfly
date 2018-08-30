namespace Core.SyntacticAnalysis.Nodes
{
    public class ForNode : AnalysisNode
    {
        public AnalysisNode Left { get; internal set; }
        public ExpressionNode Middle { get; internal set; }
        public AnalysisNode Right { get; internal set; }
        public ChunkNode Chunk { get; internal set; }

        public ForNode()
        {
            NodeType = NodeType.For;
        }
    }
}