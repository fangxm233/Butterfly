using Core.LexicalAnalysis;
using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class ReturnNode : SyntaxNode
    {
        private Token _return;
        private Token _semicolon;

        public ExpressionNode Expression { get; internal set; }
        public FunctionDefinition Function { get; internal set; }

        public ReturnNode(Token @return, Token semicolon, ExpressionNode expression)
        {
            NodeType = NodeType.Return;
            _return = @return;
            _semicolon = semicolon;
            Expression = expression;
        }

        public ReturnNode(Token @return, Token semicolon)
        {
            NodeType = NodeType.Return;
            _return = @return;
            _semicolon = semicolon;
        }
    }
}
