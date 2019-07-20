using Core.LexicalAnalysis;
using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class ElementNode : ExpressionNode
    {
        private Token _content;

        public string Content { get; internal set; }

        //给除了null、Unknown和expression以外的所有种类
        public ElementNode NextElement { get; internal set; }
        public ElementNode LastElement => GetLastElement();

        //给括号的表达式
        public ExpressionNode Expression { get; internal set; }

        //给除了null和Unknown以外的所有种类
        public DefineVariableNode Definition { get; internal set; }

        public ElementNode(NodeType nodeType, Token content)
        {
            Content = content.Content;
            _content = content;
            NodeType = nodeType;
        }

        public ElementNode(NodeType nodeType, ExpressionNode expression)
        {
            NodeType = nodeType;
            Expression = expression;
        }

        public ElementNode GetLastElement()
        {
            if (NextElement == null) return this;
            ElementNode element = NextElement;
            while (element.NextElement != null)
                element = element.NextElement;
            return element;
        }
    }
}