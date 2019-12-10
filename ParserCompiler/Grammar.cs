using System;
using System.Collections.Generic;
using System.Linq;
using ParserCompiler.Symbols;

namespace ParserCompiler
{
    public class Grammar
    {
        private IEnumerable<Rule> Rules { get; }
     
        public Grammar(IEnumerable<Rule> rules)
        {
            this.Rules = rules.ToList();
        }

        public Set<FirstSet> FirstSets =>
            this.Rules
                .Select(rule => (head: rule.Head, body: rule.Body.Take(1)))
                .SelectMany(tuple => tuple.body.Select(terminal => (head: tuple.head, terminal: terminal)))
                .GroupBy(tuple => tuple.head, tuple => tuple.terminal)
                .Select(group => new FirstSet(group.Key, group))
                .AsSet();

        public override string ToString() =>
            string.Join(Environment.NewLine, this.Rules.Select(rule => rule.ToString()));
    }
}