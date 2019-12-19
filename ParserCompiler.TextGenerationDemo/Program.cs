using EasyParse.LexicalAnalysis;

namespace ParserCompiler.TextGenerationDemo
{
    public static class Program
    {
        public static void Main(string[] args)
        {

        }

        private static Lexer CreateLexer() =>
            new Lexer()
                .AddPattern("\\d+", "n")
                .AddPattern("+", "+")
                .IgnorePattern("\\s+");
    }
}
