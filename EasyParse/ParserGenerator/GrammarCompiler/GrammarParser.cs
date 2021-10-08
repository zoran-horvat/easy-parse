using System.Collections.Generic;
using System.Reflection;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Parsing;
using Grammar = EasyParse.ParserGenerator.Models.Rules.Grammar;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class GrammarParser
    {
        public Grammar Parse(IEnumerable<string> text) => 
            (Grammar)this.CreateParser().Parse(text).Compile(new Compiler());

        private Parser CreateParser() => Parser.FromXmlResource(
            Assembly.GetExecutingAssembly(), "EasyParse.ParserGenerator.GrammarCompiler.GrammarParserDefinition.xml");
    }
}
