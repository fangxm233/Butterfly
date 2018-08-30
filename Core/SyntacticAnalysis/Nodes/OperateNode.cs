using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public enum OperateType
    {
        UMinus, Not, Cast,
        Multiply, Divide, Modulus,
        Puls, Minus,
        Greater, GreaterEqual, Less, LessEqual,
        Equal, NotEqual,
        And,
        Or,
    }

    public class OperateNode : ExpressionNode
    {
        public bool IsUnary;
        public OperateType OperateType;
        public ExpressionNode Left, Right;

        internal string CastTypeName;
        public CustomDefinition CastType;

        public OperateNode(OperateType operateType, ExpressionNode left, ExpressionNode right)
        {
            Left = left;
            Right = right;
            OperateType = operateType;
            NodeType = NodeType.Operate;
        }

        public OperateNode(OperateType operateType, ExpressionNode right)
        {
            Right = right;
            IsUnary = true;
            OperateType = operateType;
            NodeType = NodeType.Operate;
        }

        public OperateNode(string castTypeName, ExpressionNode right)
        {
            OperateType = OperateType.Cast;
            NodeType = NodeType.Operate;
            CastTypeName = castTypeName;
            Right = right;
            IsUnary = true;
        }
    }
}