using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EasyParse.Fluent.Rules;
using EasyParse.Fluent.Symbols;

namespace EasyParse.Fluent
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

        private IEnumerable<string> ConvertProductions(IEnumerable<Production> productions) 
        {
            List<(string representation, Production production)> block = this.PrepareProductions(productions).ToList();
            int textWidth = block.Select(pair => pair.representation.Length).DefaultIfEmpty(0).Max();

            return block
                .Select(pair => 
                    !pair.production.Reference.IsValidAsKey ? (representation: pair.representation, label: string.Empty)
                    : (representation: pair.representation.PadRight(textWidth + 2), label: pair.production.Reference.ToString()))
                .Select(pair => $"{pair.representation}{pair.label}");
        }

        private IEnumerable<(string representation, Production production)> PrepareProductions(IEnumerable<Production> productions) =>
            productions.Select(this.ConvertProduction);

        private (string representation, Production Production) ConvertProduction(Production production) =>
            ($"{production.Head.Name} -> {this.ConvertSymbols(production.Body)};", production);

        private string ConvertSymbols(IEnumerable<Symbol> symbols) => 
            string.Join(" ", symbols.Select(this.ConvertSymbol));

        private string ConvertSymbol(Symbol symbol) =>
            symbol is LiteralSymbol literal ? $"'{literal.Value.Replace("'", "\\'")}'"
            : symbol is RegexSymbol regex ? regex.Name
            : symbol is NonTerminalSymbol nonTerminal ? nonTerminal.Head.Name
            : symbol.ToString();
    }
}