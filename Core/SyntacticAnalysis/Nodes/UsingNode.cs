using Core.LexicalAnalysis;
using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class UsingNode : SyntaxNode
    {
        private Token _content;

        public NameSpaceDefinition NameSpace { get; internal set; }
        public UsingNode PreviousUsing { get; internal set; }
        public UsingNode NextUsing { get; internal set; }
        internal string Name;

        public UsingNode(Token content, UsingNode previousUsing)
        {
            _content = content;
            PreviousUsing = previousUsing;
            Name = content.Content;
        }

        internal void AddAfter(Token name, SyntaxFactory syntaxFactory)
        {
            NextUsing = syntaxFactory.GetUsingNode(name, this);
        }

        public UsingNode GetLast() => NextUsing == null ? this : NextUsing.GetLast();
    }
}