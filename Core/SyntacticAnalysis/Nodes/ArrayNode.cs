using Core.LexicalAnalysis;
using System.Collections.Generic;

namespace Core.SyntacticAnalysis.Nodes
{
    public class ArrayNode : ElementNode
    {
        private Token _identifier;
        private Token _openBracket;
        private Token _closeBracket;

        public ArrayNode(Token identifier, Token openBracket, Token closeBracket, List<ExpressionNode> expressions, List<Token> commas) 
            : base(NodeType.Array, identifier)
        {
            _identifier = identifier;
            _openBracket = openBracket;
            _closeBracket = closeBracket;
            Expressions = expressions;
            _commas = commas;
        }

        public List<ExpressionNode> Expressions { get; internal set; } = new List<ExpressionNode>();
        private List<Token> _commas;
    }
}