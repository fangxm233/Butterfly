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
        Continue,
        Break,
        Return,
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
        public static List<UsingNode>[] FilesReferences;
        public static List<CustomDefinition>[] FilesDefinitions;
        public static Dictionary<string, DefSpecifierNode>[] FilesAliases;
        public static NameSpaceDefinition RootNameSpace;
        private NameSpaceDefinition _endNameSpace;
        private CustomDefinition _parsingStructure;
        private Lexer _lexer;
        private SyntaxFactory _syntaxFactory;

        private readonly OrderType[] s_cusDefinitionOrder =  {OrderType.DefineVariable, OrderType.Function};
        private readonly OrderType[] s_interfaceDefinitionOrder = { OrderType.Function };
        private readonly OrderType[] s_commonOrder =
        {
            OrderType.DefineVariable, OrderType.Assign, OrderType.If,
            OrderType.While, OrderType.For, OrderType.Chunk,
            OrderType.Continue, OrderType.Break, OrderType.Return, OrderType.Invoker, OrderType.New
        };

        private void Init(string outputName)
        {
            RootNameSpace = new NameSpaceDefinition(outputName);
            _endNameSpace = new NameSpaceDefinition(outputName);
            FilesReferences = new List<UsingNode>[Lexer.FileCount];
            FilesAliases = new Dictionary<string, DefSpecifierNode>[Lexer.FileCount];
            FilesDefinitions = new List<CustomDefinition>[Lexer.FileCount];
            for (int i = 0; i < Lexer.FileCount; i++)
            {
                FilesReferences[i] = new List<UsingNode>();
                FilesAliases[i] = new Dictionary<string, DefSpecifierNode>();
                FilesDefinitions[i] = new List<CustomDefinition>();
            }
        }

        public void Parse(string outputName, Lexer lexer)
        {
            _lexer = lexer;
            _syntaxFactory = new SyntaxFactory(this, lexer);
            Init(outputName);
            while (_lexer.FileIndex < Lexer.FileCount)
            {
                switch (_lexer.NextTokenType)
                {
                    case TokenType.UsingKeyword:
                        _lexer.Next();
                        ParseUsing();
                        break;
                    case TokenType.NameSpaceKeyword:
                        _lexer.Next();
                        ParseNameSpace();
                        break;
                    case TokenType.ClassKeyword:
                        ParseClass(AccessLevel.Private, false);
                        break;
                    case TokenType.StructKeyword:
                        ParseStruct(AccessLevel.Private);
                        break;
                    case TokenType.InterfaceKeyword:
                        ParseInterface(AccessLevel.Private);
                        break;
                    case TokenType.StaticKeyword:
                    case TokenType.PublicKeyword:
                        AccessLevel accessLevel = ParseAccessLevelAndStatic(out bool isStatic);
                        switch (_lexer.NextTokenType)
                        {
                            case TokenType.ClassKeyword:
                                ParseClass(accessLevel, isStatic);
                                break;
                            case TokenType.StructKeyword:
                                ParseStruct(accessLevel);
                                break;
                            case TokenType.InterfaceKeyword:
                                ParseInterface(accessLevel);
                                break;
                        }
                        break;
                    default:
                        return; //TODO:报错 意外的符号
                }
                if(_lexer.NextTokenType == TokenType.EOF) Console.WriteLine("一个文件编译完成!!");
            }
        }

        private void ParseUsing()
        {
            List<UsingNode> references = FilesReferences[_lexer.FileIndex];
            Token s = _lexer.Eat(TokenType.Identifer); //TODO:使用Match函数报错
            if (_lexer.MatchNow("="))
            {
                FilesAliases[_lexer.FileIndex].Add(s.Content, ParseDefinitionSpecifier());
                return;
            }
            references.Add(_syntaxFactory.GetUsingNode(s, null));
            UsingNode last = references.Last();
            while (_lexer.Match("."))
            {
                _lexer.MatchNext(TokenType.Identifer); //TODO:使用Match函数报错
                last.AddAfter(_lexer.Eat(TokenType.Identifer), _syntaxFactory);
                last = last.NextUsing;
            }
            _lexer.MatchNow(";"); //TODO:使用Match函数报错
        }

        private void ParseNameSpace()
        {
            _lexer.Match(TokenType.Identifer); //TODO:使用Match函数报错
            if (!RootNameSpace.IsContainNameSpace(_lexer.NextTokenContent))
                RootNameSpace.AddAfter(_lexer.NextTokenContent);
            NameSpaceDefinition last = RootNameSpace.GetNameSpaceDefinition(_lexer.NextTokenContent);
            _lexer.Next();
            while (_lexer.Match("."))
            {
                _lexer.MatchNext(TokenType.Identifer); //TODO:使用Match函数报错
                last.AddAfter(_lexer.NextTokenContent);
                last = last.GetNameSpaceDefinition(_lexer.NextTokenContent);
                _lexer.Next();
            }
            _endNameSpace = last;
            _lexer.MatchNow(";"); //TODO:使用Match函数报错
        }

        private void ParseClass(AccessLevel accessLevel, bool isStatic)
        {
            Token classKeyword = _lexer.Eat(TokenType.ClassKeyword);
            Token name = _lexer.Eat(TokenType.Identifer);
            if (accessLevel != AccessLevel.Public)
                accessLevel = AnalysisAccessLevel(name.Content);
            Token colon = null, inheritanceToken = null;
            string inheritanceName = "object";
            if (_lexer.Match(":"))
            {
                colon = _lexer.Eat(TokenType.Colon);
                inheritanceToken = _lexer.Eat(TokenType.Identifer);
                inheritanceName = inheritanceToken.Content;
            }
            ClassDefinition classDefinition =
                new ClassDefinition(classKeyword, name, colon, inheritanceToken, inheritanceName, _endNameSpace, accessLevel, isStatic);
            _endNameSpace.AddStructure(classDefinition);
            FilesDefinitions[_lexer.FileIndex].Add(classDefinition);
            _parsingStructure = classDefinition;
            classDefinition.AddFunction(new FunctionDefinition(classDefinition, ".ctor", classDefinition.AccessLevel, false));
            classDefinition.AddFunction(new FunctionDefinition(classDefinition, ".cctor", classDefinition.AccessLevel, true));
            ChunkNode chunk = ParseChunk(s_cusDefinitionOrder, out Token openBrace, out Token closeBrace);
            classDefinition.SetBraces(openBrace, closeBrace);
            foreach (SyntaxNode analysisNode in chunk.Nodes)
            {
                if (analysisNode.NodeType == NodeType.Assign)
                    classDefinition.GetFunctionDefinition(".ctor").ChunkNode.AddFirstNode(analysisNode);
                classDefinition.AddField((DefineVariableNode)analysisNode);
            }
            _parsingStructure = null;
        }

        private void ParseStruct(AccessLevel accessLevel)
        {
            Token structKeyword = _lexer.Eat(TokenType.StructKeyword);
            Token name = _lexer.Eat(TokenType.Identifer);
            if (accessLevel != AccessLevel.Public)
                accessLevel = AnalysisAccessLevel(name.Content);
            Token colon = null, inheritanceToken = null;
            if (_lexer.Match(":"))
            {
                colon = _lexer.Eat(TokenType.Colon);
                inheritanceToken = _lexer.Eat(TokenType.Identifer);
            }
            StructDefinition structDefinition =
                new StructDefinition(structKeyword, name, colon, inheritanceToken, _endNameSpace, accessLevel);
            if (accessLevel != AccessLevel.Public)
                structDefinition.AccessLevel = AnalysisAccessLevel(_lexer.NextTokenContent);
            if (_lexer.MatchNext(":"))
                if (_lexer.MatchNext(TokenType.Identifer)) //TODO:使用Match函数报错
                {
                    structDefinition.InheritanceName = _lexer.NextTokenContent;
                    _lexer.Next();
                }
            _endNameSpace.AddStructure(structDefinition);
            _parsingStructure = structDefinition;
            FilesDefinitions[_lexer.FileIndex].Add(structDefinition);
            structDefinition.AddFunction(new FunctionDefinition(structDefinition, ".ctor", structDefinition.AccessLevel, false));
            ChunkNode chunk = ParseChunk(s_cusDefinitionOrder, out Token openBrace, out Token closeBrace);
            structDefinition.SetBraces(openBrace, closeBrace);
            foreach (SyntaxNode analysisNode in chunk.Nodes)
                structDefinition.AddField((DefineVariableNode)analysisNode);
            _parsingStructure = null;
        }

        private void ParseInterface(AccessLevel accessLevel)
        {
            Token interfaceKeyword = _lexer.Eat(TokenType.InterfaceKeyword);
            Token name = _lexer.Eat(TokenType.Identifer);
            if (accessLevel != AccessLevel.Public)
                accessLevel = AnalysisAccessLevel(name.Content);
            Token colon = null, inheritanceToken = null;
            if (_lexer.Match(":"))
            {
                colon = _lexer.Eat(TokenType.Colon);
                inheritanceToken = _lexer.Eat(TokenType.Identifer);
            }
            InterfaceDefinition interfaceDefinition =
                new InterfaceDefinition(interfaceKeyword, name, colon, inheritanceToken, _endNameSpace, accessLevel);
            _endNameSpace.AddStructure(interfaceDefinition);
            FilesDefinitions[_lexer.FileIndex].Add(interfaceDefinition);
            _parsingStructure = interfaceDefinition;
            ParseChunk(s_interfaceDefinitionOrder, out Token openBrace, out Token closeBrace);
            interfaceDefinition.SetBraces(openBrace, closeBrace);
            _parsingStructure = null;
        }

        private void ParseFunction(AccessLevel accessLevel, bool isStatic)
        {
            Token name = _lexer.Eat(TokenType.Identifer);
            Token openParen = _lexer.Eat(TokenType.OpenParen);

            //参数
            List<DefineVariableNode> parms = new List<DefineVariableNode>();
            if (!_lexer.Match(")"))
                do
                {
                    _lexer.Match(TokenType.Identifer); //TODO:使用Match函数报错
                    DefineVariableNode variable =
                        ParseDefineVariable(out AssignNode assign, false);
                    if (assign != null) return; //TODO:报错 没有赋值
                    parms.Add(variable);
                    if (_lexer.Match(")")) break;
                } while (_lexer.MatchNow(","));
            if (!_lexer.Match(")")) return; //TODO:报错 意外的符号
            Token closeParen = _lexer.Eat(TokenType.CloseParen);

            //返回值
            Token colon = null, returnToken = null;
            string returnName = "void";
            if (_lexer.Match(":"))
            {
                colon = _lexer.Eat(TokenType.Colon);
                if (_lexer.Match(TokenType.Identifer))
                {
                    returnToken = _lexer.Eat(TokenType.Identifer);
                    returnName = returnToken.Content;
                }
                else return; //TODO:使用Match函数报错
            }

            FunctionDefinition function;
            if (_parsingStructure is InterfaceDefinition)
            {
                if (_lexer.Match("{")) return; //TODO:报错 接口成员不能有定义
                _lexer.MatchNow(";"); //TODO:使用Match函数报错
                function = new FunctionDefinition(name, openParen, closeParen, colon, returnToken, returnName,
                    null, null, _parsingStructure, parms, null, accessLevel, false);
                _parsingStructure.AddFunction(function);
                return;
            }
            ChunkNode chunkNode = ParseChunk(s_commonOrder, out Token openBrace, out Token closeBrace);
            function = new FunctionDefinition(name, openParen, closeParen, colon, returnToken, returnName, 
                openBrace, closeBrace, _parsingStructure, parms, chunkNode, accessLevel, isStatic);
            _parsingStructure.AddFunction(function);
        }

        private ChunkNode ParseChunk(OrderType[] order)
        {
            return ParseChunkCore(order, out Token o, out Token c);
        }

        private ChunkNode ParseChunk(OrderType[] order, out Token openBrace, out Token closeBrace)
        {
            return ParseChunkCore(order, out openBrace, out closeBrace);
        }

        private ChunkNode ParseChunkCore(OrderType[] order, out Token openBrace, out Token closeBrace)
        {
            ChunkNode chunk = _syntaxFactory.GetChunkNode();
            openBrace = null;
            closeBrace = null;
            bool isMatchOneNode = false;
            if (_lexer.Match("{"))
                openBrace = _lexer.Eat(TokenType.OpenBrace);
            else
                isMatchOneNode = true;
            while (true)
            {
                if (_lexer.Match("}"))
                {
                    closeBrace = _lexer.Eat(TokenType.CloseBrace);
                    return chunk;
                }
                switch (_lexer.NextTokenType)
                {
                    case TokenType.StaticKeyword:
                    case TokenType.PublicKeyword:
                        AccessLevel accessLevel = ParseAccessLevelAndStatic(out bool isStatic);
                        switch (_lexer.NextTokenType)
                        {
                            case TokenType.FuncKeyword:
                                if (!IsInclude(order, OrderType.Function)) return chunk; //TODO:报错 意外的符号
                                _lexer.Next();
                                ParseFunction(accessLevel, isStatic);
                                break;
                            case TokenType.VarKeyword:
                                if (!IsInclude(order, OrderType.DefineVariable))
                                    return chunk; //TODO:报错 意外的符号
                                _lexer.Next();
                                chunk.AddNode(ParseDefineVariable(out AssignNode assign1, true, accessLevel, isStatic));
                                if (assign1 != null) chunk.AddNode(assign1);
                                break;
                        }
                        break;
                    case TokenType.FuncKeyword:
                        if (!IsInclude(order, OrderType.Function)) return chunk; //TODO:报错 意外的符号
                        _lexer.Next();
                        ParseFunction(AccessLevel.Private, false);
                        break;
                    case TokenType.VarKeyword:
                        if (!IsInclude(order, OrderType.DefineVariable) && !IsInclude(order, OrderType.LocalVariable))
                            return chunk; //TODO:报错 意外的符号
                        _lexer.Next();
                        chunk.AddNode(ParseDefineVariable(out AssignNode assign, true,
                            IsInclude(order, OrderType.LocalVariable) ? AccessLevel.Local : AccessLevel.Private));
                        if (assign != null) chunk.AddNode(assign);
                        break;
                    case TokenType.LetKeyword:
                        if (!IsInclude(order, OrderType.Assign)) return chunk; //TODO:报错 意外的符号
                        _lexer.Next();
                        chunk.AddNode(ParseAssign());
                        break;
                    case TokenType.OpenBrace:
                        if (!IsInclude(order, OrderType.Chunk)) return chunk; //TODO:报错 意外的符号
                        chunk.AddNode(ParseChunk(s_commonOrder));
                        break;
                    case TokenType.ContinueKeyword:
                        if (!IsInclude(order, OrderType.Continue)) return chunk; //TODO:报错 意外的符号
                        chunk.AddNode(ParseContinue());
                        break;
                    case TokenType.BreakKeyword:
                        if (!IsInclude(order, OrderType.Break)) return chunk; //TODO:报错 意外的符号
                        chunk.AddNode(ParseBreak());
                        break;
                    case TokenType.ReturnKeyword:
                        if (!IsInclude(order, OrderType.Return)) return chunk; //TODO:报错 意外的符号
                        chunk.AddNode(ParseReturn());
                        break;
                    case TokenType.ForKeyword:
                        if (!IsInclude(order, OrderType.For)) return chunk; //TODO:报错 意外的符号
                        chunk.AddNode(ParseFor());
                        break;
                    case TokenType.IfKeyword:
                        if (!IsInclude(order, OrderType.If)) return chunk; //TODO:报错 意外的符号
                        chunk.AddNode(ParseIf());
                        break;
                    case TokenType.InvKeyword:
                        if (!IsInclude(order, OrderType.Invoker)) return chunk; //TODO:报错 意外的符号
                        _lexer.Next();
                        chunk.AddNode(ParseElement());
                        _lexer.MatchNow(";"); //TODO:使用Match函数报错
                        break;
                    case TokenType.NewKeyword:
                        if (!IsInclude(order, OrderType.New)) return chunk; //TODO:报错 意外的符号
                        chunk.AddNode(ParseElement());
                        _lexer.MatchNow(";"); //TODO:使用Match函数报错
                        break;
                    case TokenType.WhileKeyword:
                        if (!IsInclude(order, OrderType.While)) return chunk; //TODO:报错 意外的符号
                        chunk.AddNode(ParseWhile());
                        break;
                    default:
                        return chunk; //TODO:报错 意外的符号
                }
                if (isMatchOneNode) return chunk;
            }
        }

        private bool IsInclude(OrderType[] orders, OrderType order) =>
            orders.Any(orderType => order == orderType);

        private AccessLevel ParseAccessLevelAndStatic(out bool isStatic)
        {
            AccessLevel accessLevel = _lexer.Match(TokenType.PublicKeyword) ? AccessLevel.Public : AccessLevel.Private;
            if (accessLevel == AccessLevel.Public) _lexer.Next();
            isStatic = _lexer.Match(TokenType.StaticKeyword);
            if (isStatic) _lexer.Next();
            return accessLevel;
        }

        private IfNode ParseIf()
        {
            Token ifKeyword = _lexer.Eat(TokenType.IfKeyword);
            Token openParen = _lexer.Eat("("); //TODO:使用Match函数报错
            ExpressionNode expression = ParseExpression();
            Token closeParen = _lexer.Eat(")"); //TODO:使用Match函数报错
            ChunkNode chunkNode = ParseChunk(s_commonOrder, out Token openBrace, out Token closeBrace);
            if (_lexer.Match(TokenType.ElseKeyword))
            {
                Token elseKeyword = _lexer.Eat(TokenType.ElseKeyword);
                ChunkNode elseChunkNode = ParseChunk(s_commonOrder, out Token EpenBrace, out Token EcloseBrace);
                return _syntaxFactory.GetIfNode(ifKeyword, openParen, closeParen, openBrace, closeBrace,
                    expression, chunkNode, elseKeyword, EpenBrace, EcloseBrace, elseChunkNode);
            }
            return _syntaxFactory.GetIfNode(ifKeyword, openParen, closeParen, openBrace, closeBrace, expression, chunkNode);
        }

        private ForNode ParseFor()
        {
            Token forKeyword = _lexer.Eat(TokenType.ForKeyword);
            Token openParen = _lexer.Eat("("); //TODO:使用Match函数报错
            ChunkNode left = ParseChunk(s_commonOrder);
            ExpressionNode middle = ParseExpression();
            _lexer.MatchNow(";"); //TODO:使用Match函数报错
            ChunkNode right = ParseChunk(s_commonOrder);
            Token closeParen = _lexer.Eat(")"); //TODO:使用Match函数报错
            ChunkNode chunk = ParseChunk(s_commonOrder, out Token openBrace, out Token cloceBrace);
            return _syntaxFactory.GetForNode(forKeyword, openParen, closeParen, openBrace, cloceBrace, left, right, chunk, middle);
        }

        private WhileNode ParseWhile()
        {
            Token whileKeyword = _lexer.Eat(TokenType.WhileKeyword);
            Token openParen = _lexer.Eat("("); //TODO:使用Match函数报错
            ExpressionNode condition = ParseExpression();
            Token closeParen = _lexer.Eat(")"); //TODO:使用Match函数报错
            ChunkNode chunk = ParseChunk(s_commonOrder, out Token openBrace, out Token closeBrace);
            return _syntaxFactory.GetWhileNode(whileKeyword, openParen, closeParen, openBrace, closeBrace, condition, chunk);
        }

        private DefineVariableNode ParseDefineVariable(out AssignNode assign, bool haveAssign = true,
            AccessLevel accessLevel = AccessLevel.Local, bool isStatic = false)
        {
            assign = null;
            bool isArray = false;
            byte rankNum = 1;
            _lexer.Match(TokenType.Identifer); //TODO:使用Match函数报错
            Token type = _lexer.NextToken;
            if (_lexer.MatchNext("["))
            {
                _lexer.Next();
                for (; !_lexer.Match("]"); _lexer.Next())
                    if (_lexer.Match(",")) rankNum++;
                    else return null; //TODO:报错 无效的秩说明符:应为","或"]"
                _lexer.MatchNow("]");
                isArray = true;
            }
            _lexer.Match(TokenType.Identifer); //TODO:使用Match函数报错
            Token name = _lexer.NextToken;
            _lexer.Next();
            DefineVariableNode defineVariable = _syntaxFactory.GetDefineVariableNode(name, type, accessLevel, isStatic, isArray, rankNum);
            if (_lexer.NextTokenContent == ";")
            {
                _lexer.Next();
                return defineVariable;
            }
            if (!haveAssign) return defineVariable;
            _lexer.Back();
            assign = ParseAssign();
            return defineVariable;
        }

        private AssignNode ParseAssign()
        {
            ElementNode left = ParseElement();
            Token assignToken = _lexer.Eat(TokenType.Assign); //TODO:使用Match函数报错
            ExpressionNode expression = ParseExpression();
            if (assignToken.Content != "=")
                switch (assignToken.Content[0])
                {
                    case '+':
                        expression = _syntaxFactory.GetOperateNode(assignToken, OperateType.Plus, left, expression);
                        break;
                    case '-':
                        expression = _syntaxFactory.GetOperateNode(assignToken, OperateType.Minus, left, expression);
                        break;
                    case '*':
                        expression = _syntaxFactory.GetOperateNode(assignToken, OperateType.Multiply, left, expression);
                        break;
                    case '/':
                        expression = _syntaxFactory.GetOperateNode(assignToken, OperateType.Divide, left, expression);
                        break;
                    case '%':
                        expression = _syntaxFactory.GetOperateNode(assignToken, OperateType.Modulus, left, expression);
                        break;
                }
            _lexer.MatchNow(";"); //TODO:使用Match函数报错
            return _syntaxFactory.GetAssignNode(assignToken, left, expression);
        }

        private InvokerNode ParseInvoker(Token name)
        {
            Token openParen = _lexer.Eat("("); //TODO:使用Match函数报错
            List<ExpressionNode> parms = new List<ExpressionNode>();
            if (!_lexer.Match(")"))
                do
                {
                    parms.Add(ParseExpression());
                    if (_lexer.Match(")")) break;
                } while (_lexer.MatchNow(","));
            Token closeParen = _lexer.Eat(")"); //TODO:使用Match函数报错
            //分号由调用者处理
            return _syntaxFactory.GetInvokerNode(name, openParen, closeParen, parms);
        }

        private ArrayNode ParseArray(Token name)
        {
            Token openBracket = _lexer.Eat("["); //TODO:使用Match函数报错
            List<ExpressionNode> expressions = new List<ExpressionNode>();
            List<Token> commas = new List<Token>();
            do
            {
                commas.Add(_lexer.Eat(","));
                expressions.Add(ParseExpression());
                if (_lexer.Match("]")) break;
            } while (_lexer.Match(","));
            Token closeBracket = _lexer.Eat("]"); //TODO:使用Match函数报错
            return _syntaxFactory.getArrayNode(name, openBracket, closeBracket, expressions, commas);
        }

        private ContinueNode ParseContinue()
        {
            Token continueKeyword = _lexer.Eat(TokenType.ContinueKeyword);
            Token semicolon = _lexer.Eat(TokenType.Semicolon);
            return _syntaxFactory.GetContinueNode(continueKeyword, semicolon);
        }

        private BreakNode ParseBreak()
        {
            Token breakKeyword = _lexer.Eat(TokenType.BreakKeyword);
            Token semicolon = _lexer.Eat(TokenType.Semicolon);
            return _syntaxFactory.GetBreakNode(breakKeyword, semicolon);
        }

        private ReturnNode ParseReturn()
        {
            Token returnKeyword = _lexer.Eat(TokenType.ReturnKeyword);
            if (_lexer.Match(";"))
            {
                Token semicolon = _lexer.Eat(TokenType.Semicolon);
                return _syntaxFactory.GetReturnNode(returnKeyword, semicolon);
            }
            else
            {
                ExpressionNode expression = ParseExpression();
                Token semicolon = _lexer.Eat(TokenType.Semicolon);
                return _syntaxFactory.GetReturnNode(returnKeyword, semicolon, expression);
            }
        }

        private DefSpecifierNode ParseDefinitionSpecifier()
        {
            DefSpecifierNode defSpecifier = _syntaxFactory.GetDefSpecifierNode(_lexer.NextTokenContent);
            DefSpecifierNode next = defSpecifier;
            _lexer.Next();
            while (_lexer.NextTokenContent == ".")
            {
                _lexer.MatchNext(TokenType.Identifer); //TODO:使用Match函数报错
                next.NextSpecifier = _syntaxFactory.GetDefSpecifierNode(_lexer.NextTokenContent);
                next = next.NextSpecifier;
                _lexer.Next();
            }
            return defSpecifier;
        }

        private ExpressionNode ParseExpression()
        {
            ExpressionNode expression = ParseBinary5();
            while (_lexer.NextTokenContent == "||")
            {
                Token op = _lexer.Eat("||");
                expression = _syntaxFactory.GetOperateNode(op, OperateType.Or, expression, ParseBinary5());
            }
            return expression;
        }
        private ExpressionNode ParseBinary5()
        {
            ExpressionNode expression = ParseBinary4();
            while (_lexer.NextTokenContent == "&&")
            {
                Token op = _lexer.Eat("&&");
                expression = _syntaxFactory.GetOperateNode(op, OperateType.And, expression, ParseBinary4());
            }
            return expression;
        }
        private ExpressionNode ParseBinary4()
        {
            ExpressionNode expression = ParseBinary3();
            while (_lexer.NextTokenContent == "==" || _lexer.NextTokenContent == "!=")
            {
                string s = _lexer.NextTokenContent;
                Token op;
                switch (s)
                {
                    case "==":
                        op = _lexer.Eat("==");
                        expression = _syntaxFactory.GetOperateNode(op, OperateType.Equal, expression, ParseBinary3());
                        break;
                    case "!=":
                        op = _lexer.Eat("||");
                        expression = _syntaxFactory.GetOperateNode(op, OperateType.NotEqual, expression, ParseBinary3());
                        break;
                }
            }
            return expression;
        }
        private ExpressionNode ParseBinary3()
        {
            ExpressionNode expression = ParseBinary2();
            while (_lexer.NextTokenContent == ">" || _lexer.NextTokenContent == ">=" || 
                   _lexer.NextTokenContent == "<" || _lexer.NextTokenContent == "<=")
            {
                string s = _lexer.NextTokenContent;
                Token op;
                switch (s)
                {
                    case ">":
                        op = _lexer.Eat(">");
                        expression = _syntaxFactory.GetOperateNode(op, OperateType.Greater, expression, ParseBinary2());
                        break;
                    case ">=":
                        op = _lexer.Eat(">=");
                        expression = _syntaxFactory.GetOperateNode(op, OperateType.GreaterEqual, expression, ParseBinary2());
                        break;
                    case "<":
                        op = _lexer.Eat("<");
                        expression = _syntaxFactory.GetOperateNode(op, OperateType.Less, expression, ParseBinary2());
                        break;
                    case "<=":
                        op = _lexer.Eat("<=");
                        expression = _syntaxFactory.GetOperateNode(op, OperateType.LessEqual, expression, ParseBinary2());
                        break;
                }
            }
            return expression;
        }
        private ExpressionNode ParseBinary2()
        {
            ExpressionNode expression = ParseBinary1();
            while (_lexer.NextTokenContent == "+" || _lexer.NextTokenContent == "-")
            {
                string s = _lexer.NextTokenContent;
                Token op;
                switch (s)
                {
                    case "+":
                        op = _lexer.Eat("+");
                        expression = _syntaxFactory.GetOperateNode(op, OperateType.Plus, expression, ParseBinary1());
                        break;
                    case "-":
                        op = _lexer.Eat("-");
                        expression = _syntaxFactory.GetOperateNode(op, OperateType.Minus, expression, ParseBinary1());
                        break;
                }
            }
            return expression;
        }
        private ExpressionNode ParseBinary1()
        {
            ExpressionNode expression = ParseUnary();
            while (_lexer.NextTokenContent == "*" || _lexer.NextTokenContent == "/" || _lexer.NextTokenContent == "%")
            {
                string s = _lexer.NextTokenContent;
                Token op;
                switch (s)
                {
                    case "*":
                        op = _lexer.Eat("*");
                        expression = _syntaxFactory.GetOperateNode(op, OperateType.Multiply, expression, ParseUnary());
                        break;
                    case "/":
                        op = _lexer.Eat("/");
                        expression = _syntaxFactory.GetOperateNode(op, OperateType.Divide, expression, ParseUnary());
                        break;
                    case "%":
                        op = _lexer.Eat("%");
                        expression = _syntaxFactory.GetOperateNode(op, OperateType.Modulus, expression, ParseUnary());
                        break;
                }
            }
            return expression;
        }
        private ExpressionNode ParseUnary()
        {
            switch (_lexer.NextTokenContent)
            {
                case "-":
                    return _syntaxFactory.GetOperateNode(_lexer.Eat("-"), OperateType.UMinus, ParseElement());
                case "!":
                    return _syntaxFactory.GetOperateNode(_lexer.Eat("!"), OperateType.Not, ParseElement());
                case "cast":
                    Token op = _lexer.Eat(TokenType.CastKeyword);
                    _lexer.MatchNow("<"); //TODO:使用Match函数报错
                    Token t = _lexer.Eat(TokenType.Identifer); //TODO:使用Match函数报错
                    _lexer.MatchNow(">"); //TODO:使用Match函数报错
                    return _syntaxFactory.GetOperateNode(op, t.Content, ParseElement());
                default:
                    return ParseElement();
            }
        }
        private ElementNode ParseElement() //TODO:添加++，--的支持
        {
            switch (_lexer.NextTokenType)
            {
                case TokenType.TrueKeyword:
                    return _syntaxFactory.GetElementNode(NodeType.BooleanLiteral, _lexer.Eat(TokenType.TrueKeyword));
                case TokenType.FalseKeyword:
                    return _syntaxFactory.GetElementNode(NodeType.BooleanLiteral, _lexer.Eat(TokenType.FalseKeyword));
                case TokenType.NumericLiteralToken:
                    return _syntaxFactory.GetElementNode(NodeType.NumericLiteral, _lexer.Eat(TokenType.NumericLiteralToken));
                case TokenType.FloatLiteralToken:
                    return _syntaxFactory.GetElementNode(NodeType.FloatLiteral, _lexer.Eat(TokenType.FloatLiteralToken));
                case TokenType.CharacterLiteralToken:
                    return _syntaxFactory.GetElementNode(NodeType.CharacterLiteral, _lexer.Eat(TokenType.CharacterLiteralToken));
                case TokenType.StringLiteralToken:
                    return _syntaxFactory.GetElementNode(NodeType.StringLiteral, _lexer.Eat(TokenType.StringLiteralToken));
                case TokenType.Identifer:
                    Token name = _lexer.NextToken;
                    _lexer.Next();
                    ElementNode element;
                    if (_lexer.NextTokenContent == "(")
                        element = ParseInvoker(name);
                    else if (_lexer.NextTokenContent == "[")
                        element = ParseArray(name);
                    else element = _syntaxFactory.GetElementNode(NodeType.Variable, name);
                    ElementNode next = element;
                    while (_lexer.NextTokenContent == ".")
                    {
                        _lexer.MatchNext(TokenType.Identifer); //TODO:使用Match函数报错
                        name = _lexer.NextToken;
                        _lexer.Next();
                        if (_lexer.NextTokenContent == "(")
                            next.NextElement = ParseInvoker(name);
                        else if (_lexer.NextTokenContent == "[")
                            next.NextElement = ParseArray(name);
                        else next.NextElement = _syntaxFactory.GetElementNode(NodeType.Variable, name);
                        next = next.NextElement;
                    }
                    return element;
                case TokenType.NewKeyword:
                    Token newKeyword = _lexer.Eat(TokenType.NewKeyword);
                    Token typeName = _lexer.Eat(TokenType.Identifer);
                    Token openParen = null, closeParen = null;
                    bool isArray = false;
                    List<ExpressionNode> parms = new List<ExpressionNode>();
                    //_lexer.Next();
                    if (_lexer.Match("["))
                    {
                        openParen = _lexer.Eat(TokenType.OpenBracket);
                        isArray = true;
                        do
                        {
                            parms.Add(ParseExpression());
                            if (_lexer.Match("]")) break;
                        } while (_lexer.MatchNow(","));
                        closeParen = _lexer.Eat(TokenType.CloseBracket); //TODO:使用Match函数报错
                    }
                    else
                    {
                        openParen = _lexer.Eat(TokenType.OpenParen); //TODO:使用Match函数报错
                        if (!_lexer.Match(")"))
                            do
                            {
                                parms.Add(ParseExpression());
                                if (_lexer.Match(")")) break;
                            } while (_lexer.MatchNow(","));
                        closeParen = _lexer.Eat(TokenType.CloseParen); //TODO:使用Match函数报错
                    }
                    NewNode newNode = _syntaxFactory.GetNewNode(newKeyword, typeName, openParen, closeParen, parms, isArray);
                    if (!_lexer.Match(".")) return newNode;
                    _lexer.Next();
                    newNode.NextElement = ParseElement();
                    return newNode;
                case TokenType.OpenParen:
                    if (!_lexer.Match("(")) return null; //TODO:报错 意外的符号
                    _lexer.Next();
                    ElementNode expression = new ElementNode(NodeType.Expression, ParseExpression());
                    _lexer.MatchNow(")"); //TODO:使用Match函数报错
                    return expression;
                default:
                    //TODO:报错 意外的字符
                    return null;
            }
        }

        private AccessLevel AnalysisAccessLevel(string name)
        {
            char c = name[0];
            if (c == '_') return AccessLevel.Private;
            if (char.IsLower(c)) return AccessLevel.Private;
            if (char.IsUpper(c)) return AccessLevel.Internal;
            return AccessLevel.Private;
        }
    }
}