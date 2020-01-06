using System.Linq;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.Parsing.Patterns
{
    class RulePattern
    {
        public string NonTerminal { get; }
        public int BodyLength { get; }

        public RulePattern(string nonTerminal, int bodyLength)
        {
            this.NonTerminal = nonTerminal;
            this.BodyLength = bodyLength;
        }

        public static RulePattern From(ReduceCommand reduce, RuleDefinition[] rules) =>
            new RulePattern(rules[reduce.To].Head.Value, rules[reduce.To].Body.Count());

        public static string AugmentedRootNonTerminal => "S'";

        public override string ToString() =>
            $"{this.NonTerminal} -> ({this.BodyLength} symbol(s))";
    }
}
