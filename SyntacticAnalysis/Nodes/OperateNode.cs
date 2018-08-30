namespace Compiler.SyntacticAnalysis.Nodes
{
    public class OperateNode : ExpressionNode
    {
        public bool IsBinary, IsUnary;
        public ExpressionNode Left, Right;
    }
}