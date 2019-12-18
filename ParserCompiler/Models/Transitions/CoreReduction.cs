using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models.Transitions
{
    public class CoreReduction : Transition<Core, Terminal, Rule>
    {
        public CoreReduction(Core from, Terminal symbol, Rule to) : base(from, symbol, to)
        {
        }
    }
}