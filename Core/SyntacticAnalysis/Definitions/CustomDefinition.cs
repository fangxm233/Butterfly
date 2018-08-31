using System.Collections.Generic;

namespace Core.SyntacticAnalysis.Definitions
{
    public class CustomDefinition
    {
        public CustomDefinition(string name, NameSpaceDefinition nameSpace, AccessLevel accessLevel, bool isStatic)
        {
            Name = name;
            NameSpace = nameSpace;
            AccessLevel = accessLevel;
            IsStatic = isStatic;
        }

        public string Name { get; internal set; }
        public string FullName
        {
            get {
                if (_fullName != null) return _fullName;
                _fullName = ToString();
                return _fullName;
            }
        }
        public NameSpaceDefinition NameSpace { get; internal set; }
        public AccessLevel AccessLevel { get; internal set; }
        public bool IsStatic { get; internal set; }
        public CustomDefinition InheritanceType;
        public readonly Dictionary<string, FunctionDefinition> ContainFunctions = new Dictionary<string, FunctionDefinition>();
        internal string InheritanceName;
        private string _fullName;

        internal bool AddFunction(FunctionDefinition functionDefinition)
        {
            if (ContainFunctions.ContainsKey(functionDefinition.NameWithParms)) return false;
            ContainFunctions.Add(functionDefinition.NameWithParms, functionDefinition);
            return true;
        }

        public FunctionDefinition GetFunctionDefinition(string fullname) => ContainFunctions[fullname];

        public bool Contain(string function) => ContainFunctions.ContainsKey(function);

        public override string ToString() => NameSpace + "." + Name;
    }
}