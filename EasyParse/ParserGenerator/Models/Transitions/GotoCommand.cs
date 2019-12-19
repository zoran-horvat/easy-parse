using System;
using System.Collections.Generic;
using EasyParse.ParserGenerator.Collections;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Transitions
{
    public class GotoCommand : IndexTransition<NonTerminal>
    {
        private GotoCommand(int fromState, NonTerminal symbol, int toState) : base(fromState, symbol, toState)
        {
        }

        public static GotoCommand Of(CoreTransition transition, IDictionary<Core, int> coreToIndex) =>
            transition.Symbol is NonTerminal symbol ? new GotoCommand(coreToIndex[transition.From], symbol, coreToIndex[transition.To])
            : throw new ArgumentException();
    }
}