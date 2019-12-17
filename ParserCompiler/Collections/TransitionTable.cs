using System.Collections.Immutable;
using ParserCompiler.Models.Symbols;
using ParserCompiler.Models.Transitions;

namespace ParserCompiler.Collections
{
    public class TransitionTable<TState, TSymbol> where TSymbol : Symbol
    {
        protected ImmutableList<Transition<TState, TSymbol>> Content { get; }

        public TransitionTable() : this(ImmutableList<Transition<TState, TSymbol>>.Empty)
        {
        }

        protected TransitionTable(ImmutableList<Transition<TState, TSymbol>> content)
        {
            this.Content = content;
        }
    }
}
