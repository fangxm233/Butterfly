using System.Collections.Generic;
using System.Linq;
using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class NewNode : ElementNode
    {
        public List<ExpressionNode> Parms { get; internal set; } = new List<ExpressionNode>();
        public bool IsArray { get; internal set; }
        public FunctionDefinition Constructor { get; internal set; }
        internal string NameWithParms => ToString();
        internal string TypeName;

        public NewNode()
        {
            NodeType = NodeType.Element;
            ElemtntType = ElemtntType.New;
        }

        internal void AddParm(ExpressionNode expression) => Parms.Add(expression);

        public override string ToString() =>
            Parms.Aggregate(".ctor", (current, expression) => current + "#" + expression.Type.Name);
    }
}