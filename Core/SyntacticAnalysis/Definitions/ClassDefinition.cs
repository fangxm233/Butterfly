using System.Collections.Generic;
using Core.SyntacticAnalysis.Nodes;

namespace Core.SyntacticAnalysis.Definitions
{
    public class ClassDefinition : CustomDefinition
    {
        public readonly Dictionary<string, DefineVariableNode> ContainFields = new Dictionary<string, DefineVariableNode>();

        public ClassDefinition(string name, NameSpaceDefinition nameSpace, AccessLevel accessLevel, bool isStatic) :
            base(name, nameSpace, accessLevel, isStatic)
        {
        }

        internal bool AddField(DefineVariableNode fieldDefination)
        {
            if (ContainFields.ContainsKey(fieldDefination.Name)) return false;
            ContainFields.Add(fieldDefination.Name, fieldDefination);
            return true;
        }

        public DefineVariableNode GetField(string name) => ContainFields.ContainsKey(name) ? ContainFields[name] : null;
    }
}