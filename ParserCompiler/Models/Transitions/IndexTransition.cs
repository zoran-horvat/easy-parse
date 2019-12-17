using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models.Transitions
{
    public abstract class IndexTransition<T> : Transition<int, T> where T : Symbol
    {
        protected IndexTransition(int @from, T symbol, int to) : base(@from, symbol, to)
        {
        }

        public override string ToString() => $"({this.From}, {this.Symbol}) -> {this.To}";
    }
}