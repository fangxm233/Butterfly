using Core.LexicalAnalysis;

namespace Core.SyntacticAnalysis.Definitions
{
    public class InterfaceDefinition : CustomDefinition
    {
        private Token _interface;
        private Token _name;
        private Token _colon;
        private Token _inheritanceName;
        private Token _openBrace;
        private Token _closeBrace;

        public InterfaceDefinition(Token @interface, Token name, Token colon, Token inheritanceName,
            NameSpaceDefinition nameSpace, AccessLevel accessLevel) : base(name.Content, nameSpace, accessLevel, false)
        {
            _interface = @interface;
            _name = name;
            _colon = colon;
            _inheritanceName = inheritanceName;
        }

        internal void SetBraces(Token openBrace, Token closeBrace)
        {
            _openBrace = openBrace;
            _closeBrace = closeBrace;
        }
    }
}