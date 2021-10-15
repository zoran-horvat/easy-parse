using System;
using System.Collections.Immutable;

namespace EasyParse.Parsing.Rules
{
    internal class EmptyRule : IEmptyRule
    {
        public EmptyRule(NonTerminalName head)
        {
            this.Head = head;
        }

        private NonTerminalName Head { get; }

        public IPendingMapping Match(Symbol first, params Symbol[] others) =>
            this.BeginProduction().Match(first, others);

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