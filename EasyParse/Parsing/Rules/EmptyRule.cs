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
            new IncompleteProductionBuilder(ImmutableList<Production>.Empty, this.Head)
                .Match(first, others);
    }
}