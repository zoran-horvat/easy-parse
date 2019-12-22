using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EasyParse.Parsing.Patterns;

namespace EasyParse.Parsing.Collections
{
    class ShiftTable
    {
        private IDictionary<StateIndexAndLabel, int> StateToNextState { get; }
     
        public ShiftTable(XDocument definition)
        {
            this.StateToNextState = XmlDefinitionUtils.ExtractShift(definition);
        }

        public IEnumerable<int> StateFor(StatePattern pattern) =>
            pattern is StateIndexAndLabel indexAndLabel && this.StateToNextState.TryGetValue(indexAndLabel, out int nextState) ? new [] {nextState}
            : Enumerable.Empty<int>();
    }
}
