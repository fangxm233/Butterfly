using System.Collections.Generic;
using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class NewNode : ElementNode
    {
        public List<ExpressionNode> Parms { get; internal set; } = new List<ExpressionNode>();
        public bool IsArray { get; internal set; }
        internal string TypeName;

        internal void AddParm(ExpressionNode expression)
        {
            Parms.Add(expression);
            NodeType = NodeType.New;
        }
    }
}