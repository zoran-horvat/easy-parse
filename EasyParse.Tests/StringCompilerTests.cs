﻿using System.Reflection;
using EasyParse.ParserGenerator.GrammarCompiler;
using EasyParse.Testing;
using Xunit;

namespace EasyParse.Tests
{
    public class StringCompilerTests : GeneratedParserTestsBase
    {
        protected override Assembly XmlDefinitionAssembly =>
            typeof(StringCompiler).Assembly;

        protected override string XmlDefinitionResourceName =>
            "EasyParse.ParserGenerator.GrammarCompiler.StringParserDefinition.xml";

        [Theory]
        [InlineData("something", "something")]
        [InlineData("something, again", "something, again")]
        [InlineData("     ***   2394&^Q*&^#$*^", "     ***   2394&^Q*&^#$*^")]
        [InlineData("@", "@")]
        public void CompilesString_ReturnsExpectedValue(string input, string expected) =>
            Assert.Equal(expected, this.Compile(input));

        [Theory]
        [InlineData(@"\\\\\'", @"\\'")]
        [InlineData(@"some\\thing", @"some\thing")]
        [InlineData(@"some\'thing", "some'thing")]
        [InlineData(@"some\nthing", "some\nthing")]
        [InlineData(@"some\rthing", "some\rthing")]
        [InlineData(@"some\tthing", "some\tthing")]
        public void CompilesEscapedString_ReturnsExpectedValue(string input, string expected) =>
            Assert.Equal(expected, this.Compile(input));

        private string Compile(string input) =>
            base.CompiledLine<string>(new StringCompiler(), this.Fail, input);

        private void Fail(object result) =>
            Assert.True(false, $"{result}");
    }
}
