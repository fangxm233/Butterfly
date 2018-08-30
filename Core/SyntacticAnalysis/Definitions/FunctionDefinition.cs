using System.Collections.Generic;
using Core.SyntacticAnalysis.Nodes;

namespace Core.SyntacticAnalysis.Definitions
{
    public class FunctionDefinition
    {
        public FunctionDefinition(string name, AccessLevel accessLevel, bool isStatic)
        {
            Name = name;
            AccessLevel = accessLevel;
            IsStatic = isStatic;
            NameWithParms = name;
        }

        public string Name { get; internal set; }
        public string NameWithParms { get; internal set; }
        public CustomDefinition Structure { get; internal set; }
        public CustomDefinition ReturnType { get; internal set; }
        public List<DefineVariableNode> ParmDefinition { get; internal set; } = new List<DefineVariableNode>();
        public ChunkNode ChunkNode { get; internal set; }
        public AccessLevel AccessLevel { get; internal set; }
        public bool IsStatic { get; internal set; }
        internal string ReturnTypeName;

        internal void AddParm(DefineVariableNode variable)
        {
            ParmDefinition.Add(variable);
            NameWithParms += "#" + variable.TypeName;
        }

        public override string ToString() => Structure + "::" + Name;
    }
}