using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.ParserGenerator;
using EasyParse.Testing;
using Xunit;

namespace EasyParse.Tests
{
    public class GrammarGrammarTests : ParserTestsBase
    {
        protected override Assembly XmlDefinitionAssembly => 
            typeof(GrammarLoader).Assembly;
        protected override string XmlDefinitionResourceName => 
            "EasyParse.ParserGenerator.GrammarParserDefinition.xml";

        protected override Lexer Lexer => 
            GrammarParser.CreateLexer();

        [Theory]
        [InlineData("U -> n")]
        public void RecognizesValidGrammar(string grammar) =>
            base.Recognized(grammar);
    }
}
