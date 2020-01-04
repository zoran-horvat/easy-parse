using System;
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
    public class GrammarCompilerTests : ParserTestsBase
    {
        protected override Assembly XmlDefinitionAssembly => 
            typeof(GrammarParser).Assembly;
        protected override string XmlDefinitionResourceName => 
            "EasyParse.ParserGenerator.GrammarCompiler.GrammarParserDefinition.xml";

        protected override Func<Lexer, Lexer> LexicalRules =>
            GrammarParser.AddLexicalRules;

        [Theory]
        [InlineData(
            "lexemes:",
            "# Comment on its own line",
            "       # Comment on a line containing blank spaces",
            "rules:",
            "A -> M # Comment on a line with a rule",
            "A -> A+M",
            "A -> A-M",
            "U -> n",
            "",
            "U -> (A)",
            "M -> U",
            "M -> M*U",
            "M -> M/U")]
        public void CompilesGrammar(params string[] grammar)
        {
            var x = Parser.FromXmlResource(this.XmlDefinitionAssembly, this.XmlDefinitionResourceName, this.LexicalRules).Lexer.Tokenize(grammar).ToArray();
            Assert.IsType<Grammar>(base.Compiled(new Compiler(), grammar));
        }
    }
}
