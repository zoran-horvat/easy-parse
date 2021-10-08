using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing.Rules
{
    internal class IncompleteProductionBuilder : IPendingMapping
    {
        public IncompleteProductionBuilder(ImmutableList<Production> completedLines, Production currentLine)
        {
            this.CompletedLines = completedLines;
            this.CurrentLine = currentLine;
        }

        private ImmutableList<Production> CompletedLines { get; }
        private Production CurrentLine { get; }

        public IPendingMapping Literal(string value) =>
            this.Append(new LiteralSymbol(value));

        public IPendingMapping Regex(string name, string pattern) =>
            this.Append(new RegexSymbol(name, new Regex(pattern)));

        public IPendingMapping Symbol(Func<IRule> factory) =>
            this.Append(new RecursiveNonTerminalSymbol(factory));

        public IRule End() =>
            new CompletedRule(this.CurrentLine.Head, this.CompletedLines.Add(this.CurrentLine));

        private IPendingMapping Append(Symbol symbol) =>
            new IncompleteProductionBuilder(this.CompletedLines, this.CurrentLine.Append(symbol));
    }
}