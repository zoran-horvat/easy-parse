using System;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing.Rules
{
    public abstract class Symbol
    {
        public abstract Type Type { get; }
        public abstract ParserGenerator.Models.Symbols.Symbol ToSymbolModel();

        public static implicit operator Symbol(string value) =>
            new LiteralSymbol(value);

        public static implicit operator Symbol(NonTerminal nonTerminal) =>
            new RecursiveNonTerminalSymbol(nonTerminal.Invoke);
    }
}
