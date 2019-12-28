using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.ParserGenerator;
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

        protected override Lexer Lexer =>
            GrammarParser.CreateLexer();


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
        public void CompilesGrammar(params string[] grammar) => 
            Assert.IsType<Grammar>(base.Compiled(new Compiler(), grammar));
    }
}
