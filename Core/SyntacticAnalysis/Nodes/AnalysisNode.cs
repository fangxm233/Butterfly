using Core.LexicalAnalysis;

namespace Core.SyntacticAnalysis.Nodes
{
    public enum NodeType
    {
        Assign,
        Chunk,
        Command,
        DefineVariable,
        DefSpecifier,
        Element,
        For,
        If,
        Invoker,
        New,
        Operate,
        While
    }

    public class AnalysisNode
    {
        public int Line { get; internal set; }
        public int File { get; internal set; }
        public NodeType NodeType { get; internal set; }

        public AnalysisNode()
        {
            Line = Lexer.Line;
            File = Lexer.FileIndex;
        }
    }
}