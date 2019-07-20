using Core.LexicalAnalysis;
using Core.SyntacticAnalysis.Nodes;
using System.Collections.Generic;

namespace Core.SyntacticAnalysis
{
    internal class SyntaxFactory
    {
        private Parser _parser;
        private Lexer _lexer;
        internal SyntaxFactory(Parser parser, Lexer lexer)
        {
            _parser = parser;
            _lexer = lexer;
        }

        internal ArrayNode getArrayNode(Token identifier, Token openBracket, Token closeBracket, List<ExpressionNode> expressions, List<Token> commas)
        {
            return new ArrayNode(identifier, openBracket, closeBracket, expressions, commas);
        }

        internal AssignNode GetAssignNode(Token assign, ElementNode left, ExpressionNode right)
        {
            return new AssignNode(assign, left, right);
        }

        internal ChunkNode GetChunkNode()
        {
            return new ChunkNode();
        }

        internal ContinueNode GetContinueNode(Token @continue, Token semicolon)
        {
            return new ContinueNode(@continue, semicolon);
        }

        internal BreakNode GetBreakNode(Token @break, Token semicolon)
        {
            return new BreakNode(@break, semicolon);
        }

        internal ReturnNode GetReturnNode(Token @return, Token semicolon, ExpressionNode expression)
        {
            return new ReturnNode(@return, semicolon, expression);
        }

        internal ReturnNode GetReturnNode(Token @return, Token semicolon)
        {
            return new ReturnNode(@return, semicolon);
        }

        internal DefineVariableNode GetDefineVariableNode(Token name, Token type, AccessLevel accessLevel,
            bool isStatic = false, bool isArray = false, byte rankNum = 0)
        {
            return new DefineVariableNode(name, type, accessLevel, isStatic, isArray, rankNum);
        }

        internal DefSpecifierNode GetDefSpecifierNode(string content)
        {
            return new DefSpecifierNode(content);
        }

        internal ElementNode GetElemtntNode(NodeType nodeType, ExpressionNode expression)
        {
            return new ElementNode(nodeType, expression);
        }

        internal ElementNode GetElementNode(NodeType nodeType, Token content)
        {
            return new ElementNode(nodeType, content);
        }

        internal ExpressionNode GetExpressionNode()
        {
            return new ExpressionNode();
        }

        internal ForNode GetForNode(Token @for,
            Token openParen,
            Token closeParen,
            Token openBrace,
            Token closeBrace,
            ChunkNode left,
            ChunkNode right,
            ChunkNode chunk,
            ExpressionNode middle)
        {
            return new ForNode(@for, openParen, closeParen, openBrace, closeBrace, left, right, chunk, middle);
        }

        internal IfNode GetIfNode(Token @if, Token openParen, Token closeParen, Token openBrace, Token closeBrace,
            ExpressionNode condition, ChunkNode chunk, Token @else = null, Token EopenBrace = null, Token EcloseBrace = null, ChunkNode elseChunk = null)
        {
            return new IfNode(@if, openParen, closeParen, openBrace, closeBrace, condition, chunk, @else, EopenBrace, EcloseBrace, elseChunk);
        }

        internal InvokerNode GetInvokerNode(Token name, Token openParen, Token closeParen, List<ExpressionNode> parms)
        {
            return new InvokerNode(name, openParen, closeParen, parms);
        }

        internal NewNode GetNewNode(Token @new, Token name, Token openParen, Token closeParen, List<ExpressionNode> parms, bool isArray)
        {
            return new NewNode(@new, name, openParen, closeParen, parms, isArray);
        }

        internal OperateNode GetOperateNode(Token op, OperateType operate, ExpressionNode left, ExpressionNode right)
        {
            return new OperateNode(op, operate, left, right);
        }

        internal OperateNode GetOperateNode(Token op, OperateType operate, ExpressionNode right)
        {
            return new OperateNode(op, operate, right);
        }

        internal OperateNode GetOperateNode(Token op, string castTypeName, ExpressionNode right)
        {
            return new OperateNode(op, castTypeName, right);
        }

        internal UsingNode GetUsingNode(Token content, UsingNode previousUsing)
        {
            return new UsingNode(content, previousUsing);
        }

        internal WhileNode GetWhileNode(Token @while, Token openParen, Token closeParen, Token openBrace, Token closeBrace, ExpressionNode condition, ChunkNode chunk)
        {
            return new WhileNode(@while, openParen, closeParen, openBrace, closeBrace, condition, chunk);
        }
    }
}
