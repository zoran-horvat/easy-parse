using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParse.ParserGenerator.GrammarCompiler;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Testing;
using Xunit;

namespace EasyParse.Tests
{
    public class IgnoreLexemeCompilerTests : ParserTestsBase
    {
        protected override Assembly XmlDefinitionAssembly => 
            typeof(GrammarParser).Assembly;
        protected override string XmlDefinitionResourceName => 
            "EasyParse.ParserGenerator.GrammarCompiler.GrammarParserDefinition.xml";

        [Theory]
        [InlineData(0,
            "lexemes:",
            "rules:",
            "X -> a;")]
        [InlineData(1,
            "lexemes:",
            "ignore '';",
            "rules:",
            "X -> a;")]
        [InlineData(1,
            "lexemes:",
            "ignore 'something';",
            "ignore 'something';",
            "rules:",
            "X -> a;")]
        [InlineData(2,
            "lexemes:",
            "ignore 'something';",
            "ignore 'again';",
            "rules:",
            "X -> a;")]
        public void CompilesGrammarWithIgnoreLexemes_GrammarContainsSpecifiedNumberOfIgnores(int expectedCount, params string[] grammar) => 
            Assert.Equal(expectedCount, this.GetIgnoreLexemes(grammar).Count());

        [Theory]
        [InlineData("something",
            "lexemes:",
            "ignore 'something';",
            "rules:",
            "X -> a;")]
        [InlineData("again, and again",
            "lexemes:",
            "ignore 'again, and again';",
            "rules:",
            "X -> a;")]
        public void CompilesGrammarWithIgnoreLexeme_GrammarContainsIgnorePattern(string ignore, params string[] grammar) => 
            Assert.Equal(ignore, this.CompiledGrammar(grammar).IgnoreLexemes.First().Pattern.ToString());

        [Theory]
        [InlineData(0,
            "lexemes:",
            "ignore 'something';",
            "rules:",
            "X -> a;")]
        [InlineData(2, 
            "lexemes:",
            "ignore 'something';",
            "n matches '[A-Z]';",
            "g matches '@';",
            "rules:",
            "X -> a;")]
        [InlineData(1, 
            "lexemes:",
            "ignore 'something';",
            "n matches @'[A-Z]';",
            "n matches @'[A-Z]';",
            "rules:",
            "X -> a;")]
        [InlineData(2,
            "lexemes:",
            "a matches '@';",
            "x matches @'[a-z]';",
            "",
            "# C - string content",
            "# q - single quote",
            "# p - plain text segment",
            "# a - verbatim string indicator",
            "",
            "rules:",
            "S -> q q;",
            "S -> q C q;",
            "S -> V;",
            "C -> p;",
            "V -> a q q;")]
        public void CompilesGrammarWithLexemePatterns_GrammarContainsSpecifiedNumberOfPatterns(int expectedCount, params string[] grammar) => 
            Assert.Equal(expectedCount, this.GetLexemePatterns(grammar).Count());

        private IEnumerable<IgnoreLexeme> GetIgnoreLexemes(string[] grammar) =>
            this.CompiledGrammar(grammar).IgnoreLexemes;

        private IEnumerable<LexemePattern> GetLexemePatterns(string[] grammar) =>
            this.CompiledGrammar(grammar).LexemePatterns;

        private Grammar CompiledGrammar(string[] grammar) =>
            base.Compiled<Grammar>(new Compiler(), this.OnCompileError, grammar);

        private void OnCompileError(object result) =>
            Assert.True(false, $"{result}");
    }
}
