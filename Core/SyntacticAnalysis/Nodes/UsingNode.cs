using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class UsingNode : AnalysisNode
    {
        public NameSpaceDefinition NameSpace { get; internal set; }
        public UsingNode PreviousUsing { get; internal set; }
        public UsingNode NextUsing { get; internal set; }
        internal string Name;

        public UsingNode(string name)
        {
            Name = name;
        }

        internal void AddAfter(string name)
        {
            NextUsing = new UsingNode(name);
            NextUsing.PreviousUsing = this;
        }

        public UsingNode GetLast() => NextUsing == null ? this : NextUsing.GetLast();
    }
}