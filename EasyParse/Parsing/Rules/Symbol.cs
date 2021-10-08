using System;
using System.Linq;
using EasyParse.Parsing.Rules.Symbols;

namespace EasyParse.Parsing.Rules
{
    public abstract class Symbol
    {
        public abstract ParserGenerator.Models.Symbols.Symbol ToSymbolModel();

        public static implicit operator Symbol(string value) =>
            new LiteralSymbol(value);

        public static Symbol From(object obj) =>
            obj is null ? throw new ArgumentNullException(nameof(obj))
            : obj is Symbol symbol ? symbol
            : obj is string str ? new LiteralSymbol(str)
            : obj is Func<IRule> factory ? new RecursiveNonTerminalSymbol(factory)
            : throw new ArgumentException($"Unsupported symbol type {obj.GetType().Name}");

        public static Symbol[] From(object[] objects) =>
            objects.Select(From).ToArray();
    }
}
