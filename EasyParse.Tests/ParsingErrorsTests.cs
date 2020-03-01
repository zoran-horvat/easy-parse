using System;
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
        [InlineData("bananas", typeof(LexingError))]
        [InlineData("banan", typeof(LexingError))]
        [InlineData("ba", typeof(UnexpectedEndOfInput))]
        [InlineData("banaba", typeof(SyntaxError))]
        [InlineData("babana", typeof(SyntaxError))]
        public void InvalidText_ParserReturnsError(string text, Type errorType) => 
            Assert.IsType(errorType, base.Parsed(text).Error);

        [Theory]
        [InlineData("bananas", typeof(LexingError))]
        [InlineData("banan", typeof(LexingError))]
        [InlineData("ba", typeof(UnexpectedEndOfInput))]
        [InlineData("banaba", typeof(SyntaxError))]
        [InlineData("babana", typeof(SyntaxError))]
        public void InvalidText_CompilerReturnsError(string text, Type errorType) => 
            Assert.IsType(errorType, base.Compiled(new Compiler(), text));
    }
}
