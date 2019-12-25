using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.Parsing;

namespace ParserCompiler.TextGenerationDemo
{
    class ParserBuilder
    {
        private string ResourceName { get; }

        public ParserBuilder(string resourceName)
        {
            this.ResourceName = resourceName;
        }

        public Parser Build() => 
            Parser.FromXmlResource(Assembly.GetExecutingAssembly(), this.ResourceName, this.CreateLexer());

        private Lexer CreateLexer() =>
            new Lexer()
                .AddPattern(@"\d+", "n")
                .AddPattern(@"[\+\-]", "+")
                .AddPattern(@"[\*\/]", "*")
                .AddPattern(@"\(", "(")
                .AddPattern(@"\)", ")")
                .IgnorePattern(@"\s+");
    }
}