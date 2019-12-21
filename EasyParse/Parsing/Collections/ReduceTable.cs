using System.Collections.Generic;
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
    }
}
