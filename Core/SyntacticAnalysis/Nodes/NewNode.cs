using System.Collections.Generic;
using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class NewNode : ElementNode
    {
        public CustomDefinition Type { get; internal set; }
        public List<ExpressionNode> Parms { get; internal set; } = new List<ExpressionNode>();
        internal string TypeName;

        internal void AddParm(ExpressionNode expression)
        {
            Parms.Add(expression);
            NodeType = NodeType.New;
        }
    }
}