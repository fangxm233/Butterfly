using System;
using System.Diagnostics;
using Compiler.LexicalAnalysis;
using Compiler.SyntacticAnalysis;

namespace Compiler
{
    public class CompilerControler
    {
        public static void Compile(string[] files)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            Lexer.Scan(files);
            
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