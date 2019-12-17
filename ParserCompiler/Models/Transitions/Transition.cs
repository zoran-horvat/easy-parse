using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models.Transitions
{
    public abstract class Transition<TState, TSymbol> where TSymbol : Symbol
    {
        public TState From { get; }
        public TSymbol Symbol { get; }
        public TState To { get; }

        public Transition(TState @from, TSymbol symbol, TState to)
        {
            this.From = from;
            this.Symbol = symbol;
            this.To = to;
        }
    }
}