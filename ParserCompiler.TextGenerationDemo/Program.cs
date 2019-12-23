using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.LexicalAnalysis.Tokens;
using EasyParse.Parsing;

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
            
            Console.WriteLine();
            Console.WriteLine($"Tokens: {tokensReport}");
            
            ParsingResult result = parser.Parse(line);
            Console.WriteLine();
            Console.WriteLine($"Syntax tree:{Environment.NewLine}{result}");

            Console.WriteLine();
            Console.WriteLine($"{line} = {(result.IsSuccess ? result.Compile(Calculator) : result.ErrorMessage)}");
            Console.WriteLine(new string('-', 50));
        }

        private static object Calculator(string label, object[] arguments) =>
            label == "n" && int.TryParse((string) arguments[0], out int number) ? number
            : label == "n" ? "<Overflow>"
            : label == "A" && arguments.Length == 1 ? arguments[0]
            : label == "A" && arguments[0] is string s1 ? s1
            : label == "A" && arguments[2] is string s2 ? s2
            : label == "A" && arguments[1].Equals("+") ? Add((int) arguments[0], (int) arguments[2])
            : label == "A" && arguments[1].Equals("-") ? Subtract((int) arguments[0], (int) arguments[2])
            : label == "E" ? arguments[0]
            : arguments.Length == 1 ? arguments[0]
            : "<Internal error>";

        private static object Add(int a, int b) =>
            a < 0 && b > 0 ? a + b
            : a > 0 && b < 0 ? a + b
            : a < 0 && b < 0 && int.MinValue - a <= b ? a + b
            : int.MaxValue - a >= b ? (object)(a + b)
            : "<Overflow>";

        private static object Subtract(int a, int b) =>
            a > 0 && b > 0 ? a - b
            : a < 0 && b < 0 ? a - b
            : a < 0 && b > 0 && int.MinValue + b <= a ? a - b
            : int.MinValue - a <= b ? (object) (a - b)
            : "<Overflow>";
    }
}
