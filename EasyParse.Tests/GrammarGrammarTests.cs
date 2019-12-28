using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.ParserGenerator;
using EasyParse.Parsing;
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
        [InlineData(
            "A -> M",
            "U -> n",
            "",
            "M -> U")]
        public void RecognizesValidGrammar(params string[] grammar) => 
            Assert.True(base.Recognized(grammar));

        [Theory]
        [InlineData(
            "A -> M",
            "U -> n",
            "",
            "M -- U")]
        public void DoesNotRecognizeInvalidGrammar(params string[] grammar) =>
            Assert.False(base.Recognized(grammar));
    }
}
