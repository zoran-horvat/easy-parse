using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EasyParse.ParserGenerator.Collections;
using EasyParse.Parsing.Patterns;

namespace EasyParse.Parsing.Collections
{
    class ShiftTable
    {
        private IDictionary<StateIndexAndLabel, int> StateToNextState { get; }
     
        private ShiftTable(IDictionary<StateIndexAndLabel, int> stateToNextState)
        {
            this.StateToNextState = stateToNextState;
        }

        public IEnumerable<int> StateFor(StatePattern pattern) =>
            pattern is StateIndexAndLabel indexAndLabel && this.StateToNextState.TryGetValue(indexAndLabel, out int nextState) ? new [] {nextState}
            : Enumerable.Empty<int>();

        public static ShiftTable From(XDocument definition) =>
            new ShiftTable(XmlDefinitionUtils.ExtractShift(definition));

        public static ShiftTable From(ShiftTableDefinition definition) =>
            new ShiftTable(definition.ToDictionary(
                transition => new StateIndexAndLabel(transition.From, transition.Symbol.Value),
                transition => transition.To));

    }
}
