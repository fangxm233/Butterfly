using System.Collections.Generic;
using System.Linq;
using Core.LexicalAnalysis;
using Core.SyntacticAnalysis.Nodes;

namespace Core.SyntacticAnalysis.Definitions
{
    public class FunctionDefinition
    {
        private Token _name;
        private Token _openParen;
        private Token _closeParen;
        private Token _colon;
        private Token _returnToken;
        private Token _openBrace;
        private Token _closeBrace;

        public FunctionDefinition(Token name, Token openParen, Token closeParen, Token colon, 
            Token returnToken, string returnName, Token openBrace, Token closeBrace, 
            CustomDefinition structure,
            List<DefineVariableNode> parmDefinition, ChunkNode chunkNode, AccessLevel accessLevel, bool isStatic)
        {
            _name = name;
            _openParen = openParen;
            _closeParen = closeParen;
            _colon = colon;
            _returnToken = returnToken;
            ReturnTypeName = returnName;
            _openBrace = openBrace;
            _closeBrace = closeBrace;
            Structure = structure;
            ParmDefinition = parmDefinition;
            ChunkNode = chunkNode;
            AccessLevel = accessLevel;
            IsStatic = isStatic;
            Name = name.Content;

            if (Structure.Name == Name)
            {
                NameWithParms = IsStatic ? ".cctor" : ".ctor";
                NameWithParms = ParmDefinition.Aggregate(NameWithParms,
                    (current, defineVariableNode) => current + "#" + defineVariableNode.TypeName);
                return;
            }
            NameWithParms = Name;
            NameWithParms = ParmDefinition.Aggregate(NameWithParms,
                    (current, defineVariableNode) => current + "#" + defineVariableNode.TypeName);
        }

        public FunctionDefinition(CustomDefinition structure, string name, AccessLevel accessLevel, bool isStatic)
        {
            Name = name;
            AccessLevel = accessLevel;
            IsStatic = isStatic;
            Structure = structure;

            if (Structure.Name == Name)
            {
                NameWithParms = IsStatic ? ".cctor" : ".ctor";
                NameWithParms = ParmDefinition.Aggregate(NameWithParms,
                    (current, defineVariableNode) => current + "#" + defineVariableNode.TypeName);
                return;
            }
            NameWithParms = Name;
            NameWithParms += ParmDefinition.Aggregate(NameWithParms,
                    (current, defineVariableNode) => current + "#" + defineVariableNode.TypeName);
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
                    (current, defineVariableNode) => current + "#" + defineVariableNode.TypeName);
                return;
            }
            NameWithParms += "#" + variable.TypeName;
        }

        public override string ToString() => Structure + "::" + Name;
    }
}