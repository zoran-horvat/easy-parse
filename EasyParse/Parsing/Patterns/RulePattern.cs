namespace EasyParse.Parsing.Patterns
{
    class RulePattern
    {
        private string NonTerminal { get; }
        private int BodyLength { get; }

        public RulePattern(string nonTerminal, int bodyLength)
        {
            this.NonTerminal = nonTerminal;
            this.BodyLength = bodyLength;
        }

        public override string ToString() =>
            $"{this.NonTerminal} -> ({this.BodyLength} symbol(s))";
    }
}
