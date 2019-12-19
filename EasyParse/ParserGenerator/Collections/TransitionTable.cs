using System.Collections.Generic;
using System.Collections.Immutable;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.ParserGenerator.Collections
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
