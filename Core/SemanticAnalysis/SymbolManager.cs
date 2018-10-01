using System.Collections.Generic;
using System.Linq;
using Core.SyntacticAnalysis.Definitions;
using Core.SyntacticAnalysis.Nodes;

namespace Core.SemanticAnalysis
{
    internal class SymbolManager
    {
        private List<Dictionary<string, DefineVariableNode>> _stack =
            new List<Dictionary<string, DefineVariableNode>>();
        private List<Dictionary<string, CustomDefinition>> _referSymbols =
            new List<Dictionary<string, CustomDefinition>>();
        private Dictionary<string, CustomDefinition> _aliases = new Dictionary<string, CustomDefinition>();
        private Dictionary<string, CustomDefinition> _defenitions = new Dictionary<string, CustomDefinition>();

        public void PushList() => _stack.Add(new Dictionary<string, DefineVariableNode>());
        public void PopList() => _stack.RemoveAt(_stack.Count - 1);

        public void Push(DefineVariableNode variable) => _stack.Last().Add(variable.Name, variable);

        public DefineVariableNode this[string name]
        {
            get
            {
                for (int i = _stack.Count - 1; i >= 0; i--)
                    if (_stack[i].ContainsKey(name)) return _stack[i][name];
                return null;
            }
        }
        public CustomDefinition GetDefinition(string name)
        {
            if (_defenitions.ContainsKey(name)) return _defenitions[name];
            if (_aliases.ContainsKey(name)) return _aliases[name];
            CustomDefinition definition = null;
            foreach (Dictionary<string, CustomDefinition> customDefinitions in _referSymbols)
                if (customDefinitions.ContainsKey(name))
                    if (definition == null) definition = customDefinitions[name];
                    else return null;
            return definition;
        }

        public void AddAliase(string name, CustomDefinition definition) => _aliases.Add(name, definition);
        public void AddReference(Dictionary<string, CustomDefinition> reference) => _referSymbols.Add(reference);
        public void Add(CustomDefinition definition) => _defenitions.Add(definition.Name, definition);

        public bool Contain(string name) => this[name] != null;

        public void Clear()
        {
            _referSymbols = new List<Dictionary<string, CustomDefinition>>();
            _stack = new List<Dictionary<string, DefineVariableNode>>();
            _aliases = new Dictionary<string, CustomDefinition>();
            _defenitions = new Dictionary<string, CustomDefinition>();
        }

        public bool CanImplicitCast(CustomDefinition previous, CustomDefinition castType)
        {
            if (previous.FullName == castType.FullName) return true;
            if (previous.Name == "byte" && castType.Name == "int") return true;
            if (previous.Name == "byte" && castType.Name == "float") return true;
            if (previous.Name == "char" && castType.Name == "int") return true;
            if (previous.Name == "char" && castType.Name == "float") return true;
            if (previous.Name == "int" && castType.Name == "float") return true;
            return IsInherit(previous, castType);
        }

        public bool CanBeCastInto(CustomDefinition previous, CustomDefinition castType)
        {
            if ((previous.Name == "int" || previous.Name == "float" || previous.Name == "char" ||
                 previous.Name == "byte") && (castType.Name == "int" || castType.Name == "float" ||
                                              castType.Name == "char" || castType.Name == "byte"))
                return true;
            return CanImplicitCast(previous, castType) || IsInherit(castType, previous);
        } 

        public bool IsNumberic(CustomDefinition definition) =>
            definition.Name == "int" || definition.Name == "float" || definition.Name == "char";

        public bool IsInherit(CustomDefinition subclass, CustomDefinition baseDef)
        {
            if (subclass is InterfaceDefinition) return false;
            if (baseDef.InheritanceType != null && baseDef.InheritanceType.Name == "object") return true;
            while (!(subclass is InterfaceDefinition) && subclass.InheritanceName != null)
            {
                if (subclass.InheritanceType.FullName == baseDef.FullName) return true;
                subclass = subclass.InheritanceType;
            }
            return false;
        }
    }
}