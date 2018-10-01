using System;
using System.Collections.Generic;
using Core.LexicalAnalysis;
using Core.SyntacticAnalysis;
using Core.SyntacticAnalysis.Definitions;
using Core.SyntacticAnalysis.Nodes;

namespace Core.SemanticAnalysis
{
    /* 未完成
     * 继承
     * 访问修饰
     * 构造函数
     * 
     */

    //需要支持的错误
    //TODO:支持 涉及 "" 和 "" 的循环基类依赖项 CS0146
    //TODO:支持 ""：并非所有的代码路径都返回值 CS0161
    //TODO:支持 使用了未赋值的局部变量 "" CS0165
    //TODO:支持 在给"this"对象的所有字段赋值之前，无法使用该对象 CS0188
    //TODO:支持 只有 assignment、call、/*increment、decrement*/ 和 new 对象表达式可用作语句 CS0201
    //TODO:支持 "" 和 "" 之间具有二义性 CS0229
    //TODO:支持 命名空间 "" 中不存在类型或命名空间名 ""（是否缺少程序集引用？） CS0234
    //TODO:支持 无法修改取消装箱转换的结果 CS0445
    //TODO:支持 ""：重写 "" 继承成员 "" 时不能更改访问修饰符 CS0507
    //TODO:支持 "" 类型的结构成员 "" 在结构布局中导致循环 CS0523
    //TODO:支持 在控件返回调用方之前，自动实现的属性"name"的支持字段必须完全赋值 CS0843
    //TODO:支持 应输入关键字"this"或"base" CS1018
    //TODO:支持 无法修改 "" 的返回值，因为它不是变量 CS1612

    public class SemanticAnalyzer
    {
        internal static readonly SymbolManager Symbols = new SymbolManager();
        private static int _fileIndex;
        private static CustomDefinition _analyzingStructure;
        private static FunctionDefinition _analyzingFunction;
        private static readonly Stack<AnalysisNode> s_loopStack = new Stack<AnalysisNode>();

        public static void Analyze()
        {
            for (; _fileIndex < Lexer.FileCount; _fileIndex++)
                GenerateMetadata(_fileIndex);
            for (_fileIndex = 0; _fileIndex < Lexer.FileCount; _fileIndex++)
            {
                foreach (UsingNode usingNode in Parser.FilesReferences[_fileIndex])
                    Symbols.AddReference(usingNode.GetLast().NameSpace.ContainStructures);
                foreach (KeyValuePair<string, DefSpecifierNode> defSpecifierNode in Parser.FilesAliases[_fileIndex])
                    Symbols.AddAliase(defSpecifierNode.Key, GetDefinition(defSpecifierNode.Value));
                foreach (CustomDefinition definition in Parser.FilesDefinitions[_fileIndex])
                    Symbols.Add(definition);
                foreach (CustomDefinition customDefinition in Parser.FilesDefinitions[_fileIndex])
                    AnalyzeStructure(customDefinition);
            }
        }

        private static void GenerateMetadata(int fileIndex)
        {
            foreach (UsingNode usingNode in Parser.FilesReferences[fileIndex])
            {
                AnalyzeUsing(usingNode);
                Symbols.AddReference(usingNode.GetLast().NameSpace.ContainStructures);
            }
            foreach (CustomDefinition definition in Parser.FilesDefinitions[fileIndex])
                Symbols.Add(definition);
            foreach (CustomDefinition definition in Parser.FilesDefinitions[fileIndex])
            {
                if (definition is ClassDefinition classDefinition) GenerateClassMetadata(classDefinition);
                if (definition is StructDefinition structDefinition) GenerateStructMetadata(structDefinition);
            }
            foreach (CustomDefinition definition in Parser.FilesDefinitions[fileIndex])
                foreach (KeyValuePair<string, FunctionDefinition> function in definition.ContainFunctions)
                    GenerateFuncMetadata(function.Value);
            Symbols.Clear();
        }

        //private static void GenerateStructureMetadata(CustomDefinition definition)
        //{
        //    if (definition is ClassDefinition classDefinition) GenerateClassMetadata(classDefinition);
        //    if (definition is StructDefinition structDefinition) GenerateStructMetadata(structDefinition);
        //    foreach (KeyValuePair<string, FunctionDefinition> function in definition.ContainFunctions)
        //        GenerateFuncMetadata(function.Value);
        //}

        private static void GenerateClassMetadata(ClassDefinition definition)
        {
            foreach (KeyValuePair<string, DefineVariableNode> field in definition.ContainFields)
                AnalyzeVarDef(field.Value, false);
        }

        private static void GenerateStructMetadata(StructDefinition definition)
        {
            foreach (KeyValuePair<string, DefineVariableNode> field in definition.ContainFields)
                AnalyzeVarDef(field.Value, false);
        }

        private static void GenerateFuncMetadata(FunctionDefinition function)
        {
            foreach (DefineVariableNode defineVariableNode in function.ParmDefinition)
                AnalyzeVarDef(defineVariableNode, false);
            function.ReturnType = Symbols.GetDefinition(function.ReturnTypeName);
            if (function.ReturnType == null) return; //TODO:报错 未能找到类型或命名空间名称 ""（是否缺少 using 指令或程序集引用？）
        }

        private static void AnalyzeStructure(CustomDefinition definition)
        {
            _analyzingStructure = definition;
            Symbols.PushList();
            if (definition is ClassDefinition classDefinition) AnalyzeClass(classDefinition);
            if (definition is StructDefinition structDefinition) AnalyzeStrurt(structDefinition);
            Symbols.PopList();
            _analyzingStructure = null;
        }

        private static void AnalyzeClass(ClassDefinition definition)
        {
            foreach (KeyValuePair<string, DefineVariableNode> field in definition.ContainFields)
                AnalyzeVarDef(field.Value);
            foreach (KeyValuePair<string, FunctionDefinition> function in definition.ContainFunctions)
                AnalyzeFunction(function.Value);
        }

        private static void AnalyzeStrurt(StructDefinition definition)
        {
            foreach (KeyValuePair<string, DefineVariableNode> field in definition.ContainFields)
                AnalyzeVarDef(field.Value);
            foreach (KeyValuePair<string, FunctionDefinition> function in definition.ContainFunctions)
                AnalyzeFunction(function.Value);
        }

        private static void AnalyzeUsing(UsingNode usingNode)
        {
            for (NameSpaceDefinition nameSpace = Parser.RootNameSpace;
                usingNode.NextUsing != null;
                usingNode = usingNode.NextUsing)
            {
                if (!nameSpace.IsContainNameSpace(usingNode.Name)) return; //TODO:报错 未找到命名空间名
                usingNode.NameSpace = nameSpace.GetNameSpaceDefinition(usingNode.Name);
            }
        }

        private static void AnalyzeFunction(FunctionDefinition function)
        {
            _analyzingFunction = function;
            Symbols.PushList();
            foreach (DefineVariableNode defineVariableNode in function.ParmDefinition)
                AnalyzeVarDef(defineVariableNode);
            if (function.ChunkNode != null)
                AnalyzeChunk(function.ChunkNode, false);
            Symbols.PopList();
            _analyzingFunction = null;
        }

        private static void AnalyzeVarDef(DefineVariableNode variable, bool shouldPush = true)
        {
            if (shouldPush) Symbols.Push(variable);
            if (variable.Type != null) return;
            variable.Type = Symbols.GetDefinition(variable.TypeName);
            if (variable.Type == null) return; //TODO:报错 未能找到类型或命名空间名称 ""（是否缺少 using 指令或程序集引用？）
        }

        private static void AnalyzeChunk(ChunkNode chunk, bool newList = true)
        {
            if(newList)Symbols.PushList();
            foreach (AnalysisNode analysisNode in chunk.Nodes)
            {
                switch (analysisNode)
                {
                    case AssignNode assignNode:
                        AnalyzeAssign(assignNode);
                        break;
                    case CommandNode commandNode:
                        AnalyzeCommand(commandNode);
                        break;
                    case DefineVariableNode defineVariableNode:
                        AnalyzeVarDef(defineVariableNode);
                        break;
                    case InvokerNode invokerNode:
                        AnalyzeElement(invokerNode);
                        if (invokerNode.GetLastElement().ElemtntType != ElemtntType.Invoker)
                            return; //TODO:报错 只有assignment、invoker 和 new 对象表达式可用作语句
                        break;
                    case NewNode newNode:
                        AnalyzeElement(newNode);
                        switch (newNode.GetLastElement().ElemtntType)
                        {
                            case ElemtntType.New:
                            case ElemtntType.Invoker:
                                break;
                            default:
                                return; //TODO:报错 只有assignment、invoker 和 new 对象表达式可用作语句
                        }
                        break;
                    case IfNode ifNode:
                        AnalyzeIf(ifNode);
                        break;
                    case ForNode forNode:
                        AnalyzeForNode(forNode);
                        break;
                    case WhileNode whileNode:
                        AnalyzeWhileNode(whileNode);
                        break;
                }
            }
            if (newList)Symbols.PopList();
        }

        private static void AnalyzeIf(IfNode ifNode)
        {
            AnalyzeExpression(ifNode.Condition);
            AnalyzeChunk(ifNode.Chunk);
            if (ifNode.Else != null) AnalyzeChunk(ifNode.Else);
        }

        private static void AnalyzeForNode(ForNode forNode)
        {
            Symbols.PushList();
            s_loopStack.Push(forNode);
            AnalyzeChunk(forNode.Left, false);
            AnalyzeExpression(forNode.Middle);
            AnalyzeChunk(forNode.Right, false);
            AnalyzeChunk(forNode.Chunk, false);
            s_loopStack.Pop();
            Symbols.PopList();
        }

        private static void AnalyzeWhileNode(WhileNode whileNode)
        {
            s_loopStack.Push(whileNode);
            AnalyzeExpression(whileNode.Condition);
            AnalyzeChunk(whileNode.Chunk);
            s_loopStack.Pop();
        }

        private static void AnalyzeAssign(AssignNode assignNode)
        {
            AnalyzeExpression(assignNode.Left);
            AnalyzeExpression(assignNode.Right);
            if (Symbols.CanImplicitCast(assignNode.Right.Type, assignNode.Left.Type)) return;
            //TODO:报错 无法将类型 "" 隐式转换为 ""
        }

        private static void AnalyzeCommand(CommandNode commandNode)
        {
            if (commandNode.Type == CommandType.Return)
            {
                commandNode.Function = _analyzingFunction;
                if (commandNode.Expression == null && _analyzingFunction.ReturnTypeName == "void") return;
                if (commandNode.Expression == null) return; //TODO:报错 需要一个类型可转换为 "" 的对象
                if (commandNode.Expression != null && _analyzingFunction.ReturnTypeName == "void") return; //TODO:报错 由于 "" 返回 void,返回关键字后面不得有对象表达式
                AnalyzeExpression(commandNode.Expression);
                if (!Symbols.CanImplicitCast(commandNode.Expression.Type, _analyzingFunction.ReturnType)) return; //TODO:报错 无法将类型 "" 隐式转换为 ""
                return;
            }
            if (s_loopStack.Count == 0) return; //TODO:报错 没有要中断或继续的封闭循环
            AnalysisNode loop = s_loopStack.Peek();
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
                    if (!Symbols.CanBeCastInto(operate.Right.Type, operate.CastType)) return; //TODO:报错 无法将类型 "" 转换为 ""
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
                    if (Symbols.CanImplicitCast(operate.Right.Type, operate.Left.Type))
                        operate.Type = operate.Left.Type;
                    else if (Symbols.CanImplicitCast(operate.Left.Type, operate.Right.Type)) 
                        operate.Type = operate.Right.Type;
                    else return; //TODO:报错 无法将类型 "" 隐式转换为 ""
                    break;
                case OperateType.Greater:
                case OperateType.GreaterEqual:
                case OperateType.Less:
                case OperateType.LessEqual:
                case OperateType.Equal:
                case OperateType.NotEqual:
                    AnalyzeExpression(operate.Left);
                    AnalyzeExpression(operate.Right);
                    operate.Type = Symbols.GetDefinition("bool");
                    break;
                case OperateType.And:
                case OperateType.Or:
                    AnalyzeExpression(operate.Left);
                    AnalyzeExpression(operate.Right);
                    if (operate.Left.Type.Name != "bool" || operate.Right.Type.Name != "bool") return; //TODO:报错 运算符 "" 无法用于类型 "" 和 "" 的操作数
                    operate.Type = operate.Left.Type;
                    break;
            }
        }

        private static void AnalyzeElement(ElementNode element) //写的不好
        {
            switch (element.ElemtntType)
            {
                case ElemtntType.Unknown:
                    return; //TODO:报错 未知错误
                case ElemtntType.Expression:
                    AnalyzeExpression(element.Expression);
                    element.Type = element.Expression.Type;
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
                    if (newNode.Type == null) return; //TODO:报错 未能找到类型或命名空间名称 ""（是否缺少 using 指令或程序集引用？）
                    if (newNode.Type is InterfaceDefinition) return; //TODO:报错 无法创建接口 "" 的实例
                    if (!newNode.Type.Contain(newNode.NameWithParms)) return; //TODO:报错 当前上文中不存在函数 ""
                    newNode.Constructor = newNode.Type.GetFunctionDefinition(newNode.NameWithParms);
                    if (newNode.NextElement != null) AnalyzeNextElement(newNode.NextElement, element.Type);
                    newNode.Type = newNode.LastElement.Type; //由头一个标识符表示整串标识符的类型
                    break;
                case ElemtntType.Variable:
                    if (Symbols.Contain(element.Content)) //处理标识符表示对象的情况
                    {
                        DefineVariableNode variable = Symbols[element.Content];
                        if (_analyzingFunction.IsStatic && !variable.IsStatic)
                            return; //TODO:报错 对象引用对于非静态的字段、方法或属性 "" 是必须的
                        element.Definition = variable;
                        element.Type = element.Definition.Type;
                        if (element.NextElement != null) AnalyzeNextElement(element.NextElement, element.Type);
                        element.Type = element.LastElement.Type; //由头一个标识符表示整串标识符的类型
                        break;
                    }
                    //处理标识符表示类型名字的情况
                    CustomDefinition definition = Symbols.GetDefinition(element.Content);
                    if (definition == null) return; //TODO:报错 未能找到类型或命名空间名称 ""（是否缺少 using 指令或程序集引用？）
                    element.ElemtntType = ElemtntType.CustomType;
                    element.Type = definition;
                    if (element.NextElement == null) return; //TODO:报错 "" 是个类型，在给定的上下文中无效
                    AnalyzeNextElement(element.NextElement, element.Type, true);
                    element.Type = element.LastElement.Type; //由头一个标识符表示整串标识符的类型
                    break;
                case ElemtntType.Invoker:
                    InvokerNode invoker = (InvokerNode) element;
                    foreach (ExpressionNode expressionNode in invoker.Parms)
                        AnalyzeExpression(expressionNode);
                    if (!_analyzingStructure.Contain(invoker.NameWithParms)) return; //TODO:报错 当前上文中不存在函数 ""
                    FunctionDefinition function = _analyzingStructure.GetFunctionDefinition(invoker.NameWithParms);
                    if (_analyzingFunction.IsStatic && !function.IsStatic) return; //TODO:报错 对象引用对于非静态的字段、方法或属性 "" 是必须的
                    invoker.Function = function;
                    invoker.Type = function.ReturnType;
                    if (invoker.NextElement != null) AnalyzeNextElement(invoker.NextElement, element.Type);
                    invoker.Type = invoker.LastElement.Type; //由头一个标识符表示整串标识符的类型
                    break;
                case ElemtntType.Array:
                    if (!Symbols.Contain(element.Content)) return; //TODO:报错 当前上文中不存在标识符 ""
                    DefineVariableNode variableNode = Symbols[element.Content];
                    if (_analyzingFunction.IsStatic && !variableNode.IsStatic)
                        return; //TODO:报错 对象引用对于非静态的字段、方法或属性 "" 是必须的
                    element.Definition = Symbols[element.Content];
                    element.Type = element.Definition.Type;
                    if (element.NextElement != null) AnalyzeNextElement(element.NextElement, element.Type);
                    element.Type = element.LastElement.Type; //由头一个标识符表示整串标识符的类型
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
                    InvokerNode invoker = (InvokerNode)element;
                    foreach (ExpressionNode expressionNode in invoker.Parms)
                        AnalyzeExpression(expressionNode);
                    if (!enviroument.Contain(invoker.NameWithParms)) return; //TODO:报错 当前上文中不存在函数 ""
                    FunctionDefinition function = enviroument.GetFunctionDefinition(invoker.NameWithParms);
                    if (isStatic && !function.IsStatic) return; //TODO:报错 对象引用对于非静态的字段、方法或属性 "" 是必须的
                    invoker.Function = function;
                    invoker.Type = function.ReturnType;
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

        private static CustomDefinition GetDefinition(DefSpecifierNode defSpecifier)
        {
            for (NameSpaceDefinition nameSpace = Parser.RootNameSpace;
                defSpecifier.NextSpecifier != null;
                defSpecifier = defSpecifier.NextSpecifier)
            {
                if (nameSpace.IsContainCustomDefinition(defSpecifier.Content) &&
                    nameSpace.IsContainNameSpace(defSpecifier.Content))
                    if (defSpecifier.NextSpecifier == null) return nameSpace.GetClassDefinition(defSpecifier.Content);
                if (nameSpace.IsContainCustomDefinition(defSpecifier.Content))
                    return nameSpace.GetClassDefinition(defSpecifier.Content);
                if (nameSpace.IsContainNameSpace(defSpecifier.Content))
                    nameSpace = nameSpace.GetNameSpaceDefinition(defSpecifier.Content);
            }
            return null; //TODO:报错 最后一个不是类型
        }
    }
}