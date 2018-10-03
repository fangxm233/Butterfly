using System.Collections.Generic;

namespace Core.SyntacticAnalysis.Nodes
{
    public class ArrayNode : ElementNode
    {
        public ArrayNode()
        {
            NodeType = NodeType.Element;
        }

        public List<ExpressionNode> Expressions { get; internal set; } = new List<ExpressionNode>();
    }
}