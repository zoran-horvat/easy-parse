using EasyParse.ParserGenerator.Models.Rules;

namespace EasyParse.Parsing.Rules
{
    class ConstantSymbol : TerminalSymbol
    {
        public ConstantSymbol(string value) : base(value)
        {
        }

        public string Value => base.Name;

        public override Lexeme ToLexemeModel() =>
            new ConstantLexeme(this.Value);
    }
}
