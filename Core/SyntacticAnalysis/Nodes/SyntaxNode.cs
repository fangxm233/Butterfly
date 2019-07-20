namespace Core.SyntacticAnalysis.Nodes
{
    public enum NodeType
    {
        Unknown,
        Assign,
        Chunk,
        Continue,
        Break,
        Return,
        Command,
        DefineVariable,
        DefSpecifier,
        For,
        If,
        Invoker,
        New,
        Operate,
        While,

        Expression,
        CustomType,
        NumericLiteral,
        FloatLiteral,
        StringLiteral,
        CharacterLiteral,
        BooleanLiteral,
        Variable,
        Array,
        This,
        Null,
    }

    public class SyntaxNode
    {
        public NodeType NodeType { get; internal set; }

        public SyntaxNode() { }
    }
}