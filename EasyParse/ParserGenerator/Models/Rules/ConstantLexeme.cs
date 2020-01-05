using System.Text.RegularExpressions;

namespace EasyParse.ParserGenerator.Models.Rules
{
    public class ConstantLexeme : Lexeme
    {
        public string ConstantValue { get; }

        public ConstantLexeme(string value) : base(Regex.Escape(value))
        {
            this.ConstantValue = value;
        }

        public override string ToString() =>
            $"Constant \"{this.ConstantValue}\"";
    }
}
