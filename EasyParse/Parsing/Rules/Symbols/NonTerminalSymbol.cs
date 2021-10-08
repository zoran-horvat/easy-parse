using System.Collections.Generic;

namespace EasyParse.Parsing.Rules.Symbols
{
    abstract class NonTerminalSymbol : Symbol
    {
        public abstract NonTerminal Head { get; }
        public abstract IEnumerable<Production> Productions { get; }
    }

    class NonTerminalSymbol<T> : NonTerminalSymbol
    {
        public NonTerminalSymbol(IRule<T> rule)
        {
            this.Rule = rule;
        }

        public IRule<T> Rule { get; }
        public override NonTerminal Head => this.Rule.Head;
        public override IEnumerable<Production> Productions => this.Rule.Productions;

        public override ParserGenerator.Models.Symbols.Symbol ToSymbolModel() =>
            new ParserGenerator.Models.Symbols.NonTerminal(this.Rule.Head.Name);

        public override string ToString() => 
            this.Rule.Head.ToString();
    }
}
