using System.Collections.Generic;

namespace Core.SyntacticAnalysis.Definitions
{
    public class NameSpaceDefinition
    {
        public string Name { get; internal set; }
        public NameSpaceDefinition PreviousNameSpace { get; internal set; }
        public readonly Dictionary<string, NameSpaceDefinition> ContainNameSpaces = new Dictionary<string, NameSpaceDefinition>();
        public readonly Dictionary<string, CustomDefinition> ContainStructures = new Dictionary<string, CustomDefinition>();

        public NameSpaceDefinition(string name)
        {
            Name = name;
        }

        internal void AddAfter(NameSpaceDefinition nameSpaceDefinition)
        {
            ContainNameSpaces.Add(nameSpaceDefinition.Name,nameSpaceDefinition);
            nameSpaceDefinition.PreviousNameSpace = this;
        }
        internal void AddAfter(string name)
        {
            NameSpaceDefinition definition = new NameSpaceDefinition(name);
            ContainNameSpaces.Add(name, definition);
            definition.PreviousNameSpace = this;
        }

        internal bool AddStructure(CustomDefinition classDefinition)
        {
            if (ContainStructures.ContainsKey(classDefinition.Name)) return false;
            ContainStructures.Add(classDefinition.Name, classDefinition);
            return true;
        }
        internal bool AddNameSpace(NameSpaceDefinition nameSpaceDefinition)
        {
            if (ContainNameSpaces.ContainsKey(nameSpaceDefinition.Name)) return false;
            ContainNameSpaces.Add(nameSpaceDefinition.Name, nameSpaceDefinition);
            return true;
        }

        public bool IsContain(CustomDefinition classDefinition) => ContainStructures.ContainsKey(classDefinition.Name);
        public bool IsContain(NameSpaceDefinition nameSpaceDefinition) => ContainNameSpaces.ContainsKey(nameSpaceDefinition.Name);
        public bool IsContainNameSpace(string name) => ContainNameSpaces.ContainsKey(name);
        public bool IsContainCustomDefinition(string name) => ContainStructures.ContainsKey(name);

        public CustomDefinition GetClassDefinition(string name) => ContainStructures[name];
        public NameSpaceDefinition GetNameSpaceDefinition(string name) => ContainNameSpaces[name];

        public override string ToString() => PreviousNameSpace == null ? Name : PreviousNameSpace + "." + Name;
    }
}