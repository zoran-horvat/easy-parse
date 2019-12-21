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
                .SelectMany(reduce => ExtractReduce(reduce, rules))
                .ToDictionary(tuple => tuple.pattern, tuple => tuple.rule)
            ?? new Dictionary<StatePattern, RulePattern>();

        private static IEnumerable<(StatePattern pattern, RulePattern rule)> ExtractReduce(XElement reduce, RulePattern[] rules) =>
            reduce.Attributes("Terminal").Select(terminal => ExtractTerminalReduce(reduce, terminal, rules));

        private static (StatePattern pattern, RulePattern rule) ExtractTerminalReduce(
            XElement reduce, XAttribute terminal, RulePattern[] rules) =>
            ExtractTerminalReduce(
                int.Parse(reduce.Attribute("State")?.Value ?? "-1"),
                terminal?.Value ?? string.Empty,
                int.Parse(reduce.Attribute("RuleOrdinal")?.Value ?? "-1"),
                rules);

        private static (StatePattern pattern, RulePattern rule) ExtractTerminalReduce(
            int stateIndex, string terminal, int ruleOrdinal, RulePattern[] rules) =>
            (new StateIndexAndLabel(stateIndex, terminal), rules[ruleOrdinal]);
    }
}
