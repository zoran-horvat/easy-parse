using EasyParse.Parsing;
using EasyParse.Parsing.Nodes.Errors;
using EasyParse.Testing;
using Xunit;

namespace EasyParse.Tests
{
    public class ParsingErrorsTests : AnyGrammarParserTestsBase
    {
        private class Compiler : MethodMapCompiler
        {
            public string A(string ba) => ba;
            public string A(string ba, string b) => $"{ba}{b}";
            public string B(string na) => na;
            public string B(string na, string b) => $"{na}{b}";
        }

        public ParsingErrorsTests() : base(new []
            {
                "lexemes:",
                "",
                "start: A;",
                "rules:",
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
            Assert.IsType<LexingError>(base.Parsed(text).Error);

        [Theory]
        [InlineData("bananas")]
        [InlineData("banan")]
        public void InvalidLexeme_CompilerReturnsLexicalError(string text) => 
            Assert.IsType<LexingError>(base.Compiled(new Compiler(), text));

        [Theory]
        [InlineData("ba")]
        public void ShortInput_ParserReturnsUnexpectedEndOfInput(string text) =>
            Assert.IsType<UnexpectedEndOfInput>(base.Parsed(text).Error);

        [Theory]
        [InlineData("ba")]
        public void ShortInput_CompileReturnsUnexpectedEndOfInput(string text) =>
            Assert.IsType<UnexpectedEndOfInput>(base.Compiled(new Compiler(), text));
    }
}
