namespace EasyParse.ParserGenerator.Models.Rules
{
    public class IgnoreLexeme : Lexeme
    {
        public IgnoreLexeme(string pattern) : base(pattern)
        {
        }

        public override string ToString() => 
            $"Ignore \"{this.Pattern}\"";
    }
}
