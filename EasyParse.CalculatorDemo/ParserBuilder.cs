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
            Parser.FromXmlResource(Assembly.GetExecutingAssembly(), this.ResourceName, AddLexicalRules);

        public static Lexer AddLexicalRules(Lexer lexer) =>
            lexer
                .AddPattern(@"\d+", "n")
                .AddPattern(@"[\+\-]", "+")
                .AddPattern(@"[\*\/]", "*")
                .AddPattern(@"\(", "(")
                .AddPattern(@"\)", ")");
    }
}