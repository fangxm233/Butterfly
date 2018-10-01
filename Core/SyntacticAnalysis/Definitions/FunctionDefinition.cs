﻿using System.Collections.Generic;
using System.Linq;
using Core.SyntacticAnalysis.Nodes;

namespace Core.SyntacticAnalysis.Definitions
{
    public class FunctionDefinition
    {
        public FunctionDefinition(string name, AccessLevel accessLevel, bool isStatic)
        {
            Name = name;
            AccessLevel = accessLevel;
            IsStatic = isStatic;
            NameWithParms = name;
        }

        public string Name { get; internal set; }
        public string NameWithParms { get; internal set; }
        public CustomDefinition Structure { get; internal set; }
        public CustomDefinition ReturnType { get; internal set; }
        public List<DefineVariableNode> ParmDefinition { get; internal set; } = new List<DefineVariableNode>();
        public ChunkNode ChunkNode { get; internal set; }
        public AccessLevel AccessLevel { get; internal set; }
        public bool IsStatic { get; internal set; }
        internal string ReturnTypeName = "void";

        internal void AddParm(DefineVariableNode variable)
        {
            ParmDefinition.Add(variable);
            if (Structure.Name == Name)
            {
                NameWithParms = IsStatic ? ".cctor" : ".ctor";
                NameWithParms = ParmDefinition.Aggregate(NameWithParms,
                    (current, defineVariableNode) => current + "#" + variable.TypeName);
                return;
            }
            NameWithParms += "#" + variable.TypeName;
        }

        public override string ToString() => Structure + "::" + Name;
    }
}