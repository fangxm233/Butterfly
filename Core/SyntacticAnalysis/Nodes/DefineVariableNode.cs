using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public class DefineVariableNode : AnalysisNode
    {
        public CustomDefinition Type { get; internal set; }
        public string Name { get; internal set; }
        public AccessLevel AccessLevel { get; internal set; }
        public bool IsStatic { get; internal set; }

        public bool IsArray { get; internal set; }
        public byte RankNum { get; internal set; }

        internal string TypeName;

        public DefineVariableNode(string name, string typeName, AccessLevel accessLevel, bool isStatic = false, bool isArray = false, byte rankNum = 0)
        {
            Name = name;
            TypeName = typeName;
            NodeType = NodeType.DefineVariable;
            AccessLevel = accessLevel;
            IsStatic = isStatic;
            IsArray = isArray;
            RankNum = rankNum;
        }
    }
}