using System.Collections.Generic;

namespace Core.SyntacticAnalysis.Definitions
{
    public class NameSpaceDefinition
    {
        public string Name { get; internal set; }
        public NameSpaceDefinition PreviousNameSpace { get; internal set; }
        private readonly Dictionary<string, NameSpaceDefinition> _containNameSpaces = new Dictionary<string, NameSpaceDefinition>();
        private readonly Dictionary<string, CustomDefinition> _containStructures = new Dictionary<string, CustomDefinition>();

        public NameSpaceDefinition(string name)
        {
            Name = name;
        }

        internal void AddAfter(NameSpaceDefinition nameSpaceDefinition)
        {
            _containNameSpaces.Add(nameSpaceDefinition.Name,nameSpaceDefinition);
            nameSpaceDefinition.PreviousNameSpace = this;
        }
        internal void AddAfter(string name)
        {
            NameSpaceDefinition definition = new NameSpaceDefinition(name);
            _containNameSpaces.Add(name, definition);
            definition.PreviousNameSpace = this;
        }

        internal bool AddStructure(CustomDefinition classDefinition)
        {
            if (_containStructures.ContainsKey(classDefinition.Name)) return false;
            _containStructures.Add(classDefinition.Name, classDefinition);
            return true;
        }
        internal bool AddNameSpace(NameSpaceDefinition nameSpaceDefinition)
        {
            if (_containNameSpaces.ContainsKey(nameSpaceDefinition.Name)) return false;
            _containNameSpaces.Add(nameSpaceDefinition.Name, nameSpaceDefinition);
            return true;
        }

        public bool IsContain(CustomDefinition classDefinition) => _containStructures.ContainsKey(classDefinition.Name);
        public bool IsContain(NameSpaceDefinition nameSpaceDefinition) => _containNameSpaces.ContainsKey(nameSpaceDefinition.Name);
        public bool IsContainNameSpace(string name) => _containNameSpaces.ContainsKey(name);
        public bool IsContainCustomDefinition(string name) => _containStructures.ContainsKey(name);

        public CustomDefinition GetClassDefinition(string name) => _containStructures[name];
        public NameSpaceDefinition GetNameSpaceDefinition(string name) => _containNameSpaces[name];

        public override string ToString() => PreviousNameSpace == null ? Name : PreviousNameSpace + "." + Name;
    }
}