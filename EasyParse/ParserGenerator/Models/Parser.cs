using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EasyParse.ParserGenerator.Collections;
using EasyParse.ParserGenerator.Formatting;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models
{
    public class Parser
    {
        public Grammar Grammar { get; }
        public Set<FirstSet> FirstSets { get; }
        public Set<FollowSet> FollowSets { get; }
        public StateVector States { get; }
        public ParsingTable Table { get; }

        public Parser(Grammar grammar, Set<FirstSet> firstSets, Set<FollowSet> followSets, StateVector states, ParsingTable table)
        {
            this.Grammar = grammar;
            this.FirstSets = firstSets;
            this.FollowSets = followSets;
            this.States = states;
            this.Table = table;
        }

        public XDocument ToXml() => new XDocument(
            new XElement("ParserDefinition",
                this.LexicalRulesToXml(),
                this.GrammarToXml(),
                this.ParsingTableToXml()));

        private XElement LexicalRulesToXml() =>
            new XElement("LexicalRules", 
                this.IgnoreLexemesToXml().Concat(this.LexemePatternsToXml()));

        private IEnumerable<XElement> IgnoreLexemesToXml() =>
            this.Grammar.IgnoreLexemes.Select(this.IgnoreLexemeToXml);
        private XElement IgnoreLexemeToXml(IgnoreLexeme lexeme) =>
            new XElement("Ignore", new XAttribute("Pattern", lexeme.Pattern.ToString()));

        private IEnumerable<XElement> LexemePatternsToXml() =>
            this.Grammar.LexemePatterns.Select(this.LexemePatternToXml);

        private XElement LexemePatternToXml(LexemePattern lexeme) =>
            new XElement("Lexeme", new XAttribute("Pattern", lexeme.Pattern.ToString()), new XAttribute("Name", lexeme.Name));

        private XElement GrammarToXml() =>
            new XElement("Grammar", this.Grammar.Rules.Select(this.RuleToXml));

        private XElement RuleToXml(Rule rule, int index) =>
            new XElement("Rule", new XAttribute("Ordinal", index),
                new XElement("Head", this.SymbolsToXml(rule.Head)),
                new XElement("Body", this.SymbolsToXml(rule.Body)));

        private IEnumerable<XElement> SymbolsToXml(params Symbol[] symbols) =>
            this.SymbolsToXml((IEnumerable<Symbol>) symbols);

        private IEnumerable<XElement> SymbolsToXml(IEnumerable<Symbol> symbols) =>
            symbols.Select(symbol => new XElement(symbol is Terminal ? "Terminal" : "NonTerminal", new XAttribute("Name", symbol.Value)));

        private XElement ParsingTableToXml() =>
            new XElement("ParsingTable", 
                this.ShiftsToXml(),
                this.ReducesToXml(),
                this.GotosToXml());

        private IEnumerable<XElement> ShiftsToXml() =>
            this.Table.Shift.OrderBy(shift => shift.From).ThenBy(shift => shift.Symbol.Value).Select(shift => 
                new XElement("Shift", 
                    new XAttribute("State", shift.From), 
                    new XAttribute("Terminal", shift.Symbol.Value), 
                    new XAttribute("TransitionTo", shift.To)));

        private IEnumerable<XElement> ReducesToXml() =>
            this.Table.Reduce.OrderBy(reduce => reduce.From).ThenBy(reduce => reduce.Symbol.Value).Select(reduce =>
                new XElement("Reduce",
                    new XAttribute("State", reduce.From),
                    new XAttribute(reduce.Symbol is EndOfInput ? "EndOfInput" : "Terminal", reduce.Symbol.Value),
                    new XAttribute("RuleOrdinal", reduce.To)));

        private IEnumerable<XElement> GotosToXml() =>
            this.Table.Goto.OrderBy(@goto => @goto.From).ThenBy(@goto => @goto.Symbol.Value).Select(@goto =>
                new XElement("Goto",
                    new XAttribute("State", @goto.From),
                    new XAttribute("NonTerminal", @goto.Symbol.Value),
                    new XAttribute("TransitionTo", @goto.To)));

        public override string ToString() => Formatter.ToString(this);
    }
}
