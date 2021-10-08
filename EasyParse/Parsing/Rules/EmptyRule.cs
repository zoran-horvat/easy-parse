using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing.Rules
{
    internal class EmptyRule<T> : IEmptyRule<T>
    {
        public EmptyRule(NonTerminal head)
        {
            this.EmptyProduction = new Production(head);
        }

        private Production EmptyProduction { get; }

        public IPendingMapping<T> Literal(string value) =>
            this.BeginProduction(new LiteralSymbol(value));

        public IPendingMapping<T> Regex(string name, string pattern) =>
            this.BeginProduction(new RegexSymbol(name, new Regex(pattern)));

        public IPendingMapping<T> Symbol(Func<IRule<T>> factory) =>
            this.BeginProduction(new RecursiveNonTerminalSymbol<T>(factory));

        private IPendingMapping<T> BeginProduction(Symbol symbol) =>
            new IncompleteProductionBuilder<T>(ImmutableList<Production>.Empty, this.EmptyProduction.Append(symbol));
    }
}