using System;
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
        public OperateType OperateType { get; internal set; }
        public ExpressionNode Left { get; internal set; }
        public ExpressionNode Right { get; internal set; }
        public string OP
        {
            get
            {
                switch (OperateType)
                {
                    case OperateType.UMinus:
                        return "-";
                    case OperateType.Not:
                        return "!";
                    case OperateType.Cast:
                        return "cast<>";
                    case OperateType.Multiply:
                        return "*";
                    case OperateType.Divide:
                        return "/";
                    case OperateType.Modulus:
                        return "%";
                    case OperateType.Puls:
                        return "+";
                    case OperateType.Minus:
                        return "-";
                    case OperateType.Greater:
                        return ">";
                    case OperateType.GreaterEqual:
                        return ">=";
                    case OperateType.Less:
                        return "<";
                    case OperateType.LessEqual:
                        return "<=";
                    case OperateType.Equal:
                        return "==";
                    case OperateType.NotEqual:
                        return "!=";
                    case OperateType.And:
                        return "&&";
                    case OperateType.Or:
                        return "||";
                }
                return null;
            }
        }

        public CustomDefinition CastType { get; internal set; }
        internal string CastTypeName;

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
            OperateType = operateType;
            NodeType = NodeType.Operate;
        }

        public OperateNode(string castTypeName, ExpressionNode right)
        {
            OperateType = OperateType.Cast;
            NodeType = NodeType.Operate;
            CastTypeName = castTypeName;
            Right = right;
        }
    }
}