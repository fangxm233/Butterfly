using System;
using System.Diagnostics;
using Core.LexicalAnalysis;
using Core.SemanticAnalysis;
using Core.SyntacticAnalysis;

namespace Core
{
    public class Compiler
    {
        public static void Compile(string[] files)
        {
            //Stopwatch sw = new Stopwatch();
            Lexer.Scan(files);
            //sw.Start();
            Parser.Match(files[0]);
            SemanticAnalyzer.Analyze();
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