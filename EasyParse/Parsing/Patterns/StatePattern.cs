using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.Parsing.Patterns
{
    abstract class StatePattern
    {
        public static StatePattern From(ReduceCommand reduce) =>
            reduce.Symbol is ParserGenerator.Models.Symbols.EndOfInput ? (StatePattern)new StateEnd(reduce.From)
            : new StateIndexAndLabel(reduce.From, reduce.Symbol.Value);
    }
}
