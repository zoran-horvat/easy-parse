using System.Xml.Linq;
using EasyParse.ParserGenerator.GrammarCompiler;
using EasyParse.ParserGenerator.Models;
using EasyParse.Parsing;
using Xunit;

namespace EasyParse.Tests
{
    public class LexingErrorsTests
    {
        private Parser Parser { get; }

        public LexingErrorsTests()
        {
            string[] grammar = new[]
            {
                "lexemes:",
                "",
                "start: A;",
                "rules:",
                "A -> 'ba';",
                "A -> 'ba' B;",
                "B -> 'na';",
                "B -> 'na' B;"
            };

            ParserDefinition definition = new GrammarParser().Parse(grammar).BuildParser();
            this.Parser = Parser.From(definition);
        }

        [Theory]
        [InlineData("bananas")]
        [InlineData("banan")]
        public void InvalidLexeme_ParserReturnsLexicalError(string text) => 
            Assert.IsType<string>(this.Parser.Parse(text).Error);
    }
}
