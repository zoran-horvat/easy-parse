﻿using System;
using System.Collections.Generic;

namespace EasyParse.Parsing.Rules.Symbols
{

    public class NonTerminalSymbol : Symbol
    {
        public NonTerminalSymbol(IRule rule)
        {
            this.Rule = rule;
        }

        public IRule Rule { get; }
        public override Type Type => this.Rule.Type;
        public NonTerminalName Head => this.Rule.Head;
        public IEnumerable<Production> Productions => this.Rule.Productions;

        public override ParserGenerator.Models.Symbols.Symbol ToSymbolModel() =>
            new ParserGenerator.Models.Symbols.NonTerminal(this.Rule.Head.Name);

        public override string ToString() => 
            this.Rule.Head.ToString();
    }
}