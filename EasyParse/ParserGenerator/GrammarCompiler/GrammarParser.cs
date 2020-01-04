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

        public static Lexer AddLexicalRules(Lexer lexer) => lexer
            .AddPattern(@"'[^']*'", "q");

        public static Lexer AddStringLexicalRules(Lexer lexer) => lexer
            .AddPattern(@"'", "q")
            .AddPattern(@"''", "d")
            .AddPattern(@"[^'@]+", "p");
    }
}
