using System.Collections.Generic;

namespace Compiler.SyntacticAnalysis.Definitions
{
    public class StuctDefinition
    {
        public string Name;
        public NameSpaceDefinition PreviousNameSpace;
        public List<FunctionDefinition> ContainFunctions;
        public List<FieldDefination> ContainFields;
    }
}