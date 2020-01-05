using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Formatting;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Rules
{
    public class Rule : IEquatable<Rule>
    {
        public NonTerminal Head { get; }
        public IEnumerable<Symbol> Body { get; }

        public IEnumerable<ConstantLexeme> ConstantLexemes =>
            this.Body.OfType<Constant>().Select(constant => new ConstantLexeme(constant.Value));
     
        public Rule(NonTerminal head, IEnumerable<Symbol> body)
        {
            this.Head = head;
            this.Body = body.ToList();
        }

        public static string AugmentedRootNonTerminal => "S'";

        public static Rule AugmentedGrammarRoot(string startingSymbol) =>
            new Rule(new NonTerminal(AugmentedRootNonTerminal), new Symbol[] {new NonTerminal(startingSymbol)});

        public Progression ToProgression() => new Progression(this);

        public override string ToString() => Formatter.ToString(this);

        public override bool Equals(object obj) =>
            this.Equals(obj as Rule);

        public bool Equals(Rule other) =>
            !(other is null) &&
            other.Head.Equals(this.Head) &&
            other.Body.SequenceEqual(this.Body);

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Body.Aggregate(this.Head.GetHashCode() * 397, (acc, symbol) => acc ^ symbol.GetHashCode());
            }
        }
    }
}