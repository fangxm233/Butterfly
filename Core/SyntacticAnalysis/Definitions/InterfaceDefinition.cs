namespace Core.SyntacticAnalysis.Definitions
{
    public class InterfaceDefinition : CustomDefinition
    {
        public InterfaceDefinition(string name, NameSpaceDefinition nameSpace, AccessLevel accessLevel) :
            base(name, nameSpace, accessLevel, false)
        {
        }
    }
}