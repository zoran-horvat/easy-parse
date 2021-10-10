using System;
using System.Collections.Immutable;

namespace EasyParse.Parsing.Rules
{
    internal class EmptyRule : IEmptyRule
    {
        public EmptyRule(NonTerminal head)
        {
            this.Head = head;
        }

        private NonTerminal Head { get; }

        public IPendingMapping Literal(string value) =>
            this.BeginProduction().Literal(value);

        public IPendingMapping Regex<T>(string name, string pattern, Func<string, T> transform) =>
            this.BeginProduction().Regex(name, pattern, transform);

        public IPendingMapping Symbol(Func<IRule> factory) =>
            this.BeginProduction().Symbol(factory);

        private IPendingMapping BeginProduction() =>
            new IncompleteProductionBuilder(ImmutableList<Production>.Empty, this.Head);
    }
}