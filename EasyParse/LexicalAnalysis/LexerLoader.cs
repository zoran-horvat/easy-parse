using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace EasyParse.LexicalAnalysis
{
    internal static class LexerLoader
    {
        public static Lexer From(XDocument definition) =>
            new Lexer()
                .AddIgnorePatterns(definition)
                .AddConstants(ConstantLexemes(definition))
                .AddPatterns(LexicalPatterns(definition));

        private static Lexer AddIgnorePatterns(this Lexer lexer, XDocument definition) => 
            IgnorePatterns(definition).Aggregate(lexer, (cur, ignore) => cur.IgnorePattern(ignore));
        
        private static Lexer AddConstants(this Lexer lexer, IEnumerable<string> constants) =>
            constants.Aggregate(lexer, (cur, constant) => cur.AddPattern(Regex.Escape(constant), constant));

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
                .Select(ignore => ignore.Attribute("Pattern")?.Value ?? string.Empty)
                .Where(pattern => !string.IsNullOrEmpty(pattern))
            ?? Enumerable.Empty<string>();

        private static IEnumerable<(string pattern, string name)> LexicalPatterns(XDocument definition) =>
            definition.Root
                ?.Element("LexicalRules")
                ?.Elements("Lexeme")
                .Select(lexeme => (pattern: lexeme.Attribute("Pattern")?.Value ?? string.Empty, name: lexeme.Attribute("Name")?.Value ?? string.Empty))
                .Where(tuple => !string.IsNullOrEmpty(tuple.pattern) && !string.IsNullOrEmpty(tuple.name))
            ?? Enumerable.Empty<(string, string)>();
                
    }
}