﻿using EasyParse.ParserGenerator.Models.Symbols;

namespace EasyParse.Fluent.Symbols
{
    public abstract class TerminalSymbol : Symbol
    {
        protected TerminalSymbol(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override ParserGenerator.Models.Symbols.Symbol ToSymbolModel() =>
            new Terminal(this.Name);
    }
}