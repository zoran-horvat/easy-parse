﻿using System.Reflection;
using EasyParse.ParserGenerator.GrammarCompiler;
using EasyParse.Testing;
using Xunit;

namespace EasyParse.Tests
{
    public class GrammarGrammarTests : GeneratedParserTestsBase
    {
        protected override Assembly XmlDefinitionAssembly => 
            typeof(GrammarParser).Assembly;
        protected override string XmlDefinitionResourceName => 
            "EasyParse.ParserGenerator.GrammarCompiler.GrammarParserDefinition.xml";

        [Theory]
        [InlineData(
            "lexemes:",
            "start: A;",
            "# Comment on its own line",
            "       # Comment on a line containing blank spaces",
            "rules:",
            "A -> M; # Comment on a line with a rule",
            "A -> A '+' M;",
            "A -> A '-' M;",
            "U -> n;",
            "",
            "U -> '(' A ')';",
            "M -> U;",
            "M -> M '*' U;",
            "M -> M '/' U;")]
        [InlineData(
            "lexemes:",
            "start: G;",
            "rules:",
            "# L - Line containing a single rule",
            "# R - Rule",
            "# B - Rule body",
            "# S - symbol (terminal or non-terminal)",
            "# E - Line ending (optional comment followed by end of line character)",
            "# n - Non-terminal",
            "# t - Terminal",
            "# a - Arrow (->)",
            "# e - End of line",
            "",
            "G -> L;",
            "G -> GL;",
            "L -> RE;",
            "L -> E;",
            "E -> ce;",
            "E -> e;",
            "R -> naB;",
            "B -> S;",
            "B -> BS;",
            "S -> t;",
            "S -> n;")]
        [InlineData(
            "",
            "# Grammar beginning with a blank line and a comment",
            "lexemes:",
            "start: S;",
            "rules:",
            "S -> x;")]
        public void RecognizesValidGrammar(params string[] grammar) => 
            Assert.True(base.Recognized(grammar));

        [Theory]
        [InlineData(
            "lexemes:",
            "",
            "start: A;",
            "rules:",
            "# Comment on its own line",
            "A -> M # Comment on a line with a rule",
            "U -> n",
            "",
            "M -- U")]
        public void DoesNotRecognizeInvalidGrammar(params string[] grammar) =>
            Assert.False(base.Recognized(grammar));
    }
}
