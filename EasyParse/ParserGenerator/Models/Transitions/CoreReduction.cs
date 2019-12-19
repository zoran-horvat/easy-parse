using EasyParse.ParserGenerator.Collections;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Transitions
{
    public class CoreReduction : Transition<Core, Terminal, Rule>
    {
        public CoreReduction(Core from, Terminal symbol, Rule to) : base(from, symbol, to)
        {
        }
    }
}