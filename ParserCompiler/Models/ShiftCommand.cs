using System.Collections.Generic;
using System.Collections.Immutable;
using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models
{
    public class ShiftCommand : IndexTransition<Terminal>
    {
        private ShiftCommand(int fromState, Terminal symbol, int toState) : base(fromState, symbol, toState)
        {
        }

        public static ImmutableList<ShiftCommand> Add(CoreTransition transition, IDictionary<Set<Progression>, int> coreToInt, ImmutableList<ShiftCommand> to) =>
            transition.Symbol is Terminal symbol ? to.Add(new ShiftCommand(coreToInt[transition.FromCore], symbol, coreToInt[transition.ToCore])) : to;
    }
}