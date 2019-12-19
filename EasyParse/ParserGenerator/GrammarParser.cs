using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Symbols;

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
    }
}
