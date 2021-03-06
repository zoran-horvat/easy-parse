﻿using EasyParse.ParserGenerator.Collections;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Transitions
{
    public class StateTransition : Transition<State, Symbol, State>
    {
        public StateTransition(State @from, Symbol symbol, State to) : base(@from, symbol, to)
        {
        }

        public StateTransition Closure() =>
            new StateTransition(this.From, this.Symbol, this.To.Closure());

        public CoreTransition ToCore() =>
            new CoreTransition(this.From.Core, this.Symbol, this.To.Core);
    }
}
