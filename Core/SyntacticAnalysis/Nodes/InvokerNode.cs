using System.Collections.Generic;
using System.Linq;
using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class InvokerNode : ElementNode
    {
        public string Name { get; internal set; }
        public List<ExpressionNode> Parms { get; internal set; } = new List<ExpressionNode>();
        public FunctionDefinition Function { get; internal set; }
        internal string NameWithParms => ToString();

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

        public override string ToString() => Parms.Aggregate(Name, (current, expression) => current + "#" + expression.Type.FullName);
    }
}