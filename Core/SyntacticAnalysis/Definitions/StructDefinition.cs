using System.Collections.Generic;
using Core.LexicalAnalysis;
using Core.SyntacticAnalysis.Nodes;

namespace Core.SyntacticAnalysis.Definitions
{
    public class StructDefinition : CustomDefinition
    {
        private Token _struct;
        private Token _name;
        private Token _colon;
        private Token _inheritanceToken;
        private Token _openBrace;
        private Token _closeBrace;

        public readonly Dictionary<string, DefineVariableNode> ContainFields = new Dictionary<string, DefineVariableNode>();

        public StructDefinition(Token @struct, Token name, Token colon, Token inheritanceToken, 
            NameSpaceDefinition nameSpace, AccessLevel accessLevel) : base(name.Content, nameSpace, accessLevel, false)
        {
            _struct = @struct;
            _name = name;
            _colon = colon;
            _inheritanceToken = inheritanceToken;
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