using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models
{
    public class IndexTransition<T> where T : Symbol
    {
        public int FromState { get; }
        public T Symbol { get; }
        public int ToState { get; }

        public IndexTransition(int fromState, T symbol, int toState)
        {
            FromState = fromState;
            Symbol = symbol;
            ToState = toState;
        }

        public override string ToString() => $"({this.FromState}, {this.Symbol}) -> {this.ToState}";
    }
}