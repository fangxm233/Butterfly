namespace Compiler.SyntacticAnalysis.Nodes
{
    public class AssignNode : AnalysisNode
    {
        public ElementNode Left;
        public ExpressionNode Right;
    }
}