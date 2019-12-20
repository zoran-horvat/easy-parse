using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.LexicalAnalysis;
using EasyParse.LexicalAnalysis.Tokens;

namespace ParserCompiler.TextGenerationDemo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Lexer lexer = CreateLexer();

            Console.WriteLine("Enter expressions to evaluate (blank line to exit):");
            foreach (string line in Console.In.ReadLinesUntil(string.Empty))
            {
                Process(lexer, line);
            }
        }

        private static Lexer CreateLexer() =>
            new Lexer()
                .AddPattern(@"\d+", "n")
                .AddPattern(@"\+", "+")
                .IgnorePattern(@"\s+");

        private static void Process(Lexer lexer, string line)
        {
            List<Token> tokens = lexer.Tokenize(line).ToList();
            string tokensReport = string.Join(" ", tokens.Select(x => $"{x}"));
            Console.WriteLine($"Tokens: {tokensReport}");
        }
    }
}
