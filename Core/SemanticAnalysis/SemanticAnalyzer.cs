using System;
using System.Collections.Generic;
using Core.LexicalAnalysis;
using Core.SyntacticAnalysis.Definitions;
using Core.SyntacticAnalysis.Nodes;

namespace Core.SemanticAnalysis
{
/* 未完成
 * 表达式类型推断
 * 目标函数判断
 * 引用有效判断
 * 命令作用语句判断
 * 函数返回判断
 * 各种语句判断
 */

    public class SemanticAnalyzer
    {
        internal static SymbolStack Symbols = new SymbolStack();
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
            function.Structure = _analyzingStructure;
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

        private static void AnalyzeIf()
        {
            
        }

        private static void AnalyzeExpression(ExpressionNode expression)
        {
            if (expression.NodeType == NodeType.Operate) AnalyzeOperate((OperateNode) expression);
            if (expression.NodeType == NodeType.Element) AnalyzeElement((ElementNode) expression);
        }

        private static void AnalyzeOperate(OperateNode operate)
        {
            
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
                case ElemtntType.Float:
                case ElemtntType.String:
                case ElemtntType.Char:
                case ElemtntType.Boolean:
                    break;
                case ElemtntType.Variable:
                    if (Symbols.Contain(element.Content))
                    {
                        DefineVariableNode variable = Symbols[element.Content];
                        if (_analyzingFunction.IsStatic && !variable.IsStatic) return; //TODO:报错 对象引用对于非静态的字段、方法或属性 "" 是必须的
                        element.Definition = Symbols[element.Content];
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
                    if (!_analyzingStructure.Contain(element.Content)) return; //TODO:报错 当前上文中不存在函数 ""
                    InvokerNode invoker = (InvokerNode) element;
                    foreach (ExpressionNode expressionNode in invoker.Parms)
                        AnalyzeExpression(expressionNode);
                    FunctionDefinition function = _analyzingStructure.GetFunctionDefinition(invoker.NameWithParms);
                    if (_analyzingFunction.IsStatic && !function.IsStatic) return; //TODO:报错 对象引用对于非静态的字段、方法或属性 "" 是必须的
                    invoker.Function = function;
                    break;
                case ElemtntType.Array:
                    break;
                case ElemtntType.This:
                    break;
                case ElemtntType.Null:
                    break;
            }
        }

        private static void AnalyzeNextElement(ElementNode element, CustomDefinition enviroument, bool isStatic = false)
        {
            switch (element.ElemtntType)
            {
                case ElemtntType.Variable:
                    if (Symbols.Contain(element.Content))
                    {
                        element.Definition = Symbols[element.Content];
                        element.Type = element.Definition.Type;
                        if (element.NextElement != null) AnalyzeNextElement(element.NextElement, element.Type);
                        break;
                    }
                    CustomDefinition definition = Symbols.GetDefinition(element.Content);
                    if (definition == null) return; //TODO:报错 当前上文中不存在符号
                    element.ElemtntType = ElemtntType.CustomType;
                    element.Type = definition;
                    if (element.NextElement != null) AnalyzeNextElement(element.NextElement, element.Type);
                    break;
                case ElemtntType.Invoker:
                    break;
                case ElemtntType.Array:
                    break;
                case ElemtntType.This:
                    break;
            }
        }
    }
}