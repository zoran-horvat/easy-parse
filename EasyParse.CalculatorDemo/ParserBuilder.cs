using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.Parsing;

namespace EasyParse.CalculatorDemo
{
    public class ParserBuilder
    {
        private string ResourceName { get; }

        public ParserBuilder(string resourceName)
        {
            this.ResourceName = resourceName;
        }

        public Parser Build() => 
            Parser.FromXmlResource(Assembly.GetExecutingAssembly(), this.ResourceName, CreateLexer());

        public static Lexer CreateLexer() =>
            new Lexer()
                .AddPattern(@"\d+", "n")
                .AddPattern(@"[\+\-]", "+")
                .AddPattern(@"[\*\/]", "*")
                .AddPattern(@"\(", "(")
                .AddPattern(@"\)", ")")
                .IgnorePattern(@"\s+");
    }
}