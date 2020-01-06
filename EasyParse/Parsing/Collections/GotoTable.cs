using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EasyParse.ParserGenerator.Collections;
using EasyParse.Parsing.Patterns;

namespace EasyParse.Parsing.Collections
{
    class GotoTable
    {
        private IDictionary<StateIndexAndLabel, int> StateToNextState { get; }
     
        private GotoTable(IDictionary<StateIndexAndLabel, int> stateToNextState)
        {
            this.StateToNextState = stateToNextState;
        }

        public static GotoTable From(XDocument definition) =>
            new GotoTable(XmlDefinitionUtils.ExtractGoto(definition));

        public static GotoTable From(GotoTableDefinition definition) =>
            new GotoTable(definition.ToDictionary(
                @goto => new StateIndexAndLabel(@goto.From, @goto.Symbol.Value),
                @goto => @goto.To));

        public IEnumerable<int> NextStateFor(StateIndexAndLabel state) =>
            this.StateToNextState.TryGetValue(state, out int nextStateIndex) ? new[] {nextStateIndex}
            : Enumerable.Empty<int>();
    }
}
