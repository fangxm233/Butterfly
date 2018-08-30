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

        //给除了null、Unknown和expression以外的所有种类
        public ElementNode NextElement { get; internal set; }

        //给括号、array和new的表达式
        public ExpressionNode Expression { get; internal set; }

        //给除了null和Unknown以外的所有种类
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