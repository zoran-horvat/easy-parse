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
            "EasyParse.ParserGenerator.GrammarCompiler.GrammarParserDefinition.xml";

        protected override Lexer Lexer =>
            GrammarParser.CreateLexer();

        [Theory]
        [InlineData(
            "# Comment on its own line",
            "       # Comment on a line containing blank spaces",
            "A -> M # Comment on a line with a rule",
            "A -> A+M",
            "A -> A-M",
            "U -> n",
            "",
            "U -> (A)",
            "M -> U",
            "M -> M*U",
            "M -> M/U")]
        public void RecognizesValidGrammar(params string[] grammar) => 
            Assert.True(base.Recognized(grammar));

        [Theory]
        [InlineData(
            "# Comment on its own line",
            "A -> M # Comment on a line with a rule",
            "U -> n",
            "",
            "M -- U")]
        public void DoesNotRecognizeInvalidGrammar(params string[] grammar) =>
            Assert.False(base.Recognized(grammar));
    }
}
