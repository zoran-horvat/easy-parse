using System;

namespace EasyParse.Fluent.Symbols
{
    class LiteralSymbol : TerminalSymbol
    {
        public LiteralSymbol(string value) : base(value)
        {
        }

        public string Value => Name;
        public override Type Type => typeof(string);

        public override ParserGenerator.Models.Symbols.Symbol ToSymbolModel() =>
            new ParserGenerator.Models.Symbols.Constant(this.Value);

        public override string ToString() =>
            this.Value;
    }
}
