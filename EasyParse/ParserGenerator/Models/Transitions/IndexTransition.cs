using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Transitions
{
    public abstract class IndexTransition<T> : Transition<int, T, int> where T : Symbol
    {
        protected IndexTransition(int from, T symbol, int to) : base(from, symbol, to)
        {
        }

        public override string ToString() => $"({this.From}, {this.Symbol}) -> {this.To}";
    }
}