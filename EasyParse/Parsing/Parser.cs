using System.Collections.Generic;
using System.Xml.Linq;
using EasyParse.LexicalAnalysis;
using EasyParse.LexicalAnalysis.Tokens;
using EasyParse.Parsing.Collections;
using EasyParse.Parsing.Nodes;

namespace EasyParse.Parsing
{
    public class Parser
    {
        public Lexer Lexer { get; }
        private ShiftTable Shift { get; }

        private Parser(Lexer lexer, ShiftTable shift)
        {
            this.Lexer = lexer;
            this.Shift = shift;
        }

        public static Parser From(XDocument definition, Lexer lexer) =>
            new Parser(lexer, new ShiftTable(definition));

        public Node Parse(string input) =>
            this.Parse(this.Lexer.Tokenize(input));

        private Node Parse(IEnumerable<Token> input)
        {
            return new Error("Not parsed.");
        }
    }
}
