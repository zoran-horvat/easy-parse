using EasyParse.ParserGenerator.Collections;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Transitions
{
    public class CoreTransition : Transition<Core, Symbol, Core>
    {
        public CoreTransition(Core from, Symbol symbol, Core to) : base(from, symbol, to)
        {
        }
    }
}
