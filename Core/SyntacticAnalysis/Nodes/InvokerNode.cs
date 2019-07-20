using System.Collections.Generic;
using System.Linq;
using Core.LexicalAnalysis;
using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class InvokerNode : ElementNode
    {
        private Token _name;
        private Token _openParen;
        private Token _closeParen;

        public string Name { get; internal set; }
        public List<ExpressionNode> Parms { get; internal set; } = new List<ExpressionNode>();
        public FunctionDefinition Function { get; internal set; }
        internal string NameWithParms => ToString();

        public InvokerNode(Token name, Token openParen, Token closeParen, List<ExpressionNode> parms) : base(NodeType.Invoker, name)
        {
            _name = name;
            _openParen = openParen;
            _closeParen = closeParen;
            Name = name.Content;
            Parms = parms;
        }

        internal void AddParm(ExpressionNode expression)
        {
            Parms.Add(expression);
        }

        public override string ToString() => Parms.Aggregate(Name, (current, expression) => current + "#" + expression.Type.Name);
    }
}