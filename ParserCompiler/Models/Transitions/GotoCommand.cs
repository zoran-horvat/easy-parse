using System;
using System.Collections.Generic;
using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models.Transitions
{
    public class GotoCommand : IndexTransition<NonTerminal>
    {
        private GotoCommand(int fromState, NonTerminal symbol, int toState) : base(fromState, symbol, toState)
        {
        }

        public static GotoCommand Of(CoreTransition transition, IDictionary<Set<Progression>, int> coreToIndex) =>
            transition.Symbol is NonTerminal symbol ? new GotoCommand(coreToIndex[transition.From], symbol, coreToIndex[transition.To])
            : throw new ArgumentException();
    }
}