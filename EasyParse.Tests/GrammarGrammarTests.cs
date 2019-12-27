using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.ParserGenerator;
using EasyParse.Testing;

namespace EasyParse.Tests
{
    public class GrammarGrammarTests : ParserTestsBase
    {
        protected override Assembly XmlDefinitionAssembly => typeof(GrammarLoader).Assembly;
        protected override string XmlDefinitionResourceName => "EasyParse.ParserGenerator.GrammarParserDefinition.xml";
        protected override Lexer Lexer => new Lexer();
    }
}
