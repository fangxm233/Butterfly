using Core.LexicalAnalysis;
using Core.SyntacticAnalysis.Definitions;
using Core.SyntacticAnalysis.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.SyntacticAnalysis
{
    internal enum OrderType
    {
        Function,
        Assign,
        Chunk,
        Command,
        DefineVariable,
        LocalVariable,
        DefSpecifier,
        Element,
        For,
        If,
        Invoker,
        New,
        While,

        Count,
    }

    public enum AccessLevel
    {
        Local, Public, Private, Internal,
    }
    
    internal class Parser
    {
        public static List<NameSpaceDefinition>[] FilesReferences;
        public static List<CustomDefinition>[] FilesDefinitions;
        public static Dictionary<string, DefSpecifierNode>[] FilesAliases;
        public static NameSpaceDefinition RootNameSpace;
        public static NameSpaceDefinition EndNameSpace;
        private static CustomDefinition _analyzingStructure;

        private static readonly OrderType[] s_cusDefinitionOrder =  {OrderType.DefineVariable, OrderType.Function};
        private static readonly OrderType[] s_interfaceDefinitionOrder = { OrderType.Function };
        private static readonly OrderType[] s_commonOrder =
        {
            OrderType.DefineVariable, OrderType.Assign, OrderType.If,
            OrderType.While, OrderType.For, OrderType.Chunk,
            OrderType.Command, OrderType.Invoker, OrderType.New
        };

        private static void Init(string outputName)
        {
            RootNameSpace = new NameSpaceDefinition(outputName);
            EndNameSpace = new NameSpaceDefinition(outputName);
            FilesReferences = new List<NameSpaceDefinition>[Lexer.FileCount];
            FilesAliases = new Dictionary<string, DefSpecifierNode>[Lexer.FileCount];
            for (int i = 0; i < Lexer.FileCount; i++)
            {
                FilesReferences[i] = new List<NameSpaceDefinition>();
                FilesAliases[i] = new Dictionary<string, DefSpecifierNode>();
            }
        }

        public static void Match(string outputName)
        {
            Init(outputName);
            while (Lexer.FileIndex < Lexer.FileCount)
            {
                switch (Lexer.NextTokenType)
                {
                    case TokenType.Using:
                        Lexer.Next();
                        MatchUsing();
                        break;
                    case TokenType.NameSpace:
                        Lexer.Next();
                        MatchNameSpace();
                        break;
                    case TokenType.Class:
                        Lexer.Next();
                        MatchClass(AccessLevel.Private, false);
                        break;
                    case TokenType.Struct:
                        Lexer.Next();
                        MatchStruct(AccessLevel.Private, false);
                        break;
                    case TokenType.Interface:
                        Lexer.Next();
                        MatchInterface(AccessLevel.Private);
                        break;
                    case TokenType.Static:
                    case TokenType.Public:
                        AccessLevel accessLevel = MatchAccessLevelAndStatic(out bool isStatic);
                        switch (Lexer.NextTokenType)
                        {
                            case TokenType.Class:
                                Lexer.Next();
                                MatchClass(accessLevel, isStatic);
                                break;
                            case TokenType.Struct:
                                Lexer.Next();
                                MatchStruct(accessLevel, isStatic);
                                break;
                            case TokenType.Interface:
                                Lexer.Next();
                                MatchInterface(accessLevel);
                                break;
                        }
                        break;
                    default:
                        return; //TODO:报错 意外的符号
                }
                if(Lexer.NextTokenType == TokenType.EOF) Console.WriteLine("一个文件编译完成!!");
            }
        }

        private static void MatchUsing()
        {
            List<NameSpaceDefinition> nameSpaces = FilesReferences[Lexer.FileIndex];
            Lexer.Match(TokenType.Identifer); //TODO:使用Match函数报错
            string s = Lexer.NextTokenContent;
            if (Lexer.MatchNext("="))
            {
                Lexer.Next();
                FilesAliases[Lexer.FileIndex].Add(s, MatchDefinitionSpecifier());
                return;
            }
            nameSpaces.Add(new NameSpaceDefinition(Lexer.NextTokenContent));
            NameSpaceDefinition last = nameSpaces.Last();
            while (Lexer.Match("."))
            {
                Lexer.MatchNext(TokenType.Identifer); //TODO:使用Match函数报错
                last.AddAfter(Lexer.NextTokenContent);
                last = last.GetNameSpaceDefinition(Lexer.NextTokenContent);
                Lexer.Next();
            }
            Lexer.MatchNow(";"); //TODO:使用Match函数报错
        }

        private static void MatchNameSpace()
        {
            Lexer.Match(TokenType.Identifer); //TODO:使用Match函数报错
            if (!RootNameSpace.IsContainNameSpace(Lexer.NextTokenContent))
                RootNameSpace.AddAfter(Lexer.NextTokenContent);
            NameSpaceDefinition last = RootNameSpace.GetNameSpaceDefinition(Lexer.NextTokenContent);
            Lexer.Next();
            while (Lexer.Match("."))
            {
                Lexer.MatchNext(TokenType.Identifer); //TODO:使用Match函数报错
                last.AddAfter(Lexer.NextTokenContent);
                last = last.GetNameSpaceDefinition(Lexer.NextTokenContent);
                Lexer.Next();
            }
            EndNameSpace = last;
            Lexer.MatchNow(";"); //TODO:使用Match函数报错
        }

        private static void MatchClass(AccessLevel accessLevel, bool isStatic)
        {
            Lexer.Match(TokenType.Identifer); //TODO:使用Match函数报错
            ClassDefinition classDefinition =
                new ClassDefinition(Lexer.NextTokenContent, EndNameSpace, accessLevel, isStatic);
            if (accessLevel != AccessLevel.Public)
                classDefinition.AccessLevel = AnalysisAccessLevel(Lexer.NextTokenContent);
            if(Lexer.MatchNext(":"))
                if (Lexer.MatchNext(TokenType.Identifer)) //TODO:使用Match函数报错
                {
                    classDefinition.InheritanceName = Lexer.NextTokenContent;
                    Lexer.Next();
                }
                else classDefinition.InheritanceName = "object";
            EndNameSpace.AddStructure(classDefinition);
            FilesDefinitions[Lexer.FileIndex].Add(classDefinition);
            _analyzingStructure = classDefinition;
            classDefinition.AddFunction(new FunctionDefinition(".ctor", classDefinition.AccessLevel, false));
            classDefinition.AddFunction(new FunctionDefinition(".cctor", classDefinition.AccessLevel, true));
            ChunkNode chunk = MatchChunk(s_cusDefinitionOrder);
            foreach (AnalysisNode analysisNode in chunk.Nodes)
            {
                if (analysisNode.NodeType == NodeType.Assign)
                    classDefinition.GetFunctionDefinition(".ctor").ChunkNode.AddNode(analysisNode);
                classDefinition.AddField((DefineVariableNode)analysisNode);
            }
            _analyzingStructure = null;
        }

        private static void MatchStruct(AccessLevel accessLevel, bool isStatic)
        {
            Lexer.MatchNow(TokenType.Identifer); //TODO:使用Match函数报错
            StructDefinition structDefinition =
                new StructDefinition(Lexer.NextTokenContent, EndNameSpace, accessLevel, isStatic);
            if (accessLevel != AccessLevel.Public)
                structDefinition.AccessLevel = AnalysisAccessLevel(Lexer.NextTokenContent);
            if (Lexer.MatchNext(":"))
                if (Lexer.MatchNext(TokenType.Identifer)) //TODO:使用Match函数报错
                {
                    structDefinition.InheritanceName = Lexer.NextTokenContent;
                    Lexer.Next();
                }
            EndNameSpace.AddStructure(structDefinition);
            _analyzingStructure = structDefinition;
            FilesDefinitions[Lexer.FileIndex].Add(structDefinition);
            structDefinition.AddFunction(new FunctionDefinition(".ctor", structDefinition.AccessLevel, false));
            ChunkNode chunk = MatchChunk(s_cusDefinitionOrder);
            foreach (AnalysisNode analysisNode in chunk.Nodes)
                structDefinition.AddField((DefineVariableNode)analysisNode);
            _analyzingStructure = null;
        }

        private static void MatchInterface(AccessLevel accessLevel)
        {
            Lexer.Match(TokenType.Identifer); //TODO:使用Match函数报错
            InterfaceDefinition interfaceDefinition =
                new InterfaceDefinition(Lexer.NextTokenContent, EndNameSpace, accessLevel);
            EndNameSpace.AddStructure(interfaceDefinition);
            FilesDefinitions[Lexer.FileIndex].Add(interfaceDefinition);
            if (accessLevel != AccessLevel.Public)
                interfaceDefinition.AccessLevel = AnalysisAccessLevel(Lexer.NextTokenContent);
            _analyzingStructure = interfaceDefinition;
            Lexer.Next();
            MatchChunk(s_interfaceDefinitionOrder, false, false);
            _analyzingStructure = null;
        }

        private static void MatchFunction(AccessLevel accessLevel, bool isStatic, bool haveDefinition = true)
        {
            Lexer.Match(TokenType.Identifer);
            FunctionDefinition function = new FunctionDefinition(Lexer.NextTokenContent, accessLevel, isStatic);
            function.Structure = _analyzingStructure;

            //参数
            Lexer.MatchNext("(");
            Lexer.Next();
            while (Lexer.NextTokenContent != ")")
            {
                Lexer.Match(TokenType.Identifer); //TODO:使用Match函数报错
                DefineVariableNode variable =
                    MatchDefineVariable(out AssignNode assign, false);
                function.AddParm(variable);
                if (Lexer.NextTokenContent != "," && Lexer.NextTokenContent != ")") return; //TODO:报错 意外的符号
            }

            //返回值
            if (Lexer.MatchNext(":"))
            {
                if (Lexer.MatchNext(TokenType.Identifer)) //TODO:使用Match函数报错
                {
                    function.ReturnTypeName = Lexer.NextTokenContent;
                    Lexer.Next();
                }
            }
            else function.ReturnTypeName = "void";

            if (!haveDefinition)
            {
                if (Lexer.Match("{")) return; //TODO:报错 接口成员不能有定义
                Lexer.MatchNow(";"); //TODO:使用Match函数报错
                return;
            }
            ChunkNode chunkNode = MatchChunk(s_commonOrder);
            function.ChunkNode = chunkNode;
            _analyzingStructure.AddFunction(function);
        }

        private static ChunkNode MatchChunk(OrderType[] order, bool haveAssign = true, bool haveDefinition = true)
        {
            ChunkNode chunk = new ChunkNode();
            bool isMatchOneNode = false;
            if (Lexer.Match("{"))
                Lexer.Next();
            else
                isMatchOneNode = true;
            while (true)
            {
                switch (Lexer.NextTokenType)
                {
                    case TokenType.Static:
                    case TokenType.Public:
                        AccessLevel accessLevel = MatchAccessLevelAndStatic(out bool isStatic);
                        switch (Lexer.NextTokenType)
                        {
                            case TokenType.Func:
                                if (!IsInclude(order, OrderType.Function)) return chunk; //TODO:报错 意外的符号
                                Lexer.Next();
                                MatchFunction(accessLevel, isStatic, haveDefinition);
                                break;
                            case TokenType.Var:
                                if (!IsInclude(order, OrderType.DefineVariable))
                                    return chunk; //TODO:报错 意外的符号
                                Lexer.Next();
                                chunk.AddNode(MatchDefineVariable(out AssignNode assign1, haveAssign, accessLevel,
                                    isStatic));
                                if (assign1 != null) chunk.AddNode(assign1);
                                break;
                        }
                        break;
                    case TokenType.Func:
                        if (!IsInclude(order, OrderType.Function)) return chunk; //TODO:报错 意外的符号
                        Lexer.Next();
                        MatchFunction(AccessLevel.Private, false, haveDefinition);
                        break;
                    case TokenType.Var:
                        if (!IsInclude(order, OrderType.DefineVariable) && !IsInclude(order, OrderType.LocalVariable))
                            return chunk; //TODO:报错 意外的符号
                        Lexer.Next();
                        chunk.AddNode(MatchDefineVariable(out AssignNode assign, haveAssign,
                            IsInclude(order, OrderType.LocalVariable) ? AccessLevel.Local : AccessLevel.Private));
                        if (assign != null) chunk.AddNode(assign);
                        break;
                    case TokenType.Let:
                        if (!IsInclude(order, OrderType.Assign)) return chunk; //TODO:报错 意外的符号
                        Lexer.Next();
                        chunk.AddNode(MatchAssign());
                        break;
                    case TokenType.LBraces:
                        if (!IsInclude(order, OrderType.Chunk)) return chunk; //TODO:报错 意外的符号
                        chunk.AddNode(MatchChunk(s_commonOrder));
                        break;
                    case TokenType.Command:
                        if (!IsInclude(order, OrderType.Command)) return chunk; //TODO:报错 意外的符号
                        chunk.AddNode(MatchCommand());
                        break;
                    case TokenType.For:
                        if (!IsInclude(order, OrderType.For)) return chunk; //TODO:报错 意外的符号
                        chunk.AddNode(MatchFor());
                        break;
                    case TokenType.If:
                        if (!IsInclude(order, OrderType.If)) return chunk; //TODO:报错 意外的符号
                        chunk.AddNode(MatchIf());
                        break;
                    case TokenType.Inv:
                        if (!IsInclude(order, OrderType.Invoker)) return chunk; //TODO:报错 意外的符号
                        Lexer.Next();
                        chunk.AddNode(MatchElement());
                        Lexer.MatchNow(";"); //TODO:使用Match函数报错
                        break;
                    case TokenType.New:
                        if (!IsInclude(order, OrderType.New)) return chunk; //TODO:报错 意外的符号
                        chunk.AddNode(MatchElement());
                        Lexer.MatchNow(";"); //TODO:使用Match函数报错
                        break;
                    case TokenType.While:
                        if (!IsInclude(order, OrderType.While)) return chunk; //TODO:报错 意外的符号
                        chunk.AddNode(MatchWhile());
                        break;
                    default:
                        return chunk; //TODO:报错 意外的符号
                }
                if (isMatchOneNode) return chunk;
                if (Lexer.Match("}"))
                {
                    Lexer.Next();
                    return chunk;
                }
            }
        }

        private static bool IsInclude(OrderType[] orders, OrderType order) =>
            orders.Any(orderType => order == orderType);

        private static AccessLevel MatchAccessLevelAndStatic(out bool isStatic)
        {
            AccessLevel accessLevel = Lexer.Match(TokenType.Public) ? AccessLevel.Public : AccessLevel.Private;
            if (accessLevel == AccessLevel.Public) Lexer.Next();
            isStatic = Lexer.Match(TokenType.Static);
            if (isStatic) Lexer.Next();
            return accessLevel;
        }

        private static IfNode MatchIf()
        {
            Lexer.MatchNow(TokenType.If);
            Lexer.MatchNow("("); //TODO:使用Match函数报错
            ExpressionNode expression = MatchExpression();
            Lexer.MatchNow(")"); //TODO:使用Match函数报错
            IfNode ifNode = new IfNode(expression);
            ChunkNode chunkNode = MatchChunk(s_commonOrder);
            ifNode.Chunk = chunkNode;
            if (Lexer.Match(TokenType.Else))
            {
                ChunkNode elseChunkNode = MatchChunk(s_commonOrder);
                ifNode.Else = elseChunkNode;
            }
            return ifNode;
        }

        private static ForNode MatchFor()
        {
            Lexer.MatchNow(TokenType.For);
            ForNode forNode = new ForNode();
            Lexer.MatchNow("("); //TODO:使用Match函数报错
            forNode.Left = MatchChunk(s_commonOrder);
            forNode.Middle = MatchExpression();
            Lexer.MatchNow(";"); //TODO:使用Match函数报错
            forNode.Right = MatchChunk(s_commonOrder);
            Lexer.MatchNow(")"); //TODO:使用Match函数报错
            forNode.Chunk = MatchChunk(s_commonOrder);
            return forNode;
        }

        private static WhileNode MatchWhile()
        {
            Lexer.MatchNow(TokenType.While);
            WhileNode whileNode = new WhileNode();
            Lexer.MatchNow("("); //TODO:使用Match函数报错
            whileNode.Condition = MatchExpression();
            Lexer.MatchNow(")"); //TODO:使用Match函数报错
            whileNode.Chunk = MatchChunk(s_commonOrder);
            return whileNode;
        }

        private static DefineVariableNode MatchDefineVariable(out AssignNode assign, bool haveAssign = true,
            AccessLevel accessLevel = AccessLevel.Local, bool isStatic = false)
        {
            assign = null;
            bool isArray = false;
            Lexer.Match(TokenType.Identifer); //TODO:使用Match函数报错
            string typeName = Lexer.NextTokenContent;
            Lexer.Next();
            if (Lexer.Match("["))
            {
                Lexer.MatchNext("]");
                Lexer.Next();
                isArray = true;
            }
            Lexer.Match(TokenType.Identifer); //TODO:使用Match函数报错
            string varName = Lexer.NextTokenContent;
            Lexer.Next();
            DefineVariableNode defineVariable =
                new DefineVariableNode(varName, typeName, accessLevel, isStatic, isArray);
            if (Lexer.NextTokenContent == ";")
            {
                Lexer.Next();
                return defineVariable;
            }
            if (!haveAssign) return defineVariable;
            Lexer.Back();
            assign = MatchAssign();
            return defineVariable;
        }

        private static AssignNode MatchAssign()
        {
            AssignNode assign = new AssignNode();
            assign.Left = MatchElement();
            string s = Lexer.NextTokenContent;
            Lexer.MatchNow(TokenType.Assign); //TODO:使用Match函数报错
            ExpressionNode expression = MatchExpression();
            if (s != "=")
                switch (s[0])
                {
                    case '+':
                        assign.Right = new OperateNode(OperateType.Puls, assign.Left, expression);
                        break;
                    case '-':
                        assign.Right = new OperateNode(OperateType.Minus, assign.Left, expression);
                        break;
                    case '*':
                        assign.Right = new OperateNode(OperateType.Multiply, assign.Left, expression);
                        break;
                    case '/':
                        assign.Right = new OperateNode(OperateType.Divide, assign.Left, expression);
                        break;
                    case '%':
                        assign.Right = new OperateNode(OperateType.Modulus, assign.Left, expression);
                        break;
                }
            else
                assign.Right = expression;
            Lexer.MatchNow(";"); //TODO:使用Match函数报错
            return assign;
        }

        private static InvokerNode MatchInvoker(string functionName)
        {
            InvokerNode invoker = new InvokerNode(functionName);
            while (Lexer.NextTokenContent != ")")
            {
                Lexer.Next();
                invoker.AddParm(MatchExpression());
                if (Lexer.NextTokenContent != "," && Lexer.NextTokenContent != ")") return invoker; //TODO:报错 意外的符号
            }
            //分号由调用者处理
            Lexer.Next();
            return invoker;
        }

        private static ArrayNode MatchArray(string name)
        {
            Lexer.Next();
            ArrayNode array = new ArrayNode
            {
                Content = name,
                ElemtntType = ElemtntType.Array,
                Expression = MatchExpression()
            };
            Lexer.MatchNow("]"); //TODO:使用Match函数报错
            return array;
        }

        private static CommandNode MatchCommand()
        {
            switch (Lexer.NextTokenContent)
            {
                case "continue":
                    Lexer.MatchNext(";"); //TODO:使用Match函数报错
                    Lexer.Next();
                    return new CommandNode(CommandType.Continue);
                case "break":
                    Lexer.MatchNext(";"); //TODO:使用Match函数报错
                    Lexer.Next();
                    return new CommandNode(CommandType.Break);
                case "return":
                    if (Lexer.MatchNext(";"))
                    {
                        Lexer.Next();
                        return new CommandNode(CommandType.Return);
                    }
                    else
                    {
                        ExpressionNode expression = MatchExpression();
                        Lexer.Next();
                        return new CommandNode(CommandType.Return, expression);
                    }
            }
            return null;
        }

        private static DefSpecifierNode MatchDefinitionSpecifier()
        {
            DefSpecifierNode defSpecifier = new DefSpecifierNode(Lexer.NextTokenContent);
            DefSpecifierNode next = defSpecifier;
            Lexer.Next();
            while (Lexer.NextTokenContent == ".")
            {
                Lexer.MatchNext(TokenType.Identifer); //TODO:使用Match函数报错
                next.NextSpecifier = new DefSpecifierNode(Lexer.NextTokenContent);
                next = next.NextSpecifier;
                Lexer.Next();
            }
            return defSpecifier;
        }

        private static ExpressionNode MatchExpression()
        {
            ExpressionNode expression = MatchBinary5();
            while (Lexer.NextTokenContent == "||")
            {
                Lexer.Next();
                expression = new OperateNode(OperateType.Or, expression, MatchBinary5());
            }
            return expression;
        }
        private static ExpressionNode MatchBinary5()
        {
            ExpressionNode expression = MatchBinary4();
            while (Lexer.NextTokenContent == "&&")
            {
                Lexer.Next();
                expression = new OperateNode(OperateType.And, expression, MatchBinary4());
            }
            return expression;
        }
        private static ExpressionNode MatchBinary4()
        {
            ExpressionNode expression = MatchBinary3();
            while (Lexer.NextTokenContent == "==" || Lexer.NextTokenContent == "!=")
            {
                string s = Lexer.NextTokenContent;
                Lexer.Next();
                switch (s)
                {
                    case "==":
                        expression = new OperateNode(OperateType.Equal, expression, MatchBinary3());
                        break;
                    case "!=":
                        expression = new OperateNode(OperateType.NotEqual, expression, MatchBinary3());
                        break;
                }
            }
            return expression;
        }
        private static ExpressionNode MatchBinary3()
        {
            ExpressionNode expression = MatchBinary2();
            while (Lexer.NextTokenContent == ">" || Lexer.NextTokenContent == ">=" || 
                   Lexer.NextTokenContent == "<" || Lexer.NextTokenContent == "<=")
            {
                string s = Lexer.NextTokenContent;
                Lexer.Next();
                switch (s)
                {
                    case ">":
                        expression = new OperateNode(OperateType.Greater, expression, MatchBinary2());
                        break;
                    case ">=":
                        expression = new OperateNode(OperateType.GreaterEqual, expression, MatchBinary2());
                        break;
                    case "<":
                        expression = new OperateNode(OperateType.Less, expression, MatchBinary2());
                        break;
                    case "<=":
                        expression = new OperateNode(OperateType.LessEqual, expression, MatchBinary2());
                        break;
                }
            }
            return expression;
        }
        private static ExpressionNode MatchBinary2()
        {
            ExpressionNode expression = MatchBinary1();
            while (Lexer.NextTokenContent == "+" || Lexer.NextTokenContent == "-")
            {
                string s = Lexer.NextTokenContent;
                Lexer.Next();
                switch (s)
                {
                    case "+":
                        expression = new OperateNode(OperateType.Puls, expression, MatchBinary1());
                        break;
                    case "-":
                        expression = new OperateNode(OperateType.Minus, expression, MatchBinary1());
                        break;
                }
            }
            return expression;
        }
        private static ExpressionNode MatchBinary1()
        {
            ExpressionNode expression = MatchUnary();
            while (Lexer.NextTokenContent == "*" || Lexer.NextTokenContent == "/" || Lexer.NextTokenContent == "%")
            {
                string s = Lexer.NextTokenContent;
                Lexer.Next();
                switch (s)
                {
                    case "*":
                        expression = new OperateNode(OperateType.Equal, expression, MatchUnary());
                        break;
                    case "/":
                        expression = new OperateNode(OperateType.NotEqual, expression, MatchUnary());
                        break;
                    case "%":
                        expression = new OperateNode(OperateType.NotEqual, expression, MatchUnary());
                        break;
                }
            }
            return expression;
        }
        private static ExpressionNode MatchUnary()
        {
            switch (Lexer.NextTokenContent)
            {
                case "-":
                    Lexer.Next();
                    return new OperateNode(OperateType.UMinus, MatchElement());
                case "!":
                    Lexer.Next();
                    return new OperateNode(OperateType.Not, MatchElement());
                case "cast":
                    Lexer.MatchNext("<"); //TODO:使用Match函数报错
                    Lexer.MatchNext(TokenType.Identifer); //TODO:使用Match函数报错
                    string s = Lexer.NextTokenContent;
                    Lexer.MatchNext(">"); //TODO:使用Match函数报错
                    Lexer.Next();
                    return new OperateNode(s, MatchElement());
                default:
                    return MatchElement();
            }
        }
        private static ElementNode MatchElement() //TODO:添加++，--的支持
        {
            switch (Lexer.NextTokenType)
            {
                case TokenType.True:
                    Lexer.Next();
                    return new ElementNode(ElemtntType.Boolean, "true");
                case TokenType.False:
                    Lexer.Next();
                    return new ElementNode(ElemtntType.Boolean, "false");
                case TokenType.Number:
                    string n = Lexer.NextTokenContent;
                    Lexer.Next();
                    return new ElementNode(ElemtntType.Integer, n);
                case TokenType.Float:
                    string f = Lexer.NextTokenContent;
                    Lexer.Next();
                    return new ElementNode(ElemtntType.Float, f);
                case TokenType.Char:
                    string c = Lexer.NextTokenContent;
                    Lexer.Next();
                    return new ElementNode(ElemtntType.Char, c);
                case TokenType.String:
                    string s = Lexer.NextTokenContent;
                    Lexer.Next();
                    return new ElementNode(ElemtntType.String, s);
                case TokenType.Identifer:
                    string name = Lexer.NextTokenContent;
                    Lexer.Next();
                    ElementNode element;
                    if (Lexer.NextTokenContent == "(")
                        element = MatchInvoker(name);
                    else if (Lexer.NextTokenContent == "[")
                        element = MatchArray(name);
                    else element = new ElementNode(ElemtntType.Variable, name);
                    ElementNode next = element;
                    while (Lexer.NextTokenContent == ".")
                    {
                        Lexer.MatchNext(TokenType.Identifer); //TODO:使用Match函数报错
                        name = Lexer.NextTokenContent;
                        Lexer.Next();
                        if (Lexer.NextTokenContent == "(")
                            next.NextElement = MatchInvoker(name);
                        else if (Lexer.NextTokenContent == "[")
                            next.NextElement = MatchArray(name);
                        else next.NextElement = new ElementNode(ElemtntType.Variable, name);
                        next = next.NextElement;
                    }
                    return element;
                case TokenType.New:
                    NewNode newNode = new NewNode();
                    Lexer.MatchNext(TokenType.Identifer);
                    newNode.TypeName = Lexer.NextTokenContent;
                    Lexer.Next();
                    if (Lexer.Match("["))
                    {
                        Lexer.Next();
                        newNode.IsArray = true;
                        newNode.Expression = MatchExpression();
                        Lexer.Match("]"); //TODO:使用Match函数报错
                        return newNode;
                    }
                    Lexer.Match("("); //TODO:使用Match函数报错
                    Lexer.Next();
                    while (Lexer.NextTokenContent != ")")
                    {
                        newNode.AddParm(MatchExpression());
                        if (Lexer.NextTokenContent != "," && Lexer.NextTokenContent != ")") return newNode; //TODO:报错 意外的符号
                    }
                    if (!Lexer.MatchNext(".")) return newNode;
                    Lexer.Next();
                    newNode.NextElement = MatchElement();
                    return newNode;
                case TokenType.Parenthesis:
                    if (Lexer.NextTokenContent == "(")
                    {
                        Lexer.Next();
                        ElementNode expression = new ElementNode
                        {
                            ElemtntType = ElemtntType.Expression,
                            Expression = MatchExpression()
                        };
                        Lexer.MatchNow(")"); //TODO:使用Match函数报错
                        return expression;
                    }
                    //TODO:报错 意外的符号
                    return null;
                default:
                    //TODO:报错 意外的字符
                    return null;
            }
        }

        private static AccessLevel AnalysisAccessLevel(string name)
        {
            char c = name[0];
            if (c == '_') return AccessLevel.Private;
            if (char.IsLower(c)) return AccessLevel.Private;
            if (char.IsUpper(c)) return AccessLevel.Internal;
            return AccessLevel.Private;
        }
    }
}