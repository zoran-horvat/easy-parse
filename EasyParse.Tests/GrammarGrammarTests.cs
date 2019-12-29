using System;
using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.ParserGenerator.GrammarCompiler;
using EasyParse.Testing;
using Xunit;

namespace EasyParse.Tests
{
    public class GrammarGrammarTests : ParserTestsBase
    {
        protected override Assembly XmlDefinitionAssembly => 
            typeof(GrammarParser).Assembly;
        protected override string XmlDefinitionResourceName => 
            "EasyParse.ParserGenerator.GrammarCompiler.GrammarParserDefinition.xml";

        protected override Func<Lexer, Lexer> LexicalRules =>
            GrammarParser.AddLexicalRules;

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
        [InlineData(
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
            "G -> L",
            "G -> GL",
            "L -> RE",
            "L -> E",
            "E -> ce",
            "E -> e",
            "R -> naB",
            "B -> S",
            "B -> BS",
            "S -> t",
            "S -> n")]
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
