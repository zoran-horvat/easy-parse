using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParse.ParserGenerator.GrammarCompiler;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.Testing;
using Xunit;

namespace EasyParse.Tests
{
    public class ConstantsTests : ParserTestsBase
    {
        protected override Assembly XmlDefinitionAssembly => 
            typeof(GrammarParser).Assembly;
        protected override string XmlDefinitionResourceName => 
            "EasyParse.ParserGenerator.GrammarCompiler.GrammarParserDefinition.xml";

        [Theory]
        [InlineData(0,
            "lexemes:",
            "start: A;",
            "rules:",
            "A -> x;")]
        [InlineData(1,
            "lexemes:",
            "start: A;",
            "rules:",
            "A -> 'const';")]
        [InlineData(3,
            "lexemes:",
            "start: C;",
            "rules:",
            "A -> 'something';",
            "B -> 'again';",
            "C -> 'and again';")]
        public void GrammarContainsConstantLexemes_ReturnsExpectedNumberOfConstantLexemes(int expectedCount, params string[] grammar) =>
            Assert.Equal(expectedCount, this.GetConstantLexemes(grammar).Count());

        [Theory]
        [InlineData("const",
            "lexemes:",
            "start: A;",
            "rules:",
            "A -> 'const';")]
        [InlineData("something, again",
            "lexemes:",
            "start: A;",
            "rules:",
            "A -> 'something, again';")]
        public void GrammarContainsOneConstantLexeme_ReturnsThatConstantLexeme(string expectedValue, params string[] grammar) =>
            Assert.Equal(expectedValue, this.GetFirstConstantLexeme(grammar).ConstantValue);

        private ConstantLexeme GetFirstConstantLexeme(string[] grammar) =>
            this.GetConstantLexemes(grammar).First();

        private IEnumerable<ConstantLexeme> GetConstantLexemes(string[] grammar) =>
            base.Compiled<Grammar>(new Compiler(), this.Fail, grammar).ConstantLexemes;

        private void Fail(object result) => Assert.True(false, $"{result}");
    }
}
