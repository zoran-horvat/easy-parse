using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EasyParse.ParserGenerator.Models;

namespace EasyParse.LexicalAnalysis
{
    internal static class LexerLoader
    {
        public static Lexer From(XDocument definition) =>
            new Lexer()
                .AddIgnorePatterns(definition)
                .AddConstants(definition)
                .AddPatterns(definition);

        public static Lexer From(ParserDefinition definition) =>
            new Lexer()
                .AddIgnorePatterns(definition.Grammar.IgnoreLexemes.Select(ignore => ignore.Pattern.ToString()))
                .AddConstants(definition.Grammar.ConstantLexemes.Select(constant => constant.ConstantValue))
                .AddPatterns(definition.Grammar.LexemePatterns.Select(pattern => (pattern.Pattern.ToString(), pattern.Name)));

        private static Lexer AddIgnorePatterns(this Lexer lexer, XDocument definition) =>
            lexer.AddIgnorePatterns(IgnorePatterns(definition));

        private static Lexer AddIgnorePatterns(this Lexer lexer, IEnumerable<string> ignores) =>
            ignores.Aggregate(lexer, (cur, ignore) => cur.IgnorePattern(ignore));

        private static Lexer AddConstants(this Lexer lexer, XDocument definition) =>
            lexer.AddConstants(ConstantLexemes(definition));

        private static Lexer AddConstants(this Lexer lexer, IEnumerable<string> constants) =>
            constants.Aggregate(lexer, (cur, constant) => cur.AddPattern(Regex.Escape(constant), constant));

        private static Lexer AddPatterns(this Lexer lexer, XDocument definition) =>
            lexer.AddPatterns(LexicalPatterns(definition));

        private static Lexer AddPatterns(this Lexer lexer, IEnumerable<(string expression, string name)> patterns) =>
            patterns.Aggregate(lexer, (cur, pattern) => cur.AddPattern(pattern.expression, pattern.name));

        private static IEnumerable<string> ConstantLexemes(XDocument definition) =>
            definition.Root
                ?.Element("LexicalRules")
                ?.Elements("Constant")
                .Select(constant => constant.Attribute("Value")?.Value ?? string.Empty)
                .Where(constant => !string.IsNullOrEmpty(constant))
            ?? Enumerable.Empty<string>();

        private static IEnumerable<string> IgnorePatterns(XDocument definition) =>
            definition.Root
                ?.Element("LexicalRules")
                ?.Elements("Ignore")
                .Select(ignore => ignore.Attribute("Symbol")?.Value ?? string.Empty)
                .Where(pattern => !string.IsNullOrEmpty(pattern))
            ?? Enumerable.Empty<string>();

        private static IEnumerable<(string pattern, string name)> LexicalPatterns(XDocument definition) =>
            definition.Root
                ?.Element("LexicalRules")
                ?.Elements("Lexeme")
                .Select(lexeme => (pattern: lexeme.Attribute("Symbol")?.Value ?? string.Empty, name: lexeme.Attribute("Name")?.Value ?? string.Empty))
                .Where(tuple => !string.IsNullOrEmpty(tuple.pattern) && !string.IsNullOrEmpty(tuple.name))
            ?? Enumerable.Empty<(string, string)>();
                
    }
}