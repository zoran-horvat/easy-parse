using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.LexicalAnalysis.Tokens;
using EasyParse.Parsing;
using EasyParse.Parsing.Nodes;

namespace ParserCompiler.TextGenerationDemo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Parser parser = new ParserBuilder("ParserCompiler.TextGenerationDemo.ParserDefinition.xml").Build();

            Console.WriteLine("Enter expressions to evaluate (blank line to exit):");
            foreach (string line in Console.In.ReadLinesUntil(string.Empty))
            {
                Process(parser, line);
            }
        }

        private static void Process(Parser parser, string line)
        {
            List<Token> tokens = parser.Lexer.Tokenize(line).ToList();
            string tokensReport = string.Join(" ", tokens.Select(x => $"{x}"));
            Console.WriteLine($"Tokens: {tokensReport}");
            TreeElement result = parser.Parse(line);
            Console.WriteLine($"Result: {result}");
        }
    }
}
