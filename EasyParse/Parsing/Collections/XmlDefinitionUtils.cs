using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EasyParse.Parsing.Patterns;

namespace EasyParse.Parsing.Collections
{
    static class XmlDefinitionUtils
    {
        public static IDictionary<StateIndexAndLabel, int> ExtractShift(XDocument definition) =>
            definition.Root
                ?.Elements("ParsingTable")
                .SelectMany(table => table.Elements("Shift"))
                .Select(el => (
                    state: int.Parse(el.Attribute("State")?.Value ?? "-1"),
                    label: el.Attribute("Terminal")?.Value ?? string.Empty,
                    toState: int.Parse(el.Attribute("TransitionTo")?.Value ?? "-1")))
                .ToDictionary(tuple => new StateIndexAndLabel(tuple.state, tuple.label), tuple => tuple.toState) 
                ?? new Dictionary<StateIndexAndLabel, int>();

        public static IDictionary<StatePattern, RulePattern> ExtractReduce(XDocument definition) =>
            ExtractReduce(definition, ExtractRules(definition));

        private static RulePattern[] ExtractRules(XDocument definition) =>
            definition.Root
                ?.Elements("Grammar")
                .Elements("Rule")
                .Select(rule => (
                    head: rule.Element("Head")?.Element("NonTerminal")?.Attribute("Name")?.Value ?? string.Empty,
                    count: rule.Element("Body")?.Elements().Count() ?? 0))
                .Select(tuple => new RulePattern(tuple.head, tuple.count))
                .ToArray();

        private static IDictionary<StatePattern, RulePattern> ExtractReduce(XDocument definition, RulePattern[] rules) =>
            definition.Root?.Element("ParsingTable")?.Elements("Reduce")
                .Select(reduce => ExtractReduce(reduce, rules))
                .ToDictionary(tuple => tuple.pattern, tuple => tuple.rule)
            ?? new Dictionary<StatePattern, RulePattern>();

        private static (StatePattern pattern, RulePattern rule) ExtractReduce(XElement reduce, RulePattern[] rules) =>
            ExtractReduce(
                int.Parse(reduce.Attribute("State")?.Value ?? "-1"),
                reduce.Attributes("Terminal").Select(attribute => attribute.Value),
                int.Parse(reduce.Attribute("RuleOrdinal")?.Value ?? "-1"),
                rules);

        private static (StatePattern pattern, RulePattern rule) ExtractReduce(
            int stateIndex, IEnumerable<string> terminal, int ruleIndex, RulePattern[] rules) => 
            (StatePattern(stateIndex, terminal), rules[ruleIndex]);

        private static StatePattern StatePattern(int stateIndex, IEnumerable<string> terminal) =>
            terminal
                .Select<string, StatePattern>(label => new StateIndexAndLabel(stateIndex, label))
                .DefaultIfEmpty(new StateEnd(stateIndex))
                .First();
    }
}
