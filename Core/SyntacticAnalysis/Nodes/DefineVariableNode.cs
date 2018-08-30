using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class DefineVariableNode : AnalysisNode
    {
        public CustomDefinition Type { get; internal set; }
        public string Name { get; internal set; }
        public AccessLevel AccessLevel { get; internal set; }
        public bool IsStatic { get; internal set; }
        internal string TypeName;

        public DefineVariableNode(string name, string typeName, AccessLevel accessLevel, bool isStatic = false)
        {
            Name = name;
            TypeName = typeName;
            NodeType = NodeType.DefineVariable;
            AccessLevel = accessLevel;
            IsStatic = isStatic;
        }
    }
}