using System.Collections.Generic;

namespace Core.SyntacticAnalysis.Nodes
{
    public class ChunkNode : SyntaxNode
    {
        public List<SyntaxNode> Nodes = new List<SyntaxNode>();

        public ChunkNode()
        {
            NodeType = NodeType.Chunk;
        }

        internal void AddNode(SyntaxNode node) => Nodes.Add(node);
        internal void AddFirstNode(SyntaxNode node) => Nodes.Insert(0, node);
    }
}