using System.Collections.Generic;

namespace Core.SyntacticAnalysis.Nodes
{
    public class ArrayNode : ElementNode
    {
        public List<ExpressionNode> Expressions { get; internal set; } = new List<ExpressionNode>();
    }
}