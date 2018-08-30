using System.Collections.Generic;

namespace Compiler.SyntacticAnalysis.Definitions
{
    public class InterfaceDefinition
    {
        public string Name;
        public NameSpaceDefinition PreviousNameSpace;
        public List<FunctionDefinition> ContainFunctions;
    }
}