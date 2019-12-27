using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EasyParse.LexicalAnalysis;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Symbols;
using Match = System.Text.RegularExpressions.Match;

namespace EasyParse.ParserGenerator
{
    public class GrammarParser
    {
        public Grammar Parse(IEnumerable<string> rawRules) =>
            new Grammar(rawRules.SelectMany(this.Parse));

        private IEnumerable<Rule> Parse(string line) =>
            this.LineMatch(line).Select(this.Parse);

        private IEnumerable<Match> LineMatch(string line) =>
            Regex.Matches(line, "^(?<head>[A-Z])\\s->\\s(?<body>.+)$").OfType<Match>();

        private Rule Parse(Match lineMatch) =>
            this.Parse(lineMatch.Groups["head"].Value, lineMatch.Groups["body"].Value);

        private Rule Parse(string head, string body) => 
            new Rule(new NonTerminal(head), this.ParseBody(body));

        private IEnumerable<Symbol> ParseBody(string body) =>
            body.ToCharArray().Select(Symbol.From);

        public static Lexer CreateLexer() =>
            new Lexer()
                .AddPattern("[A-Z]", "n")
                .AddPattern("[a-z]", "t")
                .AddPattern(@"\->", "a")
                .IgnorePattern(@"\s");
    }
}
