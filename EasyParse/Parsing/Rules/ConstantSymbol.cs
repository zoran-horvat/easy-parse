namespace EasyParse.Parsing.Rules
{
    class ConstantSymbol : TerminalSymbol
    {
        public ConstantSymbol(string value) : base(value)
        {
        }

        public string Value => base.Name;

        public override ParserGenerator.Models.Symbols.Symbol ToSymbolModel() =>
            new ParserGenerator.Models.Symbols.Constant(this.Value);

        public override string ToString() =>
            this.Value;
    }
}
