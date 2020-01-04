using System;
using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.Testing;
using Xunit;

namespace EasyParse.CalculatorDemo.Tests
{
    public class ParseResultTests : ParserTestsBase
    {
        protected override Assembly XmlDefinitionAssembly => typeof(Calculator).Assembly;
        protected override string XmlDefinitionResourceName => "EasyParse.CalculatorDemo.ParserDefinition.xml";
        protected override Func<Lexer, Lexer> LexicalRules => (lexer => lexer);

        [Theory]
        [InlineData("1")]
        [InlineData("1+2")]
        [InlineData("1-2")]
        [InlineData("1*2")]
        [InlineData("1/2")]
        [InlineData("(1)")]
        [InlineData("1+(2*(3-4))/(5-2)")]
        [InlineData(" 1 +   (8 - 2*(3-4)  ) /   (5 -2)  ")]
        public void ExpressionRecognized(string expression) =>
            Assert.True(base.Recognized(expression));

        [Theory]
        [InlineData("")]
        [InlineData("       ")]
        [InlineData("1+")]
        [InlineData("1+(2*(3-4))/5-2)")]
        public void ExpressionNotRecognized(string expression) =>
            Assert.False(base.Recognized(expression));

        [Theory]
        [InlineData("1", 1)]
        [InlineData("1+2", 3)]
        [InlineData("1-2", -1)]
        [InlineData("2*3", 6)]
        [InlineData("4/2", 2)]
        [InlineData("4/3", 1)]
        [InlineData("3/4", 0)]
        [InlineData(" 1 +   (8 - 2*(3-4)  ) /   (5 -2)  ", 4)]
        public void ExpressionEvaluatesToResult(string expression, int result) =>
            Assert.Equal(result, base.Compiled(new Calculator(), expression));
    }
}
