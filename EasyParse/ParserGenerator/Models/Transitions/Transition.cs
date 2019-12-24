using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Transitions
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

        public override bool Equals(object obj) =>
            obj is Transition<TState, TSymbol, TResult> other &&
            other.From.Equals(this.From) &&
            other.Symbol.Equals(this.Symbol) &&
            other.To.Equals(this.To);

        public override int GetHashCode() =>
            this.From.GetHashCode() ^ this.Symbol.GetHashCode() ^ this.To.GetHashCode();
    }
}