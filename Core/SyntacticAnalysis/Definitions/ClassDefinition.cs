using System.Collections.Generic;
using Core.LexicalAnalysis;
using Core.SyntacticAnalysis.Nodes;

namespace Core.SyntacticAnalysis.Definitions
{
    public class ClassDefinition : CustomDefinition
    {
        private Token _class;
        private Token _name;
        private Token _colon;
        private Token _inheritanceToken;
        private Token _openBrace;
        private Token _closeBrace;

        public readonly Dictionary<string, DefineVariableNode> ContainFields = new Dictionary<string, DefineVariableNode>();

        public ClassDefinition(Token @class, Token name, Token colon, Token inheritanceToken, string inheritanceName,
            NameSpaceDefinition nameSpace, AccessLevel accessLevel, bool isStatic) : base(name.Content, nameSpace, accessLevel, isStatic)
        {
            _class = @class;
            _name = name;
            _colon = colon;
            _inheritanceToken = inheritanceToken;
            InheritanceName = inheritanceName;
        }

        internal void SetBraces(Token openBrace, Token closeBrace)
        {
            _openBrace = openBrace;
            _closeBrace = closeBrace;
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