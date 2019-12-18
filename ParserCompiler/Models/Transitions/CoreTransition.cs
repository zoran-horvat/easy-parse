using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models.Transitions
{
    public class CoreTransition : Transition<Core, Symbol, Core>
    {
        public CoreTransition(Core from, Symbol symbol, Core to) : base(from, symbol, to)
        {
        }
    }
}
