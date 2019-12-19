namespace EasyParse.LexicalAnalysis
{
    public class Lexeme
    {
        public string Label { get; }
        public string Value { get; }

        public Lexeme(string label, string value)
        {
            this.Label = label;
            this.Value = value;
        }
    }
}
