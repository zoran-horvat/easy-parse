using System;
using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.ParserGenerator.GrammarCompiler;
using EasyParse.Testing;
using Xunit;

namespace EasyParse.Tests
{
    public class StringCompilerTests : ParserTestsBase
    {
        protected override Assembly XmlDefinitionAssembly =>
            typeof(StringCompiler).Assembly;

        protected override string XmlDefinitionResourceName =>
            "EasyParse.ParserGenerator.GrammarCompiler.StringParserDefinition.xml";

        protected override Func<Lexer, Lexer> LexicalRules =>
            GrammarParser.AddStringLexicalRules;

        [Fact]
        public void CompilesEmptyString() => 
            Assert.Equal(string.Empty, base.CompiledLine<string>(new StringCompiler(), this.Fail, "''"));

        [Theory]
        [InlineData("'something'", "something")]
        [InlineData("'something, again'", "something, again")]
        [InlineData("'     ***   2394&^Q*&^#$*^'", "     ***   2394&^Q*&^#$*^")]
        public void CompilesString_ReturnsExpectedValue(string input, string expected) =>
            Assert.Equal(expected, base.CompiledLine<string>(new StringCompiler(), this.Fail, input));

        private void Fail(object result) =>
            Assert.True(false, $"{result}");
    }
}
