using System.Collections.Generic;

namespace Compiler.SyntacticAnalysis.Definitions
{
    public class FunctionDefinition
    {
        public string Name;
        public ClassDefinition Class;
        public ClassDefinition ReturnType;
        public List<ClassDefinition> ParmTypes;
        public List<string> ParmNames;
    }
}