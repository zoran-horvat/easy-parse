using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.ParserGenerator.Models.Builders;
using EasyParse.Testing;
using Xunit;

namespace EasyParse.CalculatorDemo.Tests
{
    public class ParseResultTests : ParserTestsBase
    {
        protected override Assembly XmlDefinitionAssembly => typeof(Calculator).Assembly;
        protected override string XmlDefinitionResourceName => "EasyParse.CalculatorDemo.ParserDefinition.xml";
        protected override Lexer Lexer => ParserBuilder.CreateLexer();

        [Theory]
        [InlineData("1")]
        [InlineData("1+2")]
        [InlineData("1-2")]
        [InlineData("1*2")]
        [InlineData("1/2")]
        [InlineData("(1)")]
        [InlineData("1+(2*(3-4))/(5-2)")]
        public void ExpressionRecognized(string expression) =>
            Assert.True(base.Recognized(expression));

        [Theory]
        [InlineData("")]
        [InlineData("1+")]
        [InlineData("1+(2*(3-4))/5-2)")]
        public void ExpressionNotRecognized(string expression) =>
            Assert.False(base.Recognized(expression));
    }
}
