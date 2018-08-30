using System.Collections.Generic;

namespace Compiler.SyntacticAnalysis.Definitions
{
    public class NameSpaceDefinition
    {
        public string Name;
        public NameSpaceDefinition PreviousDefinitions;
        public List<NameSpaceDefinition> ContainNameSpaces;
        public List<ClassDefinition> ContainClasses;
    }
}