using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace EasyParse.Parsing.Collections
{
    static class XmlDefinitionUtils
    {
        public static IDictionary<(int state, string label), int> ExtractShift(XDocument definition) =>
            definition.Root
                ?.Elements("ParsingTable")
                .SelectMany(table => table.Elements("Shift"))
                .Select(el => (
                    state: int.Parse(el.Attribute("State")?.Value ?? "-1"),
                    label: el.Attribute("Terminal")?.Value ?? string.Empty,
                    toState: int.Parse(el.Attribute("TransitionTo")?.Value ?? "-1")))
                .ToDictionary(tuple => (tuple.state, tuple.label), tuple => tuple.toState) 
                ?? new Dictionary<(int state, string label), int>();
    }
}
