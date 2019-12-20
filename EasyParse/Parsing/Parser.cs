using System.Xml.Linq;
using EasyParse.LexicalAnalysis;

namespace EasyParse.Parsing
{
    public class Parser
    {
        public Lexer Lexer { get; }

        private Parser(Lexer lexer)
        {
            this.Lexer = lexer;
        }

        public static Parser From(XDocument definition, Lexer lexer) =>
            new Parser(lexer);
    }
}
