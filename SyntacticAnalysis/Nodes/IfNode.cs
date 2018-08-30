namespace Compiler.SyntacticAnalysis.Nodes
{
    public class IfNode : AnalysisNode
    {
        public ChunkNode Chunk;
        public ExpressionNode Condition;
        public ChunkNode Else;
    }
}