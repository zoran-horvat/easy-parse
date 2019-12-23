using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EasyParse.Parsing.Patterns;

namespace EasyParse.Parsing.Collections
{
    class GotoTable
    {
        private IDictionary<StateIndexAndLabel, int> StateToNextState { get; }
     
        public GotoTable(XDocument definition)
        {
            this.StateToNextState = XmlDefinitionUtils.ExtractGoto(definition);
        }

        public IEnumerable<int> NextStateFor(StateIndexAndLabel state) =>
            this.StateToNextState.TryGetValue(state, out int nextStateIndex) ? new[] {nextStateIndex}
            : Enumerable.Empty<int>();
    }
}
