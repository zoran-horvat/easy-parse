using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models.Transitions
{
    public class CoreTransition : Transition<Set<Progression>, Symbol, Set<Progression>>
    {
        public CoreTransition(Set<Progression> from, Symbol symbol, Set<Progression> to) : base(from, symbol, to)
        {
        }
    }
}
