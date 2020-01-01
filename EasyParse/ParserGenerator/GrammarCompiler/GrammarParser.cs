using System.Collections.Generic;
using System.Reflection;
using EasyParse.LexicalAnalysis;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Parsing;

namespace EasyParse.ParserGenerator.GrammarCompiler
{
    public class GrammarParser
    {
        public Grammar Parse(IEnumerable<string> text) =>
            (Grammar)this.CreateParser().Parse(text).Compile(new Compiler());

        private Parser CreateParser() =>
            Parser.FromXmlResource(Assembly.GetExecutingAssembly(), "EasyParse.ParserGenerator.GrammarCompiler.GrammarParserDefinition.xml", AddLexicalRules);

        public static Lexer AddLexicalRules(Lexer lexer) =>
            lexer
                .AddPattern("[A-Z]", "n")
                .AddPattern(@"[a-z\(\)\+\-\*\/,\.#]", "t")
                .AddPattern(@"\->", "a")
                .AddPattern(@"\n([ \t]*(#[^\n]*)?\n)*", "e")
                .AddPattern(@"lexemes:", "l")
                .AddPattern(@"ignore:", "i")
                .AddPattern(@"'[^']*'", "q")
                .AddPattern(@"rules:", "r")
                .IgnorePattern(@"[ \t]+")
                .IgnorePattern(@"#[^\n]*");
    }
}
