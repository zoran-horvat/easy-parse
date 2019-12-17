using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models
{
    public abstract class IndexTransition<T> where T : Symbol
    {
        public int FromState { get; }
        public T Symbol { get; }
        public int ToState { get; }

        protected IndexTransition(int fromState, T symbol, int toState)
        {
            FromState = fromState;
            Symbol = symbol;
            ToState = toState;
        }

        public override string ToString() => $"({this.FromState}, {this.Symbol}) -> {this.ToState}";
    }
}