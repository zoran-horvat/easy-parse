using System.Collections.Generic;
using EasyParse.ParserGenerator.GrammarCompiler;
using EasyParse.ParserGenerator.Models;
using EasyParse.Parsing;

namespace EasyParse.Testing
{
    public abstract class AnyGrammarParserTestsBase : ParserTestsBase
    {
        private ParserDefinition ParserDefinition { get; }

        protected AnyGrammarParserTestsBase(IEnumerable<string> grammar)
        {
            this.ParserDefinition = new GrammarParser().Parse(grammar).BuildParser();
        }

        protected override Parser CreateParser() => 
            Parser.From(this.ParserDefinition);
    }
}