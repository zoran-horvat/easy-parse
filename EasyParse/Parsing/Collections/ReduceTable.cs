using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EasyParse.ParserGenerator.Collections;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.Parsing.Patterns;

namespace EasyParse.Parsing.Collections
{
    class ReduceTable
    {
        private IDictionary<StatePattern, RulePattern> StateToRule { get; }

        private ReduceTable(IDictionary<StatePattern, RulePattern> stateToRule)
        {
            this.StateToRule = stateToRule;
        }

        public static ReduceTable From(XDocument definition) =>
            new ReduceTable(XmlDefinitionUtils.ExtractReduce(definition));

        public static ReduceTable From(ReduceTableDefinition definition, RuleDefinition[] rules) =>
            new ReduceTable(definition.ToDictionary(
                reduce => StatePattern.From(reduce),
                reduce => RulePattern.From(reduce, rules)));

        public IEnumerable<RulePattern> ReductionFor(StatePattern state) =>
            this.StateToRule.TryGetValue(state, out RulePattern rule) ? new [] {rule}
            : Enumerable.Empty<RulePattern>();
    }
}
