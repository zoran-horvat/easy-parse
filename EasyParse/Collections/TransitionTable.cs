using System.Collections.Generic;
using System.Collections.Immutable;
using EasyParse.Models.Symbols;
using EasyParse.Models.Transitions;

namespace EasyParse.Collections
{
    public abstract class TransitionTable<TState, TSymbol, TResult> where TSymbol : Symbol
    {
        public IEnumerable<Transition<TState, TSymbol, TResult>> Items => this.Content;

        protected ImmutableList<Transition<TState, TSymbol, TResult>> Content { get; }

        protected TransitionTable() : this(ImmutableList<Transition<TState, TSymbol, TResult>>.Empty)
        {
        }

        protected TransitionTable(ImmutableList<Transition<TState, TSymbol, TResult>> content)
        {
            this.Content = content;
        }
    }
}
