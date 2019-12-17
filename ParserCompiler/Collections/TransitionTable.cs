using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ParserCompiler.Models.Symbols;
using ParserCompiler.Models.Transitions;

namespace ParserCompiler.Collections
{
    public abstract class TransitionTable<TState, TSymbol> where TSymbol : Symbol
    {
        public IEnumerable<Transition<TState, TSymbol>> Items => this.Content;

        protected ImmutableList<Transition<TState, TSymbol>> Content { get; }

        protected TransitionTable() : this(ImmutableList<Transition<TState, TSymbol>>.Empty)
        {
        }

        protected TransitionTable(ImmutableList<Transition<TState, TSymbol>> content)
        {
            this.Content = content;
        }
    }
}
