using System;
using System.Collections.Generic;
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

        public static void Analyze()
        {
            
        }

        private static void AnalyzeCusDef(CustomDefinition definition)
        {
            Symbols.PushList();
            if (definition is ClassDefinition classDefinition) AnalyzeClass(classDefinition);
            if (definition is StructDefinition structDefinition) AnalyzeStruct(structDefinition);
            foreach (KeyValuePair<string, FunctionDefinition> function in definition.ContainFunctions)
                AnalyzeFunction(function.Value);
            Symbols.PopList();
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
            Symbols.PushList();
            foreach (DefineVariableNode defineVariableNode in function.ParmDefinition)
                AnalyzeVarDef(defineVariableNode);
            function.ReturnType = Symbols.GetDefinition(function.ReturnTypeName);
            AnalyzeChunk(function.ChunkNode, false);
            Symbols.PopList();
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
                case ElemtntType.Variable:
                    if (Symbols.Contain(element.Content))
                    {
                        element.Definition = Symbols[element.Content];
                        element.Type = element.Definition.Type;
                        if (element.NextElement != null) AnalyzeNextElement(element.NextElement);
                        break;
                    }
                    CustomDefinition definition = Symbols.GetDefinition(element.Content);
                    if (definition == null) return; //TODO:报错 当前上文中不存在符号
                    element.ElemtntType = ElemtntType.CustomType;
                    element.Type = definition;
                    if (element.NextElement != null) AnalyzeNextElement(element.NextElement);
                    break;
                case ElemtntType.Invoker:
                    break;
                case ElemtntType.Array:
                    break;
                case ElemtntType.This:
                    break;
                case ElemtntType.Null:
                    break;
            }
        }

        private static void AnalyzeNextElement(ElementNode element)
        {
            
        }
    }
}