using System.Collections.Generic;
using Core.SyntacticAnalysis.Nodes;

namespace Core.SyntacticAnalysis.Definitions
{
    public class StructDefinition : CustomDefinition
    {
        public CustomDefinition InheritanceType;
        public readonly Dictionary<string, DefineVariableNode> ContainFields = new Dictionary<string, DefineVariableNode>();
        internal string InheritanceName;

        public StructDefinition(string name, NameSpaceDefinition nameSpace, AccessLevel accessLevel, bool isStatic) :
            base(name, nameSpace, accessLevel, isStatic)
        {
        }

        internal bool AddField(DefineVariableNode fieldDefination)
        {
            if (ContainFields.ContainsKey(fieldDefination.Name)) return false;
            ContainFields.Add(fieldDefination.Name, fieldDefination);
            return true;
        }
    }
}