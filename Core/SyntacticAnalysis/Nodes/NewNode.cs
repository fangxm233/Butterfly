using System.Collections.Generic;
using System.Linq;
using Core.LexicalAnalysis;
using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class NewNode : ElementNode
    {
        private Token _new;
        private Token _name;
        private Token _openParen;
        private Token _closeParen;

        public List<ExpressionNode> Parms { get; internal set; } = new List<ExpressionNode>();
        public bool IsArray { get; internal set; }
        public FunctionDefinition Constructor { get; internal set; }
        internal string NameWithParms => ToString();
        internal string TypeName;

        public NewNode(Token @new, Token name, Token openParen, Token closeParen, List<ExpressionNode> parms, bool isArray) : base(NodeType.New, name)
        {
            _new = @new;
            _name = name;
            _openParen = openParen;
            _closeParen = closeParen;
            Parms = parms;
            IsArray = isArray;
            TypeName = name.Content;
        }

        internal void AddParm(ExpressionNode expression) => Parms.Add(expression);

        public override string ToString() =>
            Parms.Aggregate(".ctor", (current, expression) => current + "#" + expression.Type.Name);
    }
}