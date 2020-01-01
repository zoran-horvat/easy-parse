using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.ParserGenerator.GrammarCompiler;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Parsing;
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

        protected override Func<Lexer, Lexer> LexicalRules =>
            GrammarParser.AddLexicalRules;

        [Theory]
        [InlineData(0,
            "lexemes:",
            "rules:",
            "X -> a")]
        [InlineData(1,
            "lexemes:",
            "ignore: ''",
            "rules:",
            "X -> a")]
        [InlineData(2,
            "lexemes:",
            "ignore: ''",
            "ignore: ''",
            "rules:",
            "X -> a")]
        public void CompilesGrammarWithIgnoreLexemes_GrammarContainsSpecifiedNumberOfIgnores(int expectedCount, params string[] grammar)
        {
            var x = Parser.FromXmlResource(this.XmlDefinitionAssembly, this.XmlDefinitionResourceName, this.LexicalRules).Lexer.Tokenize(grammar);
            Grammar g = this.CompiledGrammar(grammar);
            Assert.Equal(expectedCount, this.GetIgnoreLexemes(grammar).Count());
        }

        private IEnumerable<IgnoreLexeme> GetIgnoreLexemes(string[] grammar) =>
            this.CompiledGrammar(grammar).IgnoreLexemes;

        private Grammar CompiledGrammar(string[] grammar) =>
            base.Compiled<Grammar>(new Compiler(), this.OnCompileError, grammar);

        private void OnCompileError(object result) =>
            Assert.True(false, $"{result}");
    }
}
