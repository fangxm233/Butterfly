using Core.LexicalAnalysis;
using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class DefSpecifierNode : SyntaxNode
    {
        public DefSpecifierNode(string content)
        {
            Content = content;
            NodeType = NodeType.DefSpecifier;
        }

        public string Content { get; internal set; }
        public DefSpecifierNode NextSpecifier { get; internal set; }
        public CustomDefinition Type { get; internal set; }
        public NameSpaceDefinition NameSpace { get; internal set; }

        public override string ToString() => Content + (NextSpecifier == null ? "" : "." + NextSpecifier);
    }
}