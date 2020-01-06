using System.Linq;
using System.Reflection;
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

        [Theory]
        [InlineData(
            "lexemes:",
            "# Comment on its own line",
            "       # Comment on a line containing blank spaces",
            "start: A;",
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
            @"lexemes:",
            @"plaintext matches @'[^\\]+';",
            @"newLine matches @'\\n';",
            @"carriageReturn matches @'\\r';",
            @"tab matches @'\\t';",
            @"backslash matches @'\\\\';",
            @"quote matches '\\\\\'';",
            @"start: String;",
            @"",
            @"rules:",
            @"String -> Segment;",
            @"String -> String Segment;",
            @"Segment -> plaintext;",
            @"Segment -> newLine;",
            @"Segment -> carriageReturn;",
            @"Segment -> tab;",
            @"Segment -> backslash;",
            @"Segment -> quote;")]
        public void CompilesGrammar(params string[] grammar) => 
            Assert.IsType<Grammar>(base.Compiled(new Compiler(), grammar));
    }
}
