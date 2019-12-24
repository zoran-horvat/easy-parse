using System.Collections.Generic;
using System.Collections.Immutable;
using EasyParse.ParserGenerator.Models.Symbols;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.ParserGenerator.Collections
{
    public abstract class TransitionTable<TState, TSymbol, TResult> where TSymbol : Symbol
    {
        public IEnumerable<Transition<TState, TSymbol, TResult>> Items => this.Content;

        protected Set<Transition<TState, TSymbol, TResult>> Content { get; }

        protected TransitionTable() : this(new Set<Transition<TState, TSymbol, TResult>>())
        {
        }

        protected TransitionTable(Set<Transition<TState, TSymbol, TResult>> content)
        {
            this.Content = content;
        }
    }
}
