﻿using System.Linq;
using EasyParse.ParserGenerator.Models.Rules;
using EasyParse.ParserGenerator.Models.Transitions;

namespace EasyParse.Parsing.Patterns
{
    class RulePattern
    {
        public RuleReference Reference { get; }
        public string NonTerminal { get; }
        public int BodyLength { get; }

        public RulePattern(RuleReference reference, string nonTerminal, int bodyLength)
        {
            this.Reference = reference;
            this.NonTerminal = nonTerminal;
            this.BodyLength = bodyLength;
        }

        public static RulePattern From(ReduceCommand reduce, RuleDefinition[] rules) =>
            new RulePattern(rules[reduce.To].Reference, rules[reduce.To].Head.Value, rules[reduce.To].Body.Count());

        public static string AugmentedRootNonTerminal => "S'";

        public override string ToString() =>
            $"{this.NonTerminal} -> ({this.BodyLength} symbol(s))";
    }
}
