using Core.LexicalAnalysis;
using Core.SemanticAnalysis;
using Core.SyntacticAnalysis;
using System;
using System.Diagnostics;

namespace Core
{
    public class Compiler
    {
        public static void Compile(string[] files)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            Lexer lexer = new Lexer();
            lexer.Scan(files);
            Parser parser = new Parser();
            parser.Parse(files[0], lexer);
            SemanticAnalyzer analyzer = new SemanticAnalyzer();
            analyzer.Analyze();
            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds);
            //Lexer.Next();
            //while (Lexer.NextToken.Type != TokenType.EOF)
            //{
            //    Console.WriteLine(Lexer.NextToken);
            //    Lexer.Next();
            //}
            Console.ReadKey();
        }
    }
}
//Snippet