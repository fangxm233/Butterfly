using System;
using System.Collections.Generic;
using Core.LexicalAnalysis;
using Core.SyntacticAnalysis.Definitions;
using Core.SyntacticAnalysis.Nodes;

namespace Core.SemanticAnalysis
{
/* 未完成
 * 表达式类型推断
 * 命令作用语句判断
 * 函数返回判断
 * 各种语句判断
 */

    public class SemanticAnalyzer
    {
        internal static SymbolManager Symbols = new SymbolManager();
        private static int _fileIndex;
        private static CustomDefinition _analyzingStructure;
        private static FunctionDefinition _analyzingFunction;

        public static void Analyze()
        {
            
        }

        private static void AnalyzeCusDef(CustomDefinition definition)
        {
            _analyzingStructure = definition;
            Symbols.PushList();
            if (definition is ClassDefinition classDefinition) AnalyzeClass(classDefinition);
            if (definition is StructDefinition structDefinition) AnalyzeStruct(structDefinition);
            foreach (KeyValuePair<string, FunctionDefinition> function in definition.ContainFunctions)
                AnalyzeFunction(function.Value);
            Symbols.PopList();
            _analyzingStructure = null;
        }

        private static void AnalyzeClass(ClassDefinition definition)
        {
            foreach (KeyValuePair<string, DefineVariableNode> field in definition.ContainFields)
            {
                field.Value.Type = Symbols.GetDefinition(field.Value.TypeName);
                Symbols.Push(field.Value);
            }
        }

        private static void AnalyzeStruct(StructDefinition definition)
        {
            foreach (KeyValuePair<string, DefineVariableNode> field in definition.ContainFields)
            {
                field.Value.Type = Symbols.GetDefinition(field.Value.TypeName);
                Symbols.Push(field.Value);
            }
        }

        private static void AnalyzeFunction(FunctionDefinition function)
        {
            _analyzingFunction = function;
            Symbols.PushList();
            foreach (DefineVariableNode defineVariableNode in function.ParmDefinition)
                AnalyzeVarDef(defineVariableNode);
            function.ReturnType = Symbols.GetDefinition(function.ReturnTypeName);
            AnalyzeChunk(function.ChunkNode, false);
            Symbols.PopList();
            _analyzingFunction = null;
        }

        private static void AnalyzeVarDef(DefineVariableNode variable)
        {
            Symbols.Push(variable);
            variable.Type = Symbols.GetDefinition(variable.TypeName);
        }

        private static void AnalyzeChunk(ChunkNode chunk, bool newList = true)
        {
            if(newList)Symbols.PushList();

            if(newList)Symbols.PopList();
        }

        private static void AnalyzeIf(IfNode ifNode)
        {
            AnalyzeExpression(ifNode.Condition);
            AnalyzeChunk(ifNode.Chunk);
            if (ifNode.Else != null) AnalyzeChunk(ifNode.Else);
        }

        private static void AnalyzeAssign(AssignNode assignNode)
        {
            
        }

        private static void AnalyzeExpression(ExpressionNode expression)
        {
            if (expression.NodeType == NodeType.Operate) AnalyzeOperate((OperateNode) expression);
            if (expression.NodeType == NodeType.Element) AnalyzeElement((ElementNode) expression);
        }

        private static void AnalyzeOperate(OperateNode operate)
        {
            switch (operate.OperateType)
            {
                case OperateType.UMinus:
                    AnalyzeExpression(operate.Right);
                    if (operate.Right.Type.Name != "int" || operate.Right.Type.Name != "float") return; //TODO:报错 运算符 "" 不能用于 "" 类型的操作数
                    operate.Type = operate.Right.Type;
                    break;
                case OperateType.Not:
                    AnalyzeExpression(operate.Right);
                    if (operate.Right.Type.Name != "bool") return; //TODO:报错 运算符 "" 不能用于 "" 类型的操作数
                    operate.Type = operate.Right.Type;
                    break;
                case OperateType.Cast:
                    AnalyzeExpression(operate.Right);
                    operate.CastType = Symbols.GetDefinition(operate.CastTypeName);
                    if (!Symbols.CanImplicitCast(operate.Right.Type, operate.CastType)) return; //TODO:报错 无法将类型 "" 转换为 ""
                    operate.Type = operate.CastType;
                    break;
                case OperateType.Multiply:
                case OperateType.Divide:
                case OperateType.Modulus:
                case OperateType.Puls:
                case OperateType.Minus:
                    AnalyzeExpression(operate.Left);
                    AnalyzeExpression(operate.Right);
                    if (!Symbols.IsNumberic(operate.Left.Type) || !Symbols.IsNumberic(operate.Right.Type)) return; //TODO:报错 暂不支持运算符重载
                    if (!Symbols.CanImplicitCast(operate.Right.Type, operate.Left.Type))
                        operate.Type = operate.Left.Type;
                    else if (!Symbols.CanImplicitCast(operate.Left.Type, operate.Right.Type)) 
                        operate.Type = operate.Right.Type;
                    else return; //TODO:报错 无法将类型 "" 转换为 ""
                    break;
                case OperateType.Greater:
                case OperateType.GreaterEqual:
                case OperateType.Less:
                case OperateType.LessEqual:
                case OperateType.Equal:
                case OperateType.NotEqual:
                case OperateType.And:
                case OperateType.Or:
                    AnalyzeExpression(operate.Left);
                    AnalyzeExpression(operate.Right);
                    if (operate.Left.Type.Name != "bool" || operate.Right.Type.Name != "bool") return; //TODO:报错 运算符 "" 无法用于类型 "" 和 "" 的操作数
                    operate.Type = operate.Left.Type;
                    break;
            }
        }

        private static void AnalyzeElement(ElementNode element)
        {
            switch (element.ElemtntType)
            {
                case ElemtntType.Unknown:
                    return; //TODO:报错 未知错误
                case ElemtntType.Expression:
                    AnalyzeExpression(element);
                    break;
                case ElemtntType.Integer:
                    element.Type = Symbols.GetDefinition("int");
                    break;
                case ElemtntType.Float:
                    element.Type = Symbols.GetDefinition("float");
                    break;
                case ElemtntType.String:
                    element.Type = Symbols.GetDefinition("string");
                    break;
                case ElemtntType.Char:
                    element.Type = Symbols.GetDefinition("char");
                    break;
                case ElemtntType.Boolean:
                    element.Type = Symbols.GetDefinition("bool");
                    break;
                case ElemtntType.New:
                    NewNode newNode = (NewNode)element;
                    foreach (ExpressionNode expressionNode in newNode.Parms)
                        AnalyzeExpression(expressionNode);
                    newNode.Type = Symbols.GetDefinition(newNode.TypeName);
                    if (newNode.Type is InterfaceDefinition) return; //TODO:报错 无法创建接口 "" 的实例
                    if (!newNode.Type.Contain(newNode.NameWithParms)) return; //TODO:报错 当前上文中不存在函数 ""
                    newNode.Constructor = newNode.Type.GetFunctionDefinition(newNode.NameWithParms);
                    if (newNode.NextElement != null) AnalyzeNextElement(newNode.NextElement, element.Type);
                    break;
                case ElemtntType.Variable:
                    if (Symbols.Contain(element.Content))
                    {
                        DefineVariableNode variable = Symbols[element.Content];
                        if (_analyzingFunction.IsStatic && !variable.IsStatic)
                            return; //TODO:报错 对象引用对于非静态的字段、方法或属性 "" 是必须的
                        element.Definition = variable;
                        element.Type = element.Definition.Type;
                        if (element.NextElement != null) AnalyzeNextElement(element.NextElement, element.Type);
                        break;
                    }
                    CustomDefinition definition = Symbols.GetDefinition(element.Content);
                    if (definition == null) return; //TODO:报错 当前上文中不存在符号
                    element.ElemtntType = ElemtntType.CustomType;
                    element.Type = definition;
                    if (element.NextElement == null) return; //TODO:报错 "" 是个类型，在给定的上下文中无效
                    AnalyzeNextElement(element.NextElement, element.Type, true);
                    break;
                case ElemtntType.Invoker:
                    InvokerNode invoker = (InvokerNode) element;
                    foreach (ExpressionNode expressionNode in invoker.Parms)
                        AnalyzeExpression(expressionNode);
                    if (!_analyzingStructure.Contain(invoker.NameWithParms)) return; //TODO:报错 当前上文中不存在函数 ""
                    FunctionDefinition function = _analyzingStructure.GetFunctionDefinition(invoker.NameWithParms);
                    if (_analyzingFunction.IsStatic && !function.IsStatic) return; //TODO:报错 对象引用对于非静态的字段、方法或属性 "" 是必须的
                    invoker.Function = function;
                    if(invoker.NextElement!=null) AnalyzeNextElement(invoker.NextElement, element.Type);
                    break;
                case ElemtntType.Array:
                    if (!Symbols.Contain(element.Content)) return; //TODO:报错 当前上文中不存在标识符 ""
                    DefineVariableNode variableNode = Symbols[element.Content];
                    if (_analyzingFunction.IsStatic && !variableNode.IsStatic)
                        return; //TODO:报错 对象引用对于非静态的字段、方法或属性 "" 是必须的
                    element.Definition = Symbols[element.Content];
                    element.Type = element.Definition.Type;
                    if (element.NextElement != null) AnalyzeNextElement(element.NextElement, element.Type);
                    break;
            }
        }

        private static void AnalyzeNextElement(ElementNode element, CustomDefinition enviroument, bool isStatic = false)
        {
            switch (element.ElemtntType)
            {
                case ElemtntType.Variable:
                    DefineVariableNode variable = null;
                    if (enviroument is ClassDefinition classDefinition) variable = classDefinition.GetField(element.Content);
                    if (enviroument is StructDefinition structDefinition) variable = structDefinition.GetField(element.Content);
                    if (enviroument is InterfaceDefinition) return; //TODO:报错 接口不存在字段
                    if (variable == null) return; //TODO:报错 类型 "" 中不存在 "" 的定义
                    if (isStatic && !variable.IsStatic)
                        return; //TODO:报错 对象引用对于非静态的字段、方法或属性 "" 是必须的
                    element.Definition = variable;
                    element.Type = element.Definition.Type;
                    if (element.NextElement != null) AnalyzeNextElement(element.NextElement, element.Type);
                    break;
                case ElemtntType.Invoker:
                    if (!enviroument.Contain(element.Content)) return; //TODO:报错 当前上文中不存在函数 ""
                    InvokerNode invoker = (InvokerNode)element;
                    foreach (ExpressionNode expressionNode in invoker.Parms)
                        AnalyzeExpression(expressionNode);
                    FunctionDefinition function = enviroument.GetFunctionDefinition(invoker.NameWithParms);
                    if (isStatic && !function.IsStatic) return; //TODO:报错 对象引用对于非静态的字段、方法或属性 "" 是必须的
                    invoker.Function = function;
                    if (invoker.NextElement != null) AnalyzeNextElement(invoker.NextElement, element.Type);
                    break;
                case ElemtntType.Array:
                    if (!Symbols.Contain(element.Content)) return; //TODO:报错 当前上文中不存在标识符 ""
                    DefineVariableNode variableNode = null;
                    if (enviroument is ClassDefinition classDefinition1) variableNode = classDefinition1.GetField(element.Content);
                    if (enviroument is StructDefinition structDefinition1) variableNode = structDefinition1.GetField(element.Content);
                    if (enviroument is InterfaceDefinition) return; //TODO:报错 接口不存在字段
                    if (variableNode == null) return; //TODO:报错 类型 "" 中不存在 "" 的定义
                    if (isStatic && !variableNode.IsStatic)
                        return; //TODO:报错 对象引用对于非静态的字段、方法或属性 "" 是必须的
                    element.Definition = Symbols[element.Content];
                    element.Type = element.Definition.Type;
                    if (element.NextElement != null) AnalyzeNextElement(element.NextElement, element.Type);
                    break;
            }
        }
    }
}