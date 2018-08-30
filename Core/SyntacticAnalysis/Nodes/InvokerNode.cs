using System.Collections.Generic;
using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class InvokerNode : ElementNode
    {
        public string Name { get; internal set; }
        public List<CustomDefinition> ParmTypes { get; internal set; } = new List<CustomDefinition>();
        public List<ExpressionNode> Parms { get; internal set; } = new List<ExpressionNode>();

        public InvokerNode(string name)
        {
            Name = name;
            ElemtntType = ElemtntType.Invoker;
            NodeType = NodeType.Invoker;
        }

        internal void AddParm(ExpressionNode expression)
        {
            Parms.Add(expression);
        }
    }
}