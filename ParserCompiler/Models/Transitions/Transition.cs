using ParserCompiler.Collections;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models.Transitions
{
    public class Transition
    {
        public State From { get; }
        public Symbol Symbol { get; }
        public State To { get; }

        public Transition(State from, Symbol symbol, State to)
        {
            this.From = from;
            this.Symbol = symbol;
            this.To = to;
        }

        public Transition Closure() =>
            new Transition(this.From, this.Symbol, this.To.Closure());

        public CoreTransition ToCore() =>
            new CoreTransition(this.From.Core, this.Symbol, this.To.Core);
    }
}
