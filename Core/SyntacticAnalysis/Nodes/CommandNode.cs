using Core.SyntacticAnalysis.Definitions;

namespace Core.SyntacticAnalysis.Nodes
{
    public enum CommandType
    {
        Continue,
        Break,
        Return
    }

    public class CommandNode : AnalysisNode
    {
        public CommandType Type { get; internal set; }
        public ExpressionNode Expression { get; internal set; }

        public AnalysisNode Loop { get; internal set; }
        public FunctionDefinition Function { get; internal set; }

        public CommandNode(CommandType command)
        {
            Type = command;
            NodeType = NodeType.Command;
        }
        public CommandNode(CommandType command, ExpressionNode expression)
        {
            Type = command;
            NodeType = NodeType.Command;
            Expression = expression;
        }
    }
}