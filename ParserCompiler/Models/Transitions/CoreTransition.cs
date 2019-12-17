using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models.Transitions
{
    public class CoreTransition : Transition<Set<Progression>, Symbol>
    {
        public CoreTransition(Set<Progression> from, Symbol symbol, Set<Progression> to) : base(from, symbol, to)
        {
        }
    }
}
