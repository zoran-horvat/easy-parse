using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserCompiler
{
    public class Grammar
    {
        private IEnumerable<Rule> Rules { get; }
     
        public Grammar(IEnumerable<Rule> rules)
        {
            this.Rules = rules.ToList();
        }

        public override string ToString() =>
            string.Join(Environment.NewLine, this.Rules.Select(rule => rule.ToString()));
    }
}