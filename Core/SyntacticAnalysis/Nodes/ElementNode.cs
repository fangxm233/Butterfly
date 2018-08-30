using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public enum ElemtntType
    {
        Unknown,
        Expression,
        CustomType,
        Integer,
        Float,
        Variable,
        String,
        Char,
        Boolean,
        Invoker,
        Array,
        This,
        Null,
    }

    public class ElementNode : ExpressionNode
    {
        public ElemtntType ElemtntType { get; internal set; }
        public string Content { get; internal set; }

        public ElementNode NextElement { get; internal set; }

        public ExpressionNode Expression { get; internal set; }

        public CustomDefinition Type { get; internal set; }

        public DefineVariableNode Definition { get; internal set; }

        public ElementNode(ElemtntType elemtntType, string content)
        {
            ElemtntType = elemtntType;
            Content = content;
            NodeType = NodeType.Element;
        }

        public ElementNode() { }
    }
}