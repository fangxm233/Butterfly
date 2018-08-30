using System.Collections.Generic;

namespace Core.SyntacticAnalysis.Nodes
{
    public class ChunkNode : AnalysisNode
    {
        public List<AnalysisNode> Nodes = new List<AnalysisNode>();

        public ChunkNode()
        {
            NodeType = NodeType.Chunk;
        }

        internal void AddNode(AnalysisNode node) => Nodes.Add(node);
    }
}