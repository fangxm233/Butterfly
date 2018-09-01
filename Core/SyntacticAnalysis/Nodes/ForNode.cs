namespace Core.SyntacticAnalysis.Nodes
{
    public class ForNode : AnalysisNode
    {
        public ChunkNode Left { get; internal set; }
        public ChunkNode Right { get; internal set; }
        public ChunkNode Chunk { get; internal set; }
        public ExpressionNode Middle { get; internal set; }

        public ForNode()
        {
            NodeType = NodeType.For;
        }
    }
}