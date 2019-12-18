using System;
using System.Collections.Generic;
using ParserCompiler.Collections;
using ParserCompiler.Models.Rules;
using ParserCompiler.Models.Symbols;

namespace ParserCompiler.Models.Transitions
{
    public class ShiftCommand : IndexTransition<Terminal>
    {
        private ShiftCommand(int fromState, Terminal symbol, int toState) : base(fromState, symbol, toState)
        {
        }

        public static ShiftCommand Of(CoreTransition transition, IDictionary<Core, int> coreToIndex) =>
            transition.Symbol is Terminal terminal ? new ShiftCommand(coreToIndex[transition.From], terminal, coreToIndex[transition.To]) 
            : throw new ArgumentException(); 
    }
}