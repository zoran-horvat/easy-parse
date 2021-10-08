using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing.Rules
{
    internal class IncompleteProductionBuilder : IPendingProductionEnd
    {
        public IncompleteProductionBuilder(ImmutableList<Production> completedLines, Production currentLine)
        {
            this.CompletedLines = completedLines;
            this.CurrentLine = currentLine;
        }

        private ImmutableList<Production> CompletedLines { get; }
        private Production CurrentLine { get; }

        public IPendingProductionEnd Literal(string value) =>
            this.Append(new LiteralSymbol(value));

        public IPendingProductionEnd Regex(string name, string pattern) =>
            this.Append(new RegexSymbol(name, new Regex(pattern)));

        public IPendingProductionEnd Symbol(Func<IRule> factory) =>
            this.Append(new RecursiveNonTerminalSymbol(factory));

        public IRule End() =>
            new CompletedRule(this.CurrentLine.Head, this.CompletedLines.Add(this.CurrentLine));

        private IPendingProductionEnd Append(Symbol symbol) =>
            new IncompleteProductionBuilder(this.CompletedLines, this.CurrentLine.Append(symbol));
    }
}