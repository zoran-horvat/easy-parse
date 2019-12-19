using EasyParse.Collections;
using EasyParse.Models.Symbols;

namespace EasyParse.Models.Transitions
{
    public class CoreTransition : Transition<Core, Symbol, Core>
    {
        public CoreTransition(Core from, Symbol symbol, Core to) : base(from, symbol, to)
        {
        }
    }
}
