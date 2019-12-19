using EasyParse.Collections;
using EasyParse.Models.Rules;
using EasyParse.Models.Symbols;

namespace EasyParse.Models.Transitions
{
    public class CoreReduction : Transition<Core, Terminal, Rule>
    {
        public CoreReduction(Core from, Terminal symbol, Rule to) : base(from, symbol, to)
        {
        }
    }
}