namespace Compiler.SyntacticAnalysis.Nodes
{
    public class ForNode : AnalysisNode
    {
        public AnalysisNode Left;
        public ExpressionNode Middle;
        public AnalysisNode Right;
    }
}