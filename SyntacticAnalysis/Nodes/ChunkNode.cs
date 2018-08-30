using System.Collections.Generic;

namespace Compiler.SyntacticAnalysis.Nodes
{
    public class ChunkNode : AnalysisNode
    {
        public List<AnalysisNode> Sentences;
    }
}