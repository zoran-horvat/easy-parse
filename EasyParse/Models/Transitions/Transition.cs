using EasyParse.Models.Symbols;

namespace EasyParse.Models.Transitions
{
    public abstract class Transition<TState, TSymbol, TResult> where TSymbol : Symbol
    {
        public TState From { get; }
        public TSymbol Symbol { get; }
        public TResult To { get; }

        protected Transition(TState from, TSymbol symbol, TResult to)
        {
            this.From = from;
            this.Symbol = symbol;
            this.To = to;
        }
    }
}