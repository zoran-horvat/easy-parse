using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing.Rules
{
    internal class IncompleteProductionBuilder<T> : IPendingMapping<T>
    {
        public IncompleteProductionBuilder(ImmutableList<Production> completedLines, Production currentLine)
        {
            this.CompletedLines = completedLines;
            this.CurrentLine = currentLine;
        }

        private ImmutableList<Production> CompletedLines { get; }
        private Production CurrentLine { get; }

        public IPendingMapping<T> Literal(string value) =>
            this.Append(new LiteralSymbol(value));

        public IPendingMapping<T> Regex(string name, string pattern) =>
            this.Append(new RegexSymbol(name, new Regex(pattern)));

        public IPendingMapping<T> Symbol(Func<IRule<T>> factory) =>
            this.Append(new RecursiveNonTerminalSymbol<T>(factory));

        public IRule<T> End() =>
            new CompletedRule<T>(this.CurrentLine.Head, this.CompletedLines.Add(this.CurrentLine));

        private IPendingMapping<T> Append(Symbol symbol) =>
            new IncompleteProductionBuilder<T>(this.CompletedLines, this.CurrentLine.Append(symbol));
    }
}