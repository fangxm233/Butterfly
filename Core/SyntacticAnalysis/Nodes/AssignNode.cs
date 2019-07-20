using Core.LexicalAnalysis;

namespace Core.SyntacticAnalysis.Nodes
{
    public class AssignNode : SyntaxNode
    {
        private Token _assign;
        
        public ElementNode Left { get; internal set; }
        public ExpressionNode Right { get; internal set; }

        public AssignNode(Token assign, ElementNode left, ExpressionNode right)
        {
            NodeType = NodeType.Assign;
            _assign = assign;
            Left = left;
            Right = right;
        }
    }
}