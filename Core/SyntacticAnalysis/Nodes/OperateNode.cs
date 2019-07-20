using System;
using Core.LexicalAnalysis;
using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public enum OperateType
    {
        UMinus, Not, Cast,
        Multiply, Divide, Modulus,
        Plus, Minus,
        Greater, GreaterEqual, Less, LessEqual,
        Equal, NotEqual,
        And,
        Or,
    }

    public class OperateNode : ExpressionNode
    {
        private Token _op;
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
                    case OperateType.Plus:
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

        public OperateNode(Token op, OperateType operateType, ExpressionNode left, ExpressionNode right)
        {
            _op = op;
            Left = left;
            Right = right;
            OperateType = operateType;
            NodeType = NodeType.Operate;
        }

        public OperateNode(Token op, OperateType operateType, ExpressionNode right)
        {
            _op = op;
            Right = right;
            OperateType = operateType;
            NodeType = NodeType.Operate;
        }

        public OperateNode(Token op, string castTypeName, ExpressionNode right)
        {
            _op = op;
            OperateType = OperateType.Cast;
            NodeType = NodeType.Operate;
            CastTypeName = castTypeName;
            Right = right;
        }
    }
}