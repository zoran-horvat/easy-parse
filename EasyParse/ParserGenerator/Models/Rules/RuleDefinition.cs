using System;
using System.Collections.Generic;
using System.Linq;
using EasyParse.ParserGenerator.Formatting;
using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.ParserGenerator.Models.Rules
{
    public class RuleDefinition : IEquatable<RuleDefinition>
    {
        public string Reference { get; }
        public NonTerminal Head { get; }
        public IEnumerable<Symbol> Body { get; }

        public IEnumerable<ConstantLexeme> ConstantLexemes =>
            this.Body.OfType<Constant>().Select(constant => new ConstantLexeme(constant.Value));

        public RuleDefinition(NonTerminal head, IEnumerable<Symbol> body)
            : this(Guid.NewGuid().ToString(), head, body.ToList())
        {
        }

        private RuleDefinition(string reference, NonTerminal head, IEnumerable<Symbol> body)
        {
            this.Reference = reference;
            this.Head = head;
            this.Body = body;
        }

        public RuleDefinition WithReference(string value) =>
            new RuleDefinition(value, this.Head, this.Body);

        public static string AugmentedRootNonTerminal => "S'";

        public static RuleDefinition AugmentedGrammarRoot(NonTerminal startSymbol) =>
            new RuleDefinition(new NonTerminal(AugmentedRootNonTerminal), new Symbol[] {startSymbol});

        public Progression ToProgression() => new Progression(this);

        public override string ToString() => Formatter.ToString(this);

        public override bool Equals(object obj) =>
            this.Equals(obj as RuleDefinition);

        public bool Equals(RuleDefinition other) =>
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