namespace EasyParse.ParserGenerator.Models.Rules
{
    public class LexemePattern : Lexeme
    {
        public string Name { get; }

        public LexemePattern(string name, string pattern) : base(pattern)
        {
            this.Name = name;
        }

        public override string ToString() =>
            $"{this.Name} matching \"{this.Pattern}\"";
    }
}