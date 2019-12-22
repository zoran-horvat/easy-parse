using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EasyParse.Parsing.Patterns;

namespace EasyParse.Parsing.Collections
{
    class ReduceTable
    {
        private IDictionary<StatePattern, RulePattern> StateToRule { get; }

        public ReduceTable(XDocument definition)
        {
            this.StateToRule = XmlDefinitionUtils.ExtractReduce(definition);
        }

        public IEnumerable<RulePattern> ReductionFor(StatePattern state) =>
            this.StateToRule.TryGetValue(state, out RulePattern rule) ? new [] {rule}
            : Enumerable.Empty<RulePattern>();
    }
}
