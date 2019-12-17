using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models.Transitions
{
    public class CoreReduction : Transition<Set<Progression>, Terminal, Rule>
    {
        public CoreReduction(Set<Progression> from, Terminal symbol, Rule to) : base(from, symbol, to)
        {
        }
    }
}