using System.Collections.Generic;
using System.Collections.Immutable;
using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models
{
    public class GotoCommand : IndexTransition<NonTerminal>
    {
        private GotoCommand(int fromState, NonTerminal symbol, int toState) : base(fromState, symbol, toState)
        {
        }

        public static ImmutableList<GotoCommand> Add(CoreTransition transition, IDictionary<Set<Progression>, int> coreToInt, ImmutableList<GotoCommand> to) =>
            transition.Symbol is NonTerminal symbol ? to.Add(new GotoCommand(coreToInt[transition.FromCore], symbol, coreToInt[transition.ToCore])) : to;
    }
}