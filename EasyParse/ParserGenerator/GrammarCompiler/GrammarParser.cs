using System.Collections.Generic;
using System.Reflection;
using EasyParse.Parsing;
using Grammar = EasyParse.ParserGenerator.Models.Rules.Grammar;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class GrammarParser
    {
        public Grammar Parse(IEnumerable<string> text) =>
            (Grammar)this.TryParse(text);

        public object TryParse(IEnumerable<string> text) =>
            this.CreateParser().Parse(text).Compile(new SymbolCompiler());

        private Parser CreateParser() => Parser.FromXmlResource(
            Assembly.GetExecutingAssembly(), "EasyParse.ParserGenerator.GrammarCompiler.GrammarParserDefinition.xml");
    }
}
