using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EasyParse.Parsing.Rules;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing
{
    internal class GrammarToGrammarFileFormatter
    {
        public IEnumerable<string> Convert(IEnumerable<RegexSymbol> ignoreLexemes, NonTerminalName startSymbol, IEnumerable<Production> productions) => 
            this.ConvertLexemes(ignoreLexemes, this.RegularExpressionsIn(productions))
            .Concat(this.ConvertStartSymbol(startSymbol))
            .Concat(this.ConvertRules(productions));

        private IEnumerable<string> ConvertLexemes(IEnumerable<RegexSymbol> ignoreLexemes, IEnumerable<RegexSymbol> patterns) =>
            new[] { "lexemes:" }
            .Concat(this.ConvertIgnoreLexemes(ignoreLexemes))
            .Concat(ConvertRegexLexemes(patterns));

        private IEnumerable<string> ConvertStartSymbol(NonTerminalName start) =>
            new[] { string.Empty, $"start: {start};" };

        private IEnumerable<string> ConvertRules(IEnumerable<Production> productions) =>
            new[] { string.Empty, $"rules:" }
            .Concat(this.ConvertProductions(productions));

        private IEnumerable<RegexSymbol> RegularExpressionsIn(IEnumerable<Production> productions) =>
            productions.SelectMany(production => production.Body).OfType<RegexSymbol>();

        private IEnumerable<string> ConvertIgnoreLexemes(IEnumerable<RegexSymbol> ignoreLexemes) =>
            ignoreLexemes.Select(lexeme => $"ignore {this.ToLiteral(lexeme)};");

        private IEnumerable<string> ConvertRegexLexemes(IEnumerable<RegexSymbol> patterns) =>
            patterns.Select(pattern => $"match {pattern.Name} is {this.ToLiteral(pattern)};");

        private string ToLiteral(RegexSymbol pattern) => 
            this.ToLiteral(pattern.Expression);
        private string ToLiteral(Regex pattern) =>
            $"'{pattern.ToString().Replace("'", "\\'")}'";

        private IEnumerable<string> ConvertProductions(IEnumerable<Production> productions) =>
            productions.Select(this.ConvertProduction);

        private string ConvertProduction(Production production) =>
            $"{production.Head.Name} -> {this.ConvertSymbols(production.Body)};";

        private string ConvertSymbols(IEnumerable<Symbol> symbols) => 
            string.Join(" ", symbols.Select(this.ConvertSymbol));

        private string ConvertSymbol(Symbol symbol) =>
            symbol is LiteralSymbol literal ? $"'{literal.Value.Replace("'", "\\'")}'"
            : symbol is RegexSymbol regex ? regex.Name
            : symbol is NonTerminalSymbol nonTerminal ? nonTerminal.Head.Name
            : symbol.ToString();
    }
}