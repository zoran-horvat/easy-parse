using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing.Rules
{
    internal class EmptyRule : IEmptyRule
    {
        public EmptyRule(NonTerminal head)
        {
            this.EmptyProduction = new Production(head);
        }

        private Production EmptyProduction { get; }

        public IPendingProductionEnd Literal(string value) =>
            this.BeginProduction(new LiteralSymbol(value));

        public IPendingProductionEnd Regex(string name, string pattern) =>
            this.BeginProduction(new RegexSymbol(name, new Regex(pattern)));

        public IPendingProductionEnd Symbol(Func<IRule> factory) =>
            this.BeginProduction(new RecursiveNonTerminalSymbol(factory));

        private IPendingProductionEnd BeginProduction(Symbol symbol) =>
            new IncompleteProductionBuilder(ImmutableList<Production>.Empty, this.EmptyProduction.Append(symbol));
    }
}