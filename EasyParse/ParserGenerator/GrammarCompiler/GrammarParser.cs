using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EasyParse.LexicalAnalysis;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.Parsing;
using Match = System.Text.RegularExpressions.Match;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class GrammarParser
    {
        public Grammar Parse(IEnumerable<string> text) =>
            (Grammar)this.CreateParser().Parse(text).Compile(new Compiler());

        private Parser CreateParser() =>
            Parser.FromXmlResource(Assembly.GetExecutingAssembly(), "EasyParse.ParserGenerator.GrammarCompiler.GrammarParserDefinition.xml", CreateLexer());

        public static Lexer CreateLexer() =>
            new Lexer()
                .AddPattern("[A-Z]", "n")
                .AddPattern(@"[a-z\(\)\+\-\*\/,\.#]", "t")
                .AddPattern(@"\->", "a")
                .AddPattern(@"\n", "e")
                .AddPattern(@"#[^\n]*", "c")
                .IgnorePattern(@"\s");
    }
}
