using EasyParse.Testing;
using Xunit;

namespace EasyParse.Tests
{
    public class LexingErrorsTests : AnyGrammarParserTestsBase
    {
        public LexingErrorsTests() : base(new []
            {
                "lexemes:",
                "",
                "start: A;",
                "rules:",
                "A -> 'ba';",
                "A -> 'ba' B;",
                "B -> 'na';",
                "B -> 'na' B;"
            })
        {
        }

        [Theory]
        [InlineData("bananas")]
        [InlineData("banan")]
        public void InvalidLexeme_ParserReturnsLexicalError(string text) => 
            Assert.IsType<string>(base.Parsed(text).Error);
    }
}
